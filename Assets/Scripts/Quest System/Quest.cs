using System;
using UnityEngine;

namespace Assets.Scripts.Quest_System
{
    [CreateAssetMenu(fileName = "New game quest", menuName = "Eric/Game quest")]
    public class Quest : ScriptableObject
    {
        public enum QuestType { MainQuest, SideQuest };

        public string id;
        public string from;

        public QuestType type;

        public QuestRequirement requirement;

        public Objective[] objectives;

        public bool Completed()
        {
            foreach (Objective o in objectives)
            {
                if (!o.completed)
                    return false;
            }

            return true;
        }

        public string GetId()
        {
            return id;
        }
    }
}
