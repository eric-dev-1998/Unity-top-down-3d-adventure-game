using Main;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Pile_Of_Sand : MonoBehaviour
    {
        public GameObject hiddenObject;

        private GameObject mesh;
        private ParticleSystem sandParticles;
        private float cooldownTime = 1.5f;
        private float cooldownCounter = 0.0f;
        private float health = 1.0f;
        private float targetDamage = 0f;
        private float hitDamage = 0.4f;
        private bool hit = false;

        private void Start()
        {
            sandParticles = transform.Find("VFX").GetComponent<ParticleSystem>();
            mesh = transform.Find("Mesh").gameObject;

            if (hiddenObject != null)
                hiddenObject.GetComponent<Pickup>().enabled = false;
        }

        private void Update()
        {
            if (health <= 0.15f)
            {
                mesh.SetActive(false);
                sandParticles.gameObject.SetActive(false);
                transform.Find("Collider").gameObject.SetActive(false);

                if(hiddenObject != null)
                    hiddenObject.GetComponent<Pickup>().enabled = true;
            }
            else
            {
                if (hit)
                {
                    // Cooldown management:
                    cooldownCounter += Time.deltaTime;
                    if (cooldownCounter >= cooldownTime)
                    {
                        hit = false;
                        cooldownCounter = 0.0f;
                    }

                    // Scale down based on damage:
                    if (health > 0.15f)
                    {
                        health = Mathf.Lerp(health, targetDamage, Time.deltaTime);
                        mesh.transform.localScale = new Vector3(health, health, health);
                    }
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Power_Wind")
            {
                if (!hit)
                {
                    transform.Find("VFX").eulerAngles = other.transform.parent.eulerAngles;
                    sandParticles.Play();
                    hit = true;

                    targetDamage = health - hitDamage;
                }
            }
        }
    }
}
