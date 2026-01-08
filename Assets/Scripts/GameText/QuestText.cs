using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameText
{
    [Serializable]
    public class QuestText
    {
        public string id;
        public string title;
        [TextArea]
        public string description;
        [TextArea]
        public string[] objectives;
    }
}
