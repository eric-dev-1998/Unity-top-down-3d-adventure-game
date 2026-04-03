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
        private float durationInSeconds = 5f;
        private float cooldownInSeconds = 1f;

        private bool reacting = false;

        public void React(string tag)
        {
            switch (tag)
            {
                case "Spell_Neutral":
                    break;

                case "Spell_Fire":
                    Burn();
                    break;

                case "Spell_Water":
                    Soak();
                    break;

                case "Spell_Wind":
                    break;

                case "Spell_Earth":
                    break;
            }
        }

        private void Burn()
        {
            if (reacting)
                return;

            GameObject vfxAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Spells/BurningParticles.prefab");
            GameObject vfx = Instantiate(vfxAsset, transform);

            ParticleSystem particles = vfx.GetComponent<ParticleSystem>();
            ShapeModule shapeModule = particles.shape;

            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.mesh = GetComponent<MeshFilter>().mesh;
            shapeModule.scale = transform.localScale;

            reacting = true;
        }

        private void Soak()
        {
            if (reacting)
                return;

            GameObject vfxAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Spells/WetParticles.prefab");
            GameObject vfx = Instantiate(vfxAsset, transform);

            ParticleSystem particles = vfx.GetComponent<ParticleSystem>();
            ShapeModule shapeModule = particles.shape;

            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.mesh = GetComponent<MeshFilter>().mesh;
            shapeModule.scale = transform.localScale;

            reacting = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            React(other.tag);
        }
    }
}
