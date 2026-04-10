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
        public bool canBeBlasted = false;
        public bool canBurn = false;
        public bool canGetWet = false;
        public bool canBeBlownAway = false;
        public bool canGetSmashed = false;

        private float elapsedTime = 0f;
        private float durationInSeconds = 5f;
        private float cooldownInSeconds = 1f;

        private bool reacting = false;

        private GameObject vfx;

        private void Update()
        {
            if (reacting)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= durationInSeconds)
                {
                    StopReactionVfx();
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

        public void StopReactionVfx()
        {
            if (vfx != null)
            { 
                ParticleSystem particles = vfx.GetComponent<ParticleSystem>();
                if (particles != null)
                {
                    EmissionModule emission = particles.emission;
                    emission.enabled = false;

                    if (particles.particleCount <= 0)
                        Destroy(vfx);
                }
                else
                    Destroy(vfx);
            }
        }

        private void Burn()
        {
            if (reacting)
                return;

            GameObject vfxAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Spells/BurningParticles.prefab");
            vfx = Instantiate(vfxAsset, transform);

            ParticleSystem particles = vfx.GetComponent<ParticleSystem>();
            ShapeModule shapeModule = particles.shape;

            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.mesh = GetComponent<MeshFilter>().mesh;
            shapeModule.scale = transform.localScale;

            reacting = true;
            elapsedTime = 0f;
        }

        private void Soak()
        {
            if (reacting)
                return;

            GameObject vfxAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Spells/WetParticles.prefab");
            vfx = Instantiate(vfxAsset, transform);

            ParticleSystem particles = vfx.GetComponent<ParticleSystem>();
            ShapeModule shapeModule = particles.shape;

            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.mesh = GetComponent<MeshFilter>().mesh;
            shapeModule.scale = transform.localScale;

            reacting = true;
            elapsedTime = 0f;
        }

        public virtual void ReactToNeutral() { if (!canBeBlasted) return; }

        public virtual void ReactToFire() { if (!canBurn) return; Burn(); }

        public virtual void ReactToWater() { if (!canGetWet) return; Soak(); }

        public virtual void ReactToWind() { if (!canBeBlownAway) return; }

        public virtual void ReactToEarth() { if (!canGetSmashed) return; }

        public virtual void OnNeutralEnd() { }

        public virtual void OnFireEnd() { }

        public virtual void OnWaterEnd() { }

        public virtual void OnWindEnd() { }

        public virtual void OnEarthEnd() { }

        private void OnTriggerEnter(Collider other) { React(other.tag); }

        private void OnTriggerExit(Collider other) { StopReaction(other.tag); }
    }
}
