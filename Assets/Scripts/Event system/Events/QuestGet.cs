using Assets.Scripts.Event_System;
using Assets.Scripts.Quest_System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Event_system.Events
{
    public class QuestGet : Event
    {
        public string questName;
        public QuestSet.QuestState questState;

        public QuestGet() { }

        public override IEnumerator Process(Event_System.EventManager eManager, Dialogue_System.Manager dManager)
        {
            /*
                1. Check if target quest does exsist in current player's quest list.
                2. Get quest state calling quest.Completed().
                3. Compare to state target
                4. Advance based on result.
             */

            Quest quest = eManager.questManager.GetCurrentQuests().Find(q => q.name == questName);
            if (quest == null)
            {
                UnityEngine.Debug.LogError("[Event manager]: Target quest could not be found on player's quest list, it may not be active or doesnt exsist at all." +
                    $" Target quest name: '{questName}'");
                yield return next[1].Process(eManager, dManager);
                yield break;
            }    

            bool targetResult = false;
            if (questState == QuestSet.QuestState.Active)
                targetResult = false;
            else if (questState == QuestSet.QuestState.Completed)
                targetResult = true;
            else
            {
                UnityEngine.Debug.LogError($"[Event manager]: Target quest state result is not valid, value must be either 'Active' or 'Completed'. " +
                    $"Current value is: '{questState.ToString()}'.");
                yield break;
            }

            if (targetResult == quest.Completed())
                yield return next[0].Process(eManager, dManager);
            else
                yield return next[1].Process(eManager, dManager);
        }
    }
}
