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
        public bool Open = false;

        private UIDocument _document;

        private VisualElement _menuPanel;
        private Label _title;
        private Label _itemName;
        private Label _itemDescription;
        private Button _buttonClose;
        private ListView _itemList;

        private Camera _displayCamera;
        private GameObject _displayContainer;
        private GameObject _currentDisplay;

        private Inventory_System.InventoryManager _inventoryManager;

        private void Start()
        {
            _inventoryManager = FindAnyObjectByType<Inventory_System.InventoryManager>();

            _document = GetComponent<UIDocument>();
            _menuPanel = _document.rootVisualElement.Q<VisualElement>("Root");
            _itemList = _document.rootVisualElement.Q<ListView>("ItemList");
            _itemList.selectionChanged += ItemSelectionChanged;

            _title = _document.rootVisualElement.Q<Label>("Title");

            _itemName = _document.rootVisualElement.Q<Label>("ItemName");
            _itemDescription = _document.rootVisualElement.Q<Label>("ItemDescription");
            _displayCamera = transform.Find("DisplayCamera").GetComponent<Camera>();
            _displayContainer = transform.Find("DisplayContainer").gameObject;

            _buttonClose = _document.rootVisualElement.Q<Button>("ButtonClose");
            _buttonClose.clicked += CloseMenu;

            LoadText();
        }

        private void ItemSelectionChanged(IEnumerable<object> obj)
        {
            // Display the selected item name and description.
            
            Inventory_System.InventorySpace inventorySpace = obj.First() as Inventory_System.InventorySpace; 

            if (inventorySpace != null)
            {
                TextManager textManager = FindAnyObjectByType<TextManager>();
                ItemText itemText = textManager.GetItem(inventorySpace.data.item_id);

                _itemName.text = itemText.name;
                _itemDescription.text = itemText.description;
            }

            // Pending: Display the item preview.
            if (_currentDisplay != null)
                Destroy(_currentDisplay);

            _currentDisplay = Instantiate(inventorySpace.data.item_display, _displayContainer.transform);
        }

        private void LoadText()
        {
            TextManager textManager = FindAnyObjectByType<TextManager>();
            if (textManager == null)
            {
                Debug.LogError("[Text manager]: No text manager was found on scene.");
                return;
            }

            _title.text = textManager.GetUIText("inventory_menu_title");
            _buttonClose.text = textManager.GetUIText("menu_back");

            _itemList.makeNoneElement = () => 
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
            Open = true;
            RefreshInventory();
            _menuPanel.RemoveFromClassList("panel_full_hidden");
        }

        public void CloseMenu()
        {
            Open = false;
            _menuPanel.AddToClassList("panel_full_hidden");
        }

        private void SelectItem()
        { 
            
        }

        public void RefreshInventory()
        {
            _itemList.itemsSource = _inventoryManager.GetInventory();
            _itemList.fixedItemHeight = 48;

            _itemList.makeItem = () => 
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

            _itemList.bindItem = (e, index) => 
            {
                Label itemName = e.Q<Label>("Name");
                Label itemCount = e.Q<Label>("Count");

                Inventory_System.InventorySpace space;
                space = _inventoryManager.GetInventory()[index];

                itemName.text = space.data.name;
                itemCount.text = "x" + space.count.ToString();
            };

            _itemList.Rebuild();
        }
    }
}
