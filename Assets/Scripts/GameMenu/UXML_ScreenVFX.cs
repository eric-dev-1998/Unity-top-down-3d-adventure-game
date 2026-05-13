using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.GameMenu
{
    public class UXML_ScreenVFX : MonoBehaviour
    {
        public enum AwakeFadeAnimation { DarkOut, LightOut, None }
        public AwakeFadeAnimation awakeFadeAnimation;

        private VisualElement _veil;

        private UIDocument _document;

        void Start()
        {
            _document = GetComponent<UIDocument>();
            _veil = _document.rootVisualElement.Q("Veil");

            StartCoroutine(PlayAwakeFadeAnimation());
        }

        public IEnumerator PlayAwakeFadeAnimation()
        {
            var originalDuration = _veil.style.transitionDuration;

            switch (awakeFadeAnimation)
            {
                case AwakeFadeAnimation.DarkOut:
                    _veil.style.transitionDuration = new List<TimeValue> { new TimeValue(0, TimeUnit.Second) };
                    FadeToDark();

                    yield return null;

                    _veil.style.transitionDuration = originalDuration;
                    Clear();

                    yield break;

                case AwakeFadeAnimation.LightOut:
                    _veil.style.transitionDuration = new List<TimeValue> { new TimeValue(0, TimeUnit.Second) };
                    FadeToLight();

                    yield return null;

                    _veil.style.transitionDuration = originalDuration;
                    Clear();
                    yield break;

                default:
                    yield break;
            }
        }

        public void FadeToDark() 
        {
            _veil.RemoveFromClassList("screen-fade-light");
            _veil.AddToClassList("screen-fade-dark");
        }

        public void FadeToLight() 
        {
            _veil.RemoveFromClassList("screen-fade-dark");
            _veil.AddToClassList("screen-fade-light");
        }

        public void Clear() 
        {
            _veil.RemoveFromClassList("screen-fade-light");
            _veil.RemoveFromClassList("screen-fade-dark");
        }

        public VisualElement GetScreenVeil()
        {
            return _veil;
        }
    }
}