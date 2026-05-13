using UnityEngine;

namespace Assets.Scripts.World.Enemy
{
    public class Fungy : EnemyCore
    {
        private ParticleSystem pieces;
        private SkinnedMeshRenderer mesh;

        public override void Ready()
        {
            pieces = transform.Find("Pieces").GetComponent<ParticleSystem>();
            mesh = transform.Find("Fungy").GetComponent<SkinnedMeshRenderer>();
        }

        public override void Kill()
        {
            pieces.Play();
            mesh.enabled = false;
        }

        public override void Resurrect()
        {
            base.Resurrect();

            mesh.enabled = true;
            GetTriggerCollider().enabled = true;
            GetEntity().GetCharacterController().enabled = true;
        }

        public override void ReactToNeutral()
        {
            base.ReactToNeutral();

            GetComponent<Animator>().SetBool("Hit", true);
            GetComponent<Animator>().SetBool("Attack", false);

            transform.rotation = Quaternion.LookRotation(-GetPlayer().transform.forward, Vector3.up);

            Vector3 direction = Projectile.transform.position - transform.position;
            direction.Normalize();
            direction.y = 0;

            RecieveDamage(Projectile.power, -direction, 3);
        }

        public override void ReactToEarth()
        {
            base.ReactToEarth();

            Vector3 direction = Projectile.transform.position - transform.position;
            direction.Normalize();
            direction.y = 0;

            RecieveDamage(Projectile.power, -direction, 3f);
        }

        public override void ReactToFire()
        {
            base.ReactToFire();
            GetEntity().entityAnimator.animator.SetBool("Burn", true);

            Vector3 direction = GetPlayer().transform.position - transform.position;
            direction.Normalize();
            direction.y = 0;

            RecieveDamage(1, -direction, 3f);
        }

        public override void OnFireEnd()
        {
            GetEntity().entityAnimator.animator.SetBool("Burn", false);
            base.OnFireEnd();
        }

        public override void ReactToWind()
        {
            base.ReactToWind();
            GetEntity().entityAnimator.animator.SetBool("Blown", true);
            GetEntity().canMove = false;

            transform.rotation = Quaternion.LookRotation(-GetPlayer().transform.forward, Vector3.up);
        }

        public override void OnWindEnd()
        {
            base.OnWindEnd();
            GetEntity().entityAnimator.animator.SetBool("Blown", false);
            GetEntity().canMove = true;
        }
    }
}