using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class VFXManager : MonoBehaviour
    {
        public enum VFX { Light_FadeIn, Light_FadeOut, Dark_FadeIn, Dark_FadeOut };

        private Animator dark;
        private new Animator light;

        private void Start()
        {
            dark = transform.Find("Dark").GetComponent<Animator>();
            light = transform.Find("Light").GetComponent<Animator>();

            PlayEffect(VFX.Dark_FadeOut);
        }

        public void PlayEffect(VFX vfx)
        {
            StartCoroutine(PlayVFX(vfx));
        }

        public IEnumerator PlayVFX(VFX vfx)
        {
            switch (vfx)
            {
                case VFX.Light_FadeIn:
                    yield return Light(true);
                    break;

                case VFX.Light_FadeOut:
                    yield return Light(false);
                    break;

                case VFX.Dark_FadeIn:
                    yield return Dark(true);
                    break;

                case VFX.Dark_FadeOut:
                    yield return Dark(false);
                    break;
            }
        }

        private IEnumerator Dark(bool fadeIn)
        {
            dark.SetBool("Show", true);
            dark.SetBool("In", fadeIn);

            yield return new WaitForSeconds(0.05f);
            yield return new WaitUntil(() => dark.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        }

        private IEnumerator Light(bool fadeIn)
        {
            light.SetBool("Show", true);
            light.SetBool("In", fadeIn);

            yield return new WaitForSeconds(0.05f);
            yield return new WaitUntil(() => light.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);        }
    }
}
