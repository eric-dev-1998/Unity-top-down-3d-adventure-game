
using System;
using UnityEngine;

namespace Assets.Scripts.Quest_System
{
    [Serializable]
    public class Objective
    {
        public enum ObjectiveType { GetItem, ReachArea, TalkToNPC, Interact };

        public ObjectiveType type;
        [TextArea]
        public string description = string.Empty;
        public string targetId = string.Empty;
        public int maxCount = 0;
        public bool completed = false;

        public Objective() { }

        public Objective(ObjectiveType type, string description, int maxCount)
        {
            this.type = type;
            this.description = description;
            this.maxCount = maxCount;
        }

        public override string ToString()
        {
            return $"{description}\n";
        }
    }
}
