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
            if (quest.Completed() && state == QuestSet.QuestState.Completed)
                return true;

            if (!quest.Completed() && state == QuestSet.QuestState.Active)
                return true;

            return false;
        }
    }
}
