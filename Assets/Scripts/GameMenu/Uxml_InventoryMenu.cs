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
    public class Uxml_InventoryMenu : MonoBehaviour
    {
        public bool open = false;

        private UIDocument document;

        private VisualElement menuPanel;
        private Label title;
        private Button buttonClose;
        private ListView itemList;

        private Inventory_System.Manager manager;

        private void Start()
        {
            manager = FindAnyObjectByType<Inventory_System.Manager>();

            document = GetComponent<UIDocument>();
            menuPanel = document.rootVisualElement.Q<VisualElement>("Root");
            itemList = document.rootVisualElement.Q<ListView>("ItemList");
            title = document.rootVisualElement.Q<Label>("Title");

            buttonClose = document.rootVisualElement.Q<Button>("ButtonClose");
            buttonClose.clicked += CloseMenu;

            LoadText();
        }

        private void LoadText()
        {
            TextManager textManager = FindAnyObjectByType<TextManager>();
            if (textManager == null)
            {
                Debug.LogError("[Text manager]: No text manager was found on scene.");
                return;
            }

            title.text = textManager.GetUIText("inventory_menu_title");
            buttonClose.text = textManager.GetUIText("menu_back");

            itemList.makeNoneElement = () => 
            {
                Label label = new Label();
                label.text = textManager.GetUIText("inventory_menu_empty");
                label.style.fontSize = 32;
                label.style.unityFontStyleAndWeight = FontStyle.Normal;
                return label;
            };
        }

        public void OpenMenu()
        {
            open = true;
            RefreshInventory();
            menuPanel.RemoveFromClassList("panel_full_hidden");
        }

        public void CloseMenu()
        {
            open = false;
            menuPanel.AddToClassList("panel_full_hidden");
        }

        public void RefreshInventory()
        {
            itemList.itemsSource = manager.GetInventory();
            itemList.fixedItemHeight = 48;

            itemList.makeItem = () => 
            {
                VisualElement root = new VisualElement();
                root.style.flexDirection = FlexDirection.Row;
                root.style.justifyContent = Justify.SpaceBetween;

                Label itemName = new Label();
                itemName.name = "Name";
                itemName.style.fontSize = 32;

                Label itemCount = new Label();
                itemCount.name = "Count";
                itemCount.style.fontSize = 32;

                root.Add(itemName);
                root.Add(itemCount);

                return root;
            };

            itemList.bindItem = (e, index) => 
            {
                Label itemName = e.Q<Label>("Name");
                Label itemCount = e.Q<Label>("Count");

                Inventory_System.InventorySpace space;
                space = manager.GetInventory()[index];

                itemName.text = space.data.name;
                itemCount.text = "x" + space.count.ToString();
            };

            itemList.Rebuild();
        }
    }
}
