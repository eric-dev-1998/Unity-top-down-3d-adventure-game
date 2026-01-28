using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Water : MonoBehaviour
    {
        GameObject rippleB;
        ParticleSystem rippleBvfx;
        public Entity playerEntity;

        private void Start()
        {
            rippleB = transform.Find("Ripple_B").gameObject;
            rippleBvfx = rippleB.GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (playerEntity != null)
            { 
                rippleB.transform.position = new Vector3(
                    playerEntity.transform.position.x,
                    transform.position.y,
                    playerEntity.transform.position.z);

                if (playerEntity.onDeepWater)
                {
                    if(!rippleBvfx.isPlaying)
                        rippleB.GetComponent<ParticleSystem>().Play();
                }
                else
                    if(rippleBvfx.isPlaying)
                        rippleB.GetComponent<ParticleSystem>().Stop();
            }
        }
    }
}
