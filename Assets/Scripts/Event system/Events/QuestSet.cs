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

        public override IEnumerator Process(Event_System.Manager eManager, Dialogue_System.Manager dManager)
        {
            Quest_System.Quest quest = Resources.Load<Quest_System.Quest>($"Quests/{questName}");

            switch (questState)
            {
                case QuestState.None:
                    yield break;

                case QuestState.Active:
                    eManager.questManager.TriggerQuest(quest);
                    yield break;

                case QuestState.Completed:
                    eManager.questManager.SetComplete(quest);
                    yield break;
            }

            yield return base.Process(eManager, dManager);
        }
    }
}
