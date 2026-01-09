using Assets.Scripts.Event_system.Events;

namespace Assets.Scripts.Quest_System
{
    [System.Serializable]
    public class QuestRequirement
    {
        public string questId;
        public QuestSet.QuestState state;

        public bool Matched(Quest quest)
        {
            // Validate that 'quest' state matches this requirement state.

            // Just return true if this requirement is set to none:
            if (state == QuestSet.QuestState.None)
                return true;

            // Validate:

            if (quest.Completed() && state == QuestSet.QuestState.Completed)
                return true;

            if (!quest.Completed() && state == QuestSet.QuestState.Active)
                return true;

            return false;
        }
    }
}
