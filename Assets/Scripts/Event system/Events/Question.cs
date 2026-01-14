using Assets.Scripts.Event_System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Event_system.Events
{
    public class Question : Event_System.Event
    {
        public string question = "";

        public Question(string question)
        { 
            this.question = question;
        }

        public override IEnumerator Process(Event_System.EventManager eManager, Dialogue_System.Manager dManager)
        {
            if (!dManager.OnDialogue())
                yield return dManager.StartCoroutine(dManager.ShowDialogueBox());

            GameText.DialogueLine line = dManager.textManager.GetDialogueLine(question);

            yield return dManager.StartCoroutine(dManager.WriteText(line.id.Split('_')[0], line.content, true, line.optionA, line.optionB));
            yield return new WaitUntil(() => dManager.chooseA == true || dManager.chooseB == true);

            yield return new WaitForSeconds(0.26f);
            dManager.HideAnswers();

            if (dManager.chooseA)
                yield return dManager.StartCoroutine(next[0].Process(eManager, dManager));
            if(dManager.chooseB)
                yield return dManager.StartCoroutine(next[1].Process(eManager, dManager));
        }
    }
}
