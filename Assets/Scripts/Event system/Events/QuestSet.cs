using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Event_system.Events
{
    public class QuestSet : Event_System.Event
    {
        public string questName = "";
        public enum QuestState { None, Active, Completed };
        public QuestState questState;

        public QuestSet(string questName, QuestState questState) 
        {
            this.questName = questName;
            this.questState = questState;
        }

        public override IEnumerator Process(Event_System.EventManager eManager, Dialogue_System.Manager dManager)
        {
            Quest_System.Quest quest = Resources.Load<Quest_System.Quest>($"Quests/{questName}");

            if (questState == QuestState.Active)
                eManager.questManager.TriggerQuest(quest);
            else if (questState == QuestState.Completed)
                eManager.questManager.SetComplete(quest);

            if (next.Count != 0 && next[0] != null)
                yield return next[0].Process(eManager, dManager);

        }
    }
}
