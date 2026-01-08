using Assets.Art.UI.UXML;
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
    public class Uxml_GameMenu : MonoBehaviour
    {
        public bool open = false;

        private Uxml_InventoryMenu inventory;
        private Uxml_QuestMenu quests;
        private Uxml_OptionsMenu options;

        private UIDocument document;

        private Label title;
        private VisualElement content;
        private VisualElement menuPanel;
        private Button buttonResume;
        private Button buttonInventory;
        private Button buttonQuests;
        private Button buttonOptions;
        private Button buttonSave;
        private Button buttonExit;

        private void Start()
        {
            inventory = FindAnyObjectByType<Uxml_InventoryMenu>();
            quests = FindAnyObjectByType<Uxml_QuestMenu>();
            options = FindAnyObjectByType<Uxml_OptionsMenu>();

            document = GetComponent<UIDocument>();
            content = document.rootVisualElement.Q<VisualElement>("Content");
            menuPanel = document.rootVisualElement.Q<VisualElement>("Panel");

            title = document.rootVisualElement.Q<Label>("Title");

            buttonResume = document.rootVisualElement.Q<Button>("buttonResume");
            buttonInventory = document.rootVisualElement.Q<Button>("buttonInventory");
            buttonQuests = document.rootVisualElement.Q<Button>("buttonQuests");
            buttonOptions = document.rootVisualElement.Q<Button>("buttonOptions");
            buttonSave = document.rootVisualElement.Q<Button>("buttonSave");
            buttonExit = document.rootVisualElement.Q<Button>("buttonExit");

            buttonResume.clicked += CloseMenu;
            buttonInventory.clicked += OpenInventoryMenu;
            buttonQuests.clicked += OpenQuestsMenu;
            buttonOptions.clicked += OpenOptionsMenu;

            LoadText();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (inventory.open)
                {
                    inventory.CloseMenu();
                    return;
                }

                if (quests.open)
                {
                    quests.CloseMenu();
                    return;
                }

                if (options.open)
                {
                    options.CloseMenu();
                    return;
                }

                open = !open;
            }

            if (open)
                OpenMenu();
            else
                CloseMenu();
        }

        private void LoadText()
        {
            TextManager tManager = FindAnyObjectByType<TextManager>();
            if (tManager == null)
            {
                Debug.LogError("[Text Manager]: No text manager was found on scene.");
                return;
            }

            title.text = tManager.GetUIText("pause_menu_title");
            buttonResume.text = tManager.GetUIText("pause_menu_resume");
            buttonInventory.text = tManager.GetUIText("pause_menu_inventory");
            buttonQuests.text = tManager.GetUIText("pause_menu_quests");
            buttonOptions.text = tManager.GetUIText("pause_menu_settings");
            buttonSave.text = tManager.GetUIText("pause_menu_save");
            buttonExit.text = tManager.GetUIText("pause_menu_exit");
        }

        private void OpenMenu()
        {
            open = true;
            content.AddToClassList("menu-darker");
            menuPanel.RemoveFromClassList("menu_panel_hidden");
        }

        private void CloseMenu()
        {
            open = false;
            content.RemoveFromClassList("menu-darker");
            menuPanel.AddToClassList("menu_panel_hidden");
        }

        private void OpenInventoryMenu()
        {
            if (inventory != null)
                inventory.OpenMenu();
        }

        private void OpenQuestsMenu()
        {
            if(quests != null)
                quests.OpenMenu();
        }

        private void OpenOptionsMenu()
        { 
            if(options != null)
                options.OpenMenu();
        }
    }
}
