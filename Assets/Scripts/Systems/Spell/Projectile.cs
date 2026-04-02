using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Systems.Spell
{
    public class Projectile : MonoBehaviour
    {
        public float speed = 3f;
        public float timeToDisappear = 5f;
        private float timeElapsed = 0f;

        private void Update()
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
            
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= timeToDisappear)
                DestroySelf();
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            DestroySelf();
        }
    }
}
