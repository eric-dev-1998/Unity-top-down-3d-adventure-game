using Assets.Scripts.Event_System;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Event_system.Events
{
    public class Multiline : Event_System.Event
    {
        public string author = "";
        public List<String> lines = new List<String>();

        public Multiline(string author, List<string> lines) 
        {
            this.author = author;
            this.lines = lines;
        }

        public override System.Collections.IEnumerator Process(Event_System.Manager eManager, Dialogue_System.Manager dManager)
        {
            if (!dManager.OnDialogue())
                yield return dManager.StartCoroutine(dManager.ShowDialogueBox());

            foreach (string line in lines)
            {
                dManager.StartCoroutine(dManager.WriteText(author, line, false, null, null));
                yield return new WaitUntil(() => dManager.advance == true);
            }

            if (next != null && next.Count != 0)
                yield return dManager.StartCoroutine(base.Process(eManager, dManager));
            else
                yield return dManager.StartCoroutine(dManager.HideDialogueBox());
        }
    }
}
