using Assets.Scripts.Event_system.Events;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameText
{
    public class TextManager : MonoBehaviour
    {
        private TextLibrary currentLibrary;

        private void Start()
        {
            LoadLibrary();
        }

        public void LoadLibrary()
        {
            currentLibrary = Resources.Load<TextLibrary>("GameText/English");
            if (currentLibrary == null)
                Debug.LogError("[Text manager]: Text library asset could not be loaded.");
        }

        public string GetUIText(string id)
        {
            if (currentLibrary == null)
            {
                Debug.LogError("[Text manager]: There is no text library loaded.");
                return "";
            }

            PlainText text = currentLibrary.uiText.Find(t => t.id == id);
            if (text == null)
            {
                Debug.LogError($"[Text manager]: No ui text with id: '{id}' was found in current library.");
                return "";
            }

            return text.content;
        }

        public string GetWorldText(string id)
        {
            if (currentLibrary == null)
            {
                Debug.LogError("[Text manager]: There is no text library loaded.");
                return "";
            }

            PlainText text = currentLibrary.worldText.Find(t => t.id == id);
            if (text == null)
            {
                Debug.LogError($"[Text manager]: No world text with id: '{id}' was found in current library.");
                return "";
            }

            return text.content;
        }

        public ItemText GetItem(string id)
        {
            if (currentLibrary == null)
            {
                Debug.LogError("[Text manager]: There is no text library loaded.");
                return null;
            }

            ItemText itemText = currentLibrary.itemText.Find(i => i.id == id);
            if (itemText == null)
            {
                Debug.LogError($"[Text manager]: No item text with id: '{id}' was found in current library.");
                return null;
            }

            return itemText;
        }

        public QuestText GetQuest(string id)
        {
            if (currentLibrary == null)
            {
                Debug.LogError("[Text manager]: There is no text library loaded.");
                return null;
            }

            QuestText questText = currentLibrary.questText.Find(i => i.id == id);
            if (questText == null)
            {
                Debug.LogError($"[Text manager]: No quest text with id: '{id}' was found in current library.");
                return null;
            }

            return questText;
        }

        public DialogueLine GetDialogueLine(string id)
        {
            DialogueLine line = currentLibrary.dialogueText.Find(l => l.id == id);
            if (line == null)
            {
                Debug.LogError($"[Text manager]: No dialogue line with id: '{id}' was found in current library.");
                return null;
            }

            return line;
        }
    }
}
