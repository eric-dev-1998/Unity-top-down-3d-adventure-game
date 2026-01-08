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
        public string author = "";
        public string question = "";
        public string optionA = "";
        public string optionB = "";

        public Question(string author, string question, string optionA, string optionB)
        { 
            this.author = author;
            this.question = question;
            this.optionA = optionA;
            this.optionB = optionB;
        }

        public override IEnumerator Process(Event_System.Manager eManager, Dialogue_System.Manager dManager)
        {
            if (!dManager.OnDialogue())
                yield return dManager.StartCoroutine(dManager.ShowDialogueBox());

            yield return dManager.StartCoroutine(dManager.WriteText(author, question, true, optionA, optionB));
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
