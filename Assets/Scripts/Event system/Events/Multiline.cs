using Assets.Scripts.Event_System;
using Assets.Scripts.Event_System.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Event_system.Events
{
    public class Multiline : Event_System.Event
    {
        public List<String> lines = new List<String>();
        public SingleLine.Type type = SingleLine.Type.Dialogue;

        public Multiline(List<string> lines, SingleLine.Type type) 
        {
            this.lines = lines;
            this.type = type;
        }

        public override System.Collections.IEnumerator Process(Event_System.EventManager eManager, Dialogue_System.Manager dManager)
        {
            if (!dManager.OnDialogue())
                yield return dManager.StartCoroutine(dManager.ShowDialogueBox());

            foreach (string line in lines)
            {
                string text = "";
                string author = "";

                if (type == SingleLine.Type.Dialogue)
                {
                    GameText.DialogueLine dLine = dManager.textManager.GetDialogueLine(line);
                    text = dLine.content;
                    author = dLine.id.Split('_')[0];
                }
                else if (type == SingleLine.Type.World)
                { 
                    text = dManager.textManager.GetWorldText(line);
                }

                dManager.StartCoroutine(dManager.WriteText(author, text, false, null, null));
                yield return new WaitUntil(() => dManager.advance == true);
            }

            if (next != null && next.Count != 0)
                yield return dManager.StartCoroutine(base.Process(eManager, dManager));
            else
                yield return dManager.StartCoroutine(dManager.HideDialogueBox());
        }
    }
}
