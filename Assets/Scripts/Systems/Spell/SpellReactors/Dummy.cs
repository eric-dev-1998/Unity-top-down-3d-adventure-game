using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Assets.Scripts.Systems.Spell
{
    public class Dummy : SpellInteraction
    {
        public override void OnLowHealth()
        {
            Destroy(gameObject);
        }
        public override void OnWind()
        {
            PlayerCore player = FindAnyObjectByType<PlayerCore>();
            transform.position += player.transform.forward * 0.1f * Time.deltaTime;
        }

        public override void OnFire()
        {
            if (!damaged)
            {
                health -= 1;
                damaged = true;
            }
        }

        public override void ReactToEarth()
        {
            if (rock.GetComponent<Rigidbody>().linearVelocity.magnitude > 0.3f)
            {
                // Hit.
                if (!damaged)
                {
                    PlayerCore player = FindAnyObjectByType<PlayerCore>();
                    transform.position += player.transform.forward * 0.5f;

                    health -= 3;
                    damaged = true;
                }
            }
        }
    }
}