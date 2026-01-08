using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameText
{
    [Serializable]
    public class DialogueText
    {
        public List<DialogueLine> dialogueLines;
        public List<NpcText> npcText;
    }
}
