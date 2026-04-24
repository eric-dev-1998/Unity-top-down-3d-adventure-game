using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World;
using Assets.Scripts.GameSerialization;

namespace Assets.Scripts
{
    public class HUDManager : MonoBehaviour
    {
        private VisualElement _healthBar;
        private VisualElement _magicBar;
        private VisualElement _magicIcon;
        private VisualElement _powerOrbs;
        private VisualElement _magicCrystals;
        private VisualElement _spirits;
        private VisualElement _selectedCounter;

        private List<Texture2D> _magicIcons;
        private UIDocument _document;
        private PlayerCore _player;

        private bool _isCollectControlDisplayed = false;
        private float _collectDisplayVisibleTime = 4.0f;
        private float _collectDisplayTimer = 0f;

        private void Start()
        {
            if (!LoadMagicIcons())
                Debug.Log("Unable to load magic icons.");

            _document = GetComponent<UIDocument>();
            _player = FindAnyObjectByType<PlayerCore>();

            _healthBar = _document.rootVisualElement.Q<VisualElement>("PlayerHealth").Q<VisualElement>("Bar");
            _magicBar = _document.rootVisualElement.Q<VisualElement>("PlayerMana").Q<VisualElement>("Bar");
            _magicIcon = _document.rootVisualElement.Q<VisualElement>("MagicSelector").Q<VisualElement>("Icon");

            _powerOrbs = _document.rootVisualElement.Q<VisualElement>("Counter_PowerOrbs");
            _magicCrystals = _document.rootVisualElement.Q<VisualElement>("Counter_MagicCrystals");
            _spirits = _document.rootVisualElement.Q<VisualElement>("Counter_LostSpirits");
        }

        private void Update()
        {
            if (_isCollectControlDisplayed)
            { 
                _collectDisplayTimer += Time.deltaTime;
                if (_collectDisplayTimer >= _collectDisplayVisibleTime)
                {
                    _selectedCounter.AddToClassList("counter_hidden");
                    _isCollectControlDisplayed = false;
                }
            }

            int mouseScrollDelta = Mathf.CeilToInt(Input.mouseScrollDelta.y);
            if (mouseScrollDelta != 0)
                SelectMagic(mouseScrollDelta);
        }

        private bool LoadMagicIcons()
        {
            _magicIcons = Resources.LoadAll<Texture2D>("Art/Icons/").ToList();
            if(_magicIcons.Count != 0)
                return true;
            else 
                return false;   
        }

        public void SetHealth(int value)
        { 
            
        }

        public void SetMagic(int value) 
        {
            _magicBar.style.width = (int) (472f * (value / 100f));
        }

        public void DisplayCollected(Collectible.CollectibleType collectibleType)
        { 
            if(!_isCollectControlDisplayed)
            {
                int count = 0;
                _selectedCounter = null;

                switch (collectibleType)
                {
                    case Collectible.CollectibleType.Magic_Crystal:
                        _selectedCounter = _magicCrystals;
                        count = PlayerInventory.MagicCrystals;
                        break;

                    case Collectible.CollectibleType.Spirit:
                        _selectedCounter = _spirits;
                        count = PlayerInventory.Spirits;
                        break;

                    case Collectible.CollectibleType.Power_Orb:
                        _selectedCounter = _powerOrbs;
                        count = PlayerInventory.PowerOrbs;
                        break;
                }

                if (_selectedCounter != null)
                {
                    Label counter = _selectedCounter.Q<Label>("Count");
                    counter.text = count.ToString();

                    _selectedCounter.RemoveFromClassList("counter_hidden");
                    _isCollectControlDisplayed = true;
                }
            }
        }

        private void SelectMagic(int i)
        {
            Debug.Log(i);

            _player.SelectMagic(i);
            ChangeSelectedMagicIcon();
        }

        private void ChangeSelectedMagicIcon()
        {
            float index = _player.GetSelectedMagicIndex();
            if(index >= 0)
                _magicIcon.style.backgroundImage = _magicIcons.Find(icon => icon.name == _player.GetSelectedMagicName());
        }
    }
}
