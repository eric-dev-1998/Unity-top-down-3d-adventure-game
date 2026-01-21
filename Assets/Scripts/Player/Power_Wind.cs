using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Power_Wind : Power
    {
        ParticleSystem windParticles;
        BoxCollider collider;

        public Power_Wind(float cooldownTime, float energy, GameObject playerPowers) : base(cooldownTime, energy, playerPowers) 
        {
            windParticles = playerPowers.transform.Find("Wind").transform.Find("WindSlash").GetComponent<ParticleSystem>();
            collider = windParticles.transform.Find("Collider").GetComponent<BoxCollider>();
            collider.enabled = false;
        }

        public override void Use()
        {
            base.Use();

            // Power usage logic.

            // Get wind slash particles:
            windParticles = playerPowers.transform.Find("Wind").transform.Find("WindSlash").GetComponent<ParticleSystem>();

            // Play wind particles fx:
            windParticles.Play();
        }

        public override void Cooldown(float deltaTime)
        {
            base.Cooldown(deltaTime);

            if (windParticles.isPlaying)
            {
                GameObject.FindAnyObjectByType<PlayerCore>().GetEntity().LockMovement();
                collider.enabled = true;
            }
            else
            {
                GameObject.FindAnyObjectByType<PlayerCore>().GetEntity().UnlockMovement();
                collider.enabled = false;
            }
        }
    }
}
