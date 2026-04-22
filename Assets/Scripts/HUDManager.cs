using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class HUDManager : MonoBehaviour
    {
        private VisualElement healthBar;
        private VisualElement magicBar;
        private VisualElement magicIcon;

        private List<Texture2D> magicIcons;

        private UIDocument document;

        private PlayerCore _player;

        private void Start()
        {
            if (!LoadMagicIcons())
                Debug.Log("Unable to load magic icons.");

            document = GetComponent<UIDocument>();
            _player = FindAnyObjectByType<PlayerCore>();

            healthBar = document.rootVisualElement.Q<VisualElement>("PlayerHealth").Q<VisualElement>("Bar");
            magicBar = document.rootVisualElement.Q<VisualElement>("PlayerMana").Q<VisualElement>("Bar");
            magicIcon = document.rootVisualElement.Q<VisualElement>("MagicSelector").Q<VisualElement>("Icon");
        }

        private void Update()
        {
            int mouseScrollDelta = Mathf.CeilToInt(Input.mouseScrollDelta.y);
            if (mouseScrollDelta != 0)
                SelectMagic(mouseScrollDelta);
        }

        private bool LoadMagicIcons()
        {
            magicIcons = Resources.LoadAll<Texture2D>("Art/Icons/").ToList();
            if(magicIcons.Count != 0)
                return true;
            else 
                return false;   
        }

        public void SetHealth(int value)
        { 
            
        }

        public void SetMagic(int value) 
        {
            magicBar.style.width = (int) (472f * (value / 100f));
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
                magicIcon.style.backgroundImage = magicIcons.Find(icon => icon.name == _player.GetSelectedMagicName());
        }
    }
}
