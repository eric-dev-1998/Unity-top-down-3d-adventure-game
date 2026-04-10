using System.Collections;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Despawnable : MonoBehaviour
    {
        public enum When { OnAwake, OnNoMotion };
        public enum Style { None, ScaleDown, PlayParticles, Both };

        public When despawnWhen;
        public Style style;
        public ParticleSystem vfxParticles;
        public float secondsToDespawn = 5.0f;
        private float seconds = 0f;
        private bool scaleDown = false;

        // Use this for initialization
        void Start()
        {
            if (style == Style.PlayParticles && vfxParticles == null)
            {
                Debug.LogError($"[Despawnable][{name}]: Style it set to PlayParticles but no particle vfx was defined.");
                enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (style == Style.ScaleDown && scaleDown)
                transform.localScale -= Vector3.one * Time.deltaTime;

            if (despawnWhen == When.OnAwake)
            {
                seconds += Time.deltaTime;
                if (seconds >= secondsToDespawn)
                    StartCoroutine(Despawn());
            }
            else
            { 
                Rigidbody rigidbody = GetComponent<Rigidbody>();
                if (rigidbody == null)
                {
                    Debug.LogError($"[Despawable][{name}]: This despawnable was set to trigger on no motion, but a rigidbody is required.");
                    enabled = false;
                }

                if (rigidbody.linearVelocity.sqrMagnitude <= 0.15f)
                {
                    seconds += Time.deltaTime;
                    if (seconds >= secondsToDespawn)
                        StartCoroutine(Despawn());
                }
                else
                    seconds = 0f;
            }
        }

        IEnumerator Despawn()
        {
            switch (style)
            { 
                case Style.None: 
                    Destroy(gameObject);
                    yield break;

                case Style.ScaleDown:
                    scaleDown = true;
                    yield return new WaitUntil(() => transform.localScale.magnitude <= 0.05f);
                    Destroy(gameObject);
                    yield break;

                case Style.PlayParticles:
                    if (vfxParticles != null)
                    {
                        // Visually disappear.
                        GetComponent<MeshRenderer>().enabled = false;

                        // Play the particle vfx and wait for it to finish.
                        vfxParticles.Play();
                        yield return new WaitUntil(() => !vfxParticles.isStopped);
                        Destroy(gameObject);
                    }
                    yield break;

                case Style.Both:
                    // To be implemented if needed.
                    yield break;
            }
        }
    }
}