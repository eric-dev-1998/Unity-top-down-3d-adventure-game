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
        public List<String> altLines = new List<String>();
        public SingleLine.Type type = SingleLine.Type.Dialogue;

        private string _author;

        public Multiline(List<string> lines, SingleLine.Type type) 
        {
            this.lines = lines;
            this.type = type;
        }

        public Multiline(List<string> lines, string author, SingleLine.Type type)
        {
            this.lines = lines;
            this.type = type;
            _author = author;
        }

        public override System.Collections.IEnumerator Process(Event_System.EventManager eManager, Dialogue_System.Manager dManager)
        {
            UnityEngine.Debug.Log("Multiline event.");

            if (!dManager.OnDialogue())
                yield return dManager.StartCoroutine(dManager.ShowDialogueBox());

            if (lines != null)
            {
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
                    else
                    {
                        author = _author;
                        text = line;
                    }

                    dManager.StartCoroutine(dManager.WriteText(author, text, false, null, null));

                    yield return new WaitUntil(() => dManager.advance == true);
                }
            }
            else
            {
                // Use alternate text if no lines were found:

                foreach (string line in altLines)
                {
                    dManager.StartCoroutine(dManager.WriteText("", line, false, null, null));

                    yield return new WaitUntil(() => dManager.advance == true);
                }
            }
            if (next != null && next.Count != 0)
                yield return dManager.StartCoroutine(base.Process(eManager, dManager));
            else
                yield return dManager.StartCoroutine(dManager.HideDialogueBox());
        }
    }
}
