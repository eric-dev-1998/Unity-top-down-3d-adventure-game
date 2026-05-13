using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Scripts.GameMenu
{
    public class Uxml_GameOver : MonoBehaviour
    {
        private Label _text;
        private Button _button;
        private VisualElement _background;

        private UIDocument _document;

        private void Start()
        {
            _document = GetComponent<UIDocument>();

            _text = _document.rootVisualElement.Q<Label>("Text");
            _background = _document.rootVisualElement.Q("Background");

            _button = _document.rootVisualElement.Q<Button>("Button_Retry");
            _button.clicked += Retry;
        }

        public void Show()
        {
            // 1. Show darker background.
            _background.AddToClassList("t-darken");

            // 2. Show text and buttons.
            _text.AddToClassList("t-vslide-to-center");
            _button.AddToClassList("t-vslide-to-center");
        }

        private void Retry()
        {
            _button.SetEnabled(false);
            StartCoroutine(ReloadScene());
        }

        private IEnumerator ReloadScene()
        {
            // 1. Darken screen:
            var screenvfx = FindAnyObjectByType<UXML_ScreenVFX>();
            screenvfx.FadeToDark();

            StyleColor darkColor = new StyleColor();
            darkColor.value = new Color(0, 0, 0, 1);

            yield return new WaitUntil(() => screenvfx.GetScreenVeil().resolvedStyle.backgroundColor == darkColor);

            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Demo");
        }
    }
}