using System;
using UnityEngine;

namespace Assets.Scripts.GameText
{
    [Serializable]
    public class DialogueLine
    {
        public string id;

        [TextArea]
        public string content;
        public string optionA;
        public string optionB;
    }
}
