using Assets.Scripts.Event_system.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameText
{
    [Serializable]
    public class QuestSentitiveText
    {
        public string questId;
        public QuestSet.QuestState state;

        [TextArea]
        public string[] lines;
    }
}
