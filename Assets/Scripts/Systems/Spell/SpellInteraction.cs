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
        public int health = 10;
        public float damageCooldown = 1.5f;
        private float damageTimer = 0f;
        public bool damaged = false;

        public bool canBeBlasted = false;
        public bool canBurn = false;
        public bool canGetWet = false;
        public bool canBeBlownAway = false;
        public bool canGetSmashed = false;

        public SpellCaster caster;
        public Projectile projectile;

        private float elapsedTime = 0f;
        private float durationInSeconds = 5f;
        private float cooldownInSeconds = 1f;
        private enum State { None, Burning, Wet, Blowing };
        private State state = State.None;

        private bool reacting = false;

        private GameObject vfx;
        public GameObject rock;

        public void Update()
        {
            if (state == State.Blowing)
            {
                OnWind();

                if (!caster.casting)
                    state = State.None;
            }

            if (state == State.Burning)
                OnFire();
            if (state == State.Wet)
                OnWater();

            if (reacting)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= durationInSeconds)
                {
                    elapsedTime = 0f;
                    StopReactionVfx();
                }
            }

            if (damaged)
            {
                damageTimer += Time.deltaTime;
                if (damageTimer >= damageCooldown)
                {
                    damaged = false;
                    damageTimer = 0f;
                }
            }
        }

        public virtual void React(string tag)
        {
            switch (tag)
            {
                case "Spell_Neutral":
                    ReactToNeutral();
                    break;

                case "Spell_Fire":
                    ReactToFire();
                    break;

                case "Spell_Water":
                    ReactToWater();
                    break;

                case "Spell_Wind":
                    ReactToWind();
                    break;

                case "Spell_Earth":
                    ReactToEarth();
                    break;
            }

            if (health <= 0)
                OnLowHealth();
        }

        public void StopReaction(string tag)
        {
            switch (tag)
            {
                case "Spell_Neutral":
                    if (canBeBlasted)
                        OnNeutralEnd();
                    break;

                case "Spell_Fire":
                    if (canBurn)
                        OnFireEnd();
                    break;

                case "Spell_Water":
                    if (canGetWet)
                        OnWaterEnd();
                    break;

                case "Spell_Wind":
                    if (canBeBlownAway)
                        OnWindEnd();
                    break;

                case "Spell_Earth":
                    if (canGetSmashed)
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
            if (vfx != null)
            { 
                ParticleSystem particles = vfx.GetComponent<ParticleSystem>();
                if (particles != null)
                {
                    particles.Stop();
                }
                else
                    Destroy(vfx);

                reacting = false;
                state = State.None;
            }
        }

        private void Burn()
        {
            if (state == State.None)
            {
                state = State.Burning;

                GameObject vfxAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Spells/BurningParticles.prefab");
                vfx = Instantiate(vfxAsset, transform);

                ShowEffectVFX();

                reacting = true;
                elapsedTime = 0f;
            }
            else if (state == State.Wet)
            {
                vfx.GetComponent<ParticleSystem>().Stop();
                state = State.None;
                elapsedTime = 0f;
                Burn();
            }
        }

        private void Soak()
        {
            if (state == State.None)
            {
                state = State.Wet;

                GameObject vfxAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Spells/WetParticles.prefab");
                vfx = Instantiate(vfxAsset, transform);

                ShowEffectVFX();

                reacting = true;
                elapsedTime = 0f;
            }
            else if (state == State.Burning)
            {
                vfx.GetComponent<ParticleSystem>().Stop();
                state = State.None;
                elapsedTime = 0f;
                Soak();
            }
        }

        public virtual Mesh GetMesh() { return null; }

        public virtual void SetupMesh(ShapeModule shapeModule) { }

        private void ShowEffectVFX()
        {
            ParticleSystem particles = vfx.GetComponent<ParticleSystem>();
            particles.transform.eulerAngles = new Vector3(0, -90, 0);
            particles.transform.localPosition = Vector3.zero;
            ShapeModule shapeModule = particles.shape;

            SetupMesh(shapeModule);
        }
        public virtual void ReactToNeutral() { if (!canBeBlasted) return; }

        public virtual void ReactToFire() { if (!canBurn) return; Burn(); }

        public virtual void ReactToWater() { if (!canGetWet) return; Soak(); }

        public virtual void ReactToWind() { if (!canBeBlownAway) return; state = State.Blowing; }

        public virtual void ReactToEarth() { if (!canGetSmashed) return; }

        public virtual void OnNeutralEnd() { }

        public virtual void OnFireEnd() { }

        public virtual void OnWaterEnd() { }

        public virtual void OnWindEnd() { }

        public virtual void OnEarthEnd() { }

        public virtual void OnFire() { }

        public virtual void OnWater() { }

        public virtual void OnWind() { }

        public virtual void OnEarth() { }

        public virtual void OnLowHealth() { }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.tag == "Spell_Earth")
                rock = other.gameObject;

            try
            {
                caster = other.transform.parent.GetComponent<SpellCaster>();
            }
            catch
            {
                projectile = other.transform.GetComponent<Projectile>();
            }
            React(other.tag); 
        }

        private void OnTriggerStay(Collider other)
        {
            if (!reacting)
            {
                //React(other.tag);
            }
        }
        
        private void OnTriggerExit(Collider other) { StopReaction(other.tag); }
    }
}
