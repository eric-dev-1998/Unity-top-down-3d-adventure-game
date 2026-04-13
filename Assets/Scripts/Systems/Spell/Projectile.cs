using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Systems.Spell
{
    public class Projectile : MonoBehaviour
    {
        public int power = 1;
        public float speed = 3f;
        public float timeToDisappear = 5f;
        private float timeElapsed = 0f;
        private SpellCaster caster;

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

        public void SetCaster(SpellCaster caster)
        { 
            this.caster = caster;
        }

        public SpellCaster GetCaster()
        { 
            return caster;
        }
        private void OnCollisionEnter(Collision collision)
        {
            DestroySelf();
        }
    }
}
