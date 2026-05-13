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

        private Collectible.CollectibleType _currentDisplayType;
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
            // Counter display timer.
            if (_isCollectControlDisplayed)
            {
                // Update values:
                _powerOrbs.Q<Label>("Count").text = PlayerData.PowerOrbs.ToString();
                _magicCrystals.Q<Label>("Count").text = PlayerData.MagicCrystals.ToString();
                _spirits.Q<Label>("Count").text = PlayerData.Spirits.ToString();

                _collectDisplayTimer += Time.deltaTime;
                if (_collectDisplayTimer >= _collectDisplayVisibleTime)
                {
                    _selectedCounter.AddToClassList("counter-hidden");

                    _isCollectControlDisplayed = false;
                    _collectDisplayTimer = 0;
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

        // Magic bar is not working properly.

        public void SetHealth(float value, float max)
        {
            _healthBar.style.width = (int)(472f * (value / max));
        }

        public void SetMagic(float value, float max) 
        {
            _magicBar.style.width = Mathf.CeilToInt(472f * (value / max));
        }

        public void DisplayCollected(Collectible.CollectibleType collectibleType)
        { 
            int count = 0;

            switch (collectibleType)
            {
                case Collectible.CollectibleType.Magic_Crystal:
                    if (_isCollectControlDisplayed)
                    {
                        if (_currentDisplayType == Collectible.CollectibleType.Magic_Crystal)
                        {
                            _collectDisplayTimer = 0f;
                            return;
                        }
                        else
                        {
                            _selectedCounter.AddToClassList("counter-hidden");
                        }
                    }

                    _currentDisplayType = Collectible.CollectibleType.Magic_Crystal;
                    _selectedCounter = _magicCrystals;
                    count = PlayerData.MagicCrystals;
                    break;

                case Collectible.CollectibleType.Spirit:
                    if (_isCollectControlDisplayed)
                    {
                        if (_currentDisplayType == Collectible.CollectibleType.Spirit)
                        {
                            _collectDisplayTimer = 0f;
                            return;
                        }
                        else
                        {
                            _selectedCounter.AddToClassList("counter-hidden");
                        }
                    }

                    _currentDisplayType = Collectible.CollectibleType.Spirit;
                    _selectedCounter = _spirits;
                    count = PlayerData.Spirits;
                    break;

                case Collectible.CollectibleType.Power_Orb:
                    if (_isCollectControlDisplayed)
                    {
                        if (_currentDisplayType == Collectible.CollectibleType.Power_Orb)
                        {
                            _collectDisplayTimer = 0f;
                            return;
                        }
                        else
                        {
                            _selectedCounter.AddToClassList("counter-hidden");
                        }
                    }

                    _currentDisplayType = Collectible.CollectibleType.Power_Orb;
                    _selectedCounter = _powerOrbs;
                    count = PlayerData.PowerOrbs;
                    break;
            }

            if (_selectedCounter != null)
            {
                _selectedCounter.RemoveFromClassList("counter-hidden");
                _isCollectControlDisplayed = true;
            }
        }

        private void SelectMagic(int i)
        {
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
