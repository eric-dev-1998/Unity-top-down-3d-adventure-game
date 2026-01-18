using Assets.Scripts.Event_system.Events;
using System.Collections.Generic;

namespace Assets.Scripts.Quest_System
{
    [System.Serializable]
    public class QuestRequirement
    {
        public string questId;
        public QuestSet.QuestState state;

        public bool Matched(List<Quest> quests)
        {
            // Just return true if this quest requirement is set to none:
            if (state == QuestSet.QuestState.None)
                return true;

            foreach (Quest q in quests)
            {
                // Validate that 'quest' state matches this requirement state.
                // Validate:

                if (q.Completed() && state == QuestSet.QuestState.Completed)
                    return true;

                if (!q.Completed() && state == QuestSet.QuestState.Active)
                    return true;
            }

            return false;
        }
    }
}
