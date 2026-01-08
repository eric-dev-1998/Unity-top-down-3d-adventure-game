using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameText
{
    [Serializable]
    public class NpcQuestText
    {
        public string questId;
        public DialogueLine available;
        public DialogueLine active;
        public DialogueLine complete;
    }
}
