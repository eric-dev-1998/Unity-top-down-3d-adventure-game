using Assets.Scripts.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Assets.Scripts.Systems.Spell
{
    public class Dummy : SpellInteraction
    {
        public override void OnLowHealth()
        {
            StopReactionVfx();

            CapsuleCollider[] colliders = GetComponents<CapsuleCollider>();
            foreach (CapsuleCollider c in colliders)
                c.enabled = false;

            transform.Find("Mesh").transform.gameObject.SetActive(false);
            transform.Find("DeathParticles").transform.GetComponent<ParticleSystem>().Play();
            enabled = false;
        }
        public override void OnWind()
        {
            Animator animator = GetComponent<Animator>();

            transform.rotation = Quaternion.LookRotation(caster.transform.forward, Vector3.up);

            Animator anim = GetComponent<Animator>();
            anim.SetBool("Blow", true);
        }

        public override void OnWindEnd()
        {
            base.OnWindEnd();

            Animator anim = GetComponent<Animator>();
            anim.SetBool("Blow", false);
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
                    Animator animator = GetComponent<Animator>();

                    transform.rotation = Quaternion.LookRotation(-caster.transform.forward, Vector3.up);
                    animator.SetBool("Hit", true);
                    StartCoroutine(WaitForAnimationToEnd(animator));

                    health -= 3;
                    damaged = true;
                }
            }
        }

        private IEnumerator WaitForAnimationToEnd(Animator anim)
        {
            yield return new WaitForSeconds(0.1f);
            anim.SetBool("Hit", false);
        }
    }
}