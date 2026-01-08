using Assets.Scripts.GameText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameMenu
{
    public class GameMenu : MonoBehaviour
    {
        public int currentMenuPosition = 4;
        public bool usedMouse = false;

        // UI elements:
        private Image highlight;
        private TextMeshProUGUI textResume;
        private TextMeshProUGUI textInventory;
        private TextMeshProUGUI textQuest;
        private TextMeshProUGUI textSave;
        private TextMeshProUGUI textExit;

        // Containers:
        public GameObject menuInventory;
        public GameObject menuQuest;

        private void Start()
        {
            highlight = transform.Find("Main/Highlight").GetComponent<Image>();

            textResume = transform.Find("Main/Resume").GetComponent<TextMeshProUGUI>();
            textResume.GetComponent<MenuOption>().Load(currentMenuPosition, highlight);

            textInventory = transform.Find("Main/Inventory").GetComponent<TextMeshProUGUI>();
            textInventory.GetComponent<MenuOption>().Load(currentMenuPosition, highlight);

            textQuest = transform.Find("Main/Quests").GetComponent<TextMeshProUGUI>();
            textQuest.GetComponent<MenuOption>().Load(currentMenuPosition, highlight);

            textSave = transform.Find("Main/Save game").GetComponent<TextMeshProUGUI>();
            textSave.GetComponent<MenuOption>().Load(currentMenuPosition, highlight);

            textExit = transform.Find("Main/Exit").GetComponent<TextMeshProUGUI>();
            textExit.GetComponent<MenuOption>().Load(currentMenuPosition, highlight);

            highlight.rectTransform.position = textResume.rectTransform.position;
        }

        private void Update()
        {
            Navigate();
        }

        private void Navigate()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                currentMenuPosition++;
                usedMouse = false;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                currentMenuPosition--;
                usedMouse = false;
            }

            if (!usedMouse)
            {
                if (currentMenuPosition < 0)
                    currentMenuPosition = 4;
                else if (currentMenuPosition > 4)
                    currentMenuPosition = 0;

                switch (currentMenuPosition)
                {
                    case 4:
                        highlight.rectTransform.position = textResume.rectTransform.position;
                        break;

                    case 3:
                        highlight.rectTransform.position = textInventory.rectTransform.position;
                        break;

                    case 2:
                        highlight.rectTransform.position = textQuest.rectTransform.position;
                        break;

                    case 1:
                        highlight.rectTransform.position = textSave.rectTransform.position;
                        break;

                    case 0:
                        highlight.rectTransform.position = textExit.rectTransform.position;
                        break;
                }
            }
        }
    }
}
