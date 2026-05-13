using Assets.Scripts.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Diagnostics;
using static UnityEngine.ParticleSystem;

namespace Assets.Scripts.Systems.Spell
{
    public class Dummy : SpellInteraction
    {
        Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public override void OnLowHealth()
        {
            StopReactionVfx();
            StartCoroutine(Kill());
        }
        public override void OnWind()
        {
            Vector3 direction = Caster.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(-direction, Vector3.up);
            
            anim.SetBool("Blow", true);
        }

        public override void OnWindEnd()
        {
            Debug.Log("Miau");
            anim.SetBool("Blow", false);

            base.OnWindEnd();
        }
        public override void OnFire()
        {
            if (!IsDamaged)
            {
                Health -= 1;
                IsDamaged = true;
            }
        }

        public override void ReactToEarth()
        {
            if (Rock.GetComponent<Rigidbody>().linearVelocity.magnitude > 0.3f)
            {
                // Hit.
                if (!IsDamaged)
                {
                    Animator animator = GetComponent<Animator>();

                    Vector3 direction = Rock.GetComponent<Rigidbody>().linearVelocity.normalized;
                    direction.y = 0f;

                    transform.rotation = Quaternion.LookRotation(-direction, Vector3.up);
                    animator.SetBool("Hit", true);
                    StartCoroutine(WaitForAnimationToEnd(animator));

                    Health -= 3;
                    IsDamaged = true;
                }
            }
        }

        public override void ReactToNeutral()
        {
            base.ReactToNeutral();

            Animator animator = GetComponent<Animator>();

            transform.rotation = Quaternion.LookRotation(-Projectile.GetCaster().transform.forward, Vector3.up);
            animator.SetBool("Hit", true);
            StartCoroutine(WaitForAnimationToEnd(animator));

            Health -= Projectile.power;
            Projectile.DestroySelf();
            IsDamaged = true;
        }

        public override Mesh GetMesh()
        {
            Debug.Log("Got mesh from dummy.");
            return transform.Find("Fabric").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        public override void SetupMesh(ShapeModule shapeModule)
        {
            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            shapeModule.mesh = GetMesh();
            shapeModule.rotation = new Vector3(180, 0, 0);
            shapeModule.scale = transform.localScale * 0.0001f;
        }
        private IEnumerator WaitForAnimationToEnd(Animator anim)
        {
            yield return new WaitForSeconds(0.1f);
            anim.SetBool("Hit", false);
        }

        private IEnumerator Kill()
        {
            transform.Find("Fabric").transform.gameObject.SetActive(false);
            ParticleSystem p = transform.Find("DeathParticles").transform.GetComponent<ParticleSystem>();
            p.Play();

            yield return new WaitUntil(() => p.isStopped);

            transform.parent.parent.GetComponent<DummyMachine>().GetAnimator().SetBool("Show", false);
        }
    }
}