using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameText
{
    [CreateAssetMenu(fileName = "New text library", menuName = "Eric/Text library")]
    public class TextLibrary : ScriptableObject
    {
        public List<PlainText> uiText;
        public List<QuestText> questText;
        public List<ItemText> itemText;
        public List<DialogueLine> dialogueText;
        public List<PlainText> worldText;
    }
}
