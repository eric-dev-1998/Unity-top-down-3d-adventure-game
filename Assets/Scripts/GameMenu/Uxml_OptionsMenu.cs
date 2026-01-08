using Assets.Scripts.GameText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.GameMenu
{
    public class Uxml_OptionsMenu : MonoBehaviour
    {
        public bool open = false;

        private UIDocument document;
        private VisualElement menuPanel;

        private Label title;
        private Button buttonClose;

        private void Start()
        {
            document = GetComponent<UIDocument>();
            menuPanel = document.rootVisualElement.Q<VisualElement>("Root");

            title = menuPanel.Q<Label>("Title");

            buttonClose = document.rootVisualElement.Q<Button>("ButtonClose");
            buttonClose.clicked += CloseMenu;

            LoadText();
        }

        public void LoadText()
        { 
            TextManager textManager = FindAnyObjectByType<TextManager>();
            if (textManager == null)
            {
                Debug.LogError("[Text manager]: No text manager was found on scene.");
                return;
            }

            title.text = textManager.GetUIText("settings_menu_title");
            buttonClose.text = textManager.GetUIText("menu_back");
        }

        public void OpenMenu()
        {
            open = true;
            menuPanel.RemoveFromClassList("panel_full_hidden");
        }

        public void CloseMenu()
        {
            open = false;
            menuPanel.AddToClassList("panel_full_hidden");
        }
    }
}
