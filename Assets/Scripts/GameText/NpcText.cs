using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameText
{
    [Serializable]
    public class NpcText
    {
        public string npcId;
        public List<NpcQuestText> ownQuestLines;
        public List<NpcQuestText> mainQuestLines;
        public List<StandardDialogue> standardLines;
    }
}
