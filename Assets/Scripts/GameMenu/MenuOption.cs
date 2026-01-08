using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.GameMenu
{
    public class MenuOption : MonoBehaviour
    {
        int value = 0;
        private TextMeshProUGUI uiText;
        private Image highlight;

        private GameMenu menu;

        private void Start()
        {
            uiText = GetComponent<TextMeshProUGUI>();
            menu = FindAnyObjectByType<GameMenu>();
        }

        public void Load(int value, Image highlight)
        { 
            this.value = value;
            this.highlight = highlight;
        }

        public void OnPointerEnter()
        {
            if (highlight != null)
            {
                highlight.rectTransform.position = uiText.rectTransform.position;
                menu.currentMenuPosition = value;
                menu.usedMouse = true;
            }
        }
    }
}
