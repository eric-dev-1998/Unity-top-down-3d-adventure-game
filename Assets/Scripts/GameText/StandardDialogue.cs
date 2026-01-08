using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameText
{
    [Serializable]
    public class StandardDialogue
    {
        public string mainGreeting;
        public Greetings greetings;
        public List<DialogueLine> standardDialogues;
    }
}
