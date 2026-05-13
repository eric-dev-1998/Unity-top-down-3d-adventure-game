using Assets.Scripts.World.Enemy;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Assets.Scripts.Systems.Spell
{
    public class SpellInteraction : MonoBehaviour
    {
        [Header("Spell interaction properties")]
        public int Health = 10;
        public int MaxHealth = 10;
        public float DamageCooldown = 1.5f;
        public float DamageTimer = 0f;
        public bool IsDamaged = false;

        public bool CanBeBlasted = false;
        public bool CanBurn = false;
        public bool CanGetWet = false;
        public bool CanBeBlownAway = false;
        public bool CanGetSmashed = false;

        public SpellCaster Caster;
        public Projectile Projectile;

        private float _elapsedTime = 0f;
        private float _durationInSeconds = 5f;
        private float _cooldownInSeconds = 1f;
        private enum State { None, Burning, Wet, Blowing };
        private State _state = State.None;

        private bool _reacting = false;

        private Vector3 _lastCollisionPoint;
        private GameObject _vfx;
        public GameObject Rock;

        public void Update()
        {
            if (_state == State.Blowing)
                OnWind();

            if (_state == State.Burning)
                OnFire();

            if (_state == State.Wet)
                OnWater();

            if (_reacting)
            {
                _elapsedTime += Time.deltaTime;
                if (_elapsedTime >= _durationInSeconds)
                {
                    _elapsedTime = 0f;
                    StopReactionVfx();
                }
            }

            if (IsDamaged)
            {
                DamageTimer += Time.deltaTime;
                if (DamageTimer >= DamageCooldown)
                {
                    IsDamaged = false;
                    DamageTimer = 0f;
                }
            }

            UpdateExternal();
        }

        public virtual void React(string tag)
        {
            switch (tag)
            {
                case "Spell_Neutral":
                    ReactToNeutral();
                    ReactToAny();
                    break;

                case "Spell_Fire":
                    ReactToFire();
                    ReactToAny();
                    break;

                case "Spell_Water":
                    ReactToWater();
                    ReactToAny();
                    break;

                case "Spell_Wind":
                    ReactToWind();
                    ReactToAny();
                    break;

                case "Spell_Earth":
                    ReactToEarth();
                    ReactToAny();
                    break;
            }

            if (Health <= 0)
                OnLowHealth();
        }

        public void StopReaction(string tag)
        {
            switch (tag)
            {
                case "Spell_Neutral":
                    if (CanBeBlasted)
                        OnNeutralEnd();
                    break;

                case "Spell_Fire":
                    if (CanBurn)
                        OnFireEnd();
                    break;

                case "Spell_Water":
                    if (CanGetWet)
                        OnWaterEnd();
                    break;

                case "Spell_Wind":
                    if (CanBeBlownAway)
                        OnWindEnd();
                    break;

                case "Spell_Earth":
                    if (CanGetSmashed)
                        OnEarthEnd();
                    break;
            }
        }

        public void StopReactionVfx(ParticleSystem p)
        {
            p.Stop();
        }

        public void StopReactionVfx()
        {
            if (_vfx != null)
            { 
                ParticleSystem particles = _vfx.GetComponent<ParticleSystem>();
                if (particles != null)
                {
                    particles.Stop();
                }
                else
                    Destroy(_vfx);

                _reacting = false;
                _state = State.None;

                Animator anim = GetComponent<Animator>();
                anim.SetBool("Burn", false);
                anim.SetBool("Soak", false);
            }
        }

        private void Burn()
        {
            if (_state == State.None)
            {
                _state = State.Burning;

                GameObject vfxAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Spells/BurningParticles.prefab");
                _vfx = Instantiate(vfxAsset, transform);

                ShowEffectVFX();

                _reacting = true;
                _elapsedTime = 0f;
            }
            else if (_state == State.Wet)
            {
                _vfx.GetComponent<ParticleSystem>().Stop();
                _state = State.None;
                _elapsedTime = 0f;
                Burn();
            }
        }

        private void Soak()
        {
            if (_state == State.None)
            {
                _state = State.Wet;

                GameObject vfxAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Spells/WetParticles.prefab");
                _vfx = Instantiate(vfxAsset, transform);

                ShowEffectVFX();

                _reacting = true;
                _elapsedTime = 0f;
            }
            else if (_state == State.Burning)
            {
                _vfx.GetComponent<ParticleSystem>().Stop();
                _state = State.None;
                _elapsedTime = 0f;
                Soak();
            }
        }

        public virtual Mesh GetMesh() { return null; }

        public virtual void SetupMesh(ShapeModule shapeModule) { }

        private void ShowEffectVFX()
        {
            ParticleSystem particles = _vfx.GetComponent<ParticleSystem>();
            particles.transform.eulerAngles = new Vector3(0, -90, 0);
            particles.transform.localPosition = Vector3.zero;
            ShapeModule shapeModule = particles.shape;

            SetupMesh(shapeModule);
        }

        public virtual void UpdateExternal() { }

        public virtual void ReactToAny() { }

        public virtual void ReactToNeutral() { if (!CanBeBlasted) return; }

        public virtual void ReactToFire() { if (!CanBurn) return; Burn(); }

        public virtual void ReactToWater() { if (!CanGetWet) return; Soak(); }

        public virtual void ReactToWind() { if (!CanBeBlownAway) return; _state = State.Blowing; }

        public virtual void ReactToEarth() { if (!CanGetSmashed) return; }

        public virtual void OnNeutralEnd() { _state = State.None; }

        public virtual void OnFireEnd() { _state = State.None; }

        public virtual void OnWaterEnd() { _state = State.None; }

        public virtual void OnWindEnd() { _state = State.None; }

        public virtual void OnEarthEnd() { _state = State.None; }

        public virtual void OnFire() { }

        public virtual void OnWater() { }

        public virtual void OnWind() { }

        public virtual void OnEarth() { }

        public virtual void OnLowHealth() { Destroy(gameObject); }

        public virtual void OnPlayerCollision() { }

        public Vector3 GetLastCollisionPoint() { return _lastCollisionPoint; }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.GetComponent<EnemyCore>() && GetComponent<EnemyCore>())
                return;

            _lastCollisionPoint = other.transform.position;

            if (other.tag == "Player")
                OnPlayerCollision();

            if (other.tag == "Spell_Earth")
                Rock = other.gameObject;

            if (other.tag == "Spell_Fire" || other.tag == "Spell_Water")
                return;

            try
            {
                Caster = other.transform.parent.GetComponent<SpellCaster>();
            }
            catch
            {
                Projectile = other.transform.GetComponent<Projectile>();
            }

            React(other.tag); 
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.tag == "Spell_Fire" || other.tag == "Spell_Water")
            {
                Debug.Log($"Collided with {other.tag}");
                React(other.tag);
            }
        }

        private void OnTriggerExit(Collider other) { StopReaction(other.tag); }

        public GameObject GetVfx() { return _vfx; }
    }
}
