using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Event_System;
using Assets.Scripts.GameSerialization;
using Assets.Scripts.GameText;
using EventSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.GameMenu
{
    public class Uxml_InventoryMenu : MonoBehaviour
    {
        public bool IsOpen = false;

        private UIDocument _document;

        private VisualElement _menuPanel;
        private Label _title;
        private Label _collectibles;
        private Label _powerOrbCounter;
        private Label _magicCrystalCounter;
        private Label _lostSpiritCounter;
        private Button _buttonClose;
        private ListView _itemList;

        private UnityEngine.Camera _displayCamera;
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

            _powerOrbCounter = _document.rootVisualElement.Q<VisualElement>("PowerOrbs").Q<Label>("Count");
            _magicCrystalCounter = _document.rootVisualElement.Q<VisualElement>("MagicCrystals").Q<Label>("Count");
            _lostSpiritCounter = _document.rootVisualElement.Q<VisualElement>("LostSpirits").Q<Label>("Count");

            _title = _document.rootVisualElement.Q<Label>("Title");
            _collectibles = _document.rootVisualElement.Q<Label>("Collectibles");

            _displayCamera = transform.Find("DisplayCamera").GetComponent<UnityEngine.Camera>();
            _displayContainer = transform.Find("DisplayContainer").gameObject;

            _buttonClose = _document.rootVisualElement.Q<Button>("ButtonClose");
            _buttonClose.clicked += Close;

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

                // Disable the list to avoid selection more items while the info is beign displayed.
                _itemList.SetEnabled(false);

                string itemName = itemText.name;
                List<string> itemDescription = itemText.description.Split("\n").ToList();

                // Build the dialogue sequence:
                EventSequence dialogueSequence = new EventSequence();
                Multiline multilineEvent = new Multiline(itemDescription, itemName, Event_System.Events.SingleLine.Type.Item);
                dialogueSequence.startEvent = multilineEvent;

                // Trigger event:
                EventManager eventManager = FindAnyObjectByType<EventManager>();
                if (eventManager)
                {
                    eventManager.OnEventFinished += () => { _itemList.SetEnabled(true); };
                    eventManager.StartSequence(dialogueSequence, true);
                }
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
            _collectibles.text = textManager.GetUIText("inventory_collectibles");
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

        public void Open()
        {
            IsOpen = true;
            RefreshInventory();
            RefreshCounters();
            _menuPanel.RemoveFromClassList("panel_full_hidden");
        }

        public void Close()
        {
            IsOpen = false;
            _menuPanel.AddToClassList("panel_full_hidden");
        }

        public void RefreshCounters()
        {
            _powerOrbCounter.text = PlayerData.PowerOrbs.ToString();
            _magicCrystalCounter.text = PlayerData.MagicCrystals.ToString();
            _lostSpiritCounter.text = PlayerData.Spirits.ToString();
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
