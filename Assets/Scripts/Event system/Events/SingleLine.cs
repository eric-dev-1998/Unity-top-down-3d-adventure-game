using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Event_System.Events
{
    [Serializable]
    public class SingleLine : Event
    {
        public string lineId;
        
        public enum Type { Dialogue, World };
        public Type type = Type.Dialogue;

        public SingleLine(string text, Type type) 
        {
            this.lineId = text;
            this.type = type;
        }

        public override IEnumerator Process(EventManager eManager, Dialogue_System.Manager dManager)
        {
            // Show dialogue box if is not enabled yet.
            if (!dManager.OnDialogue())
                yield return dManager.StartCoroutine(dManager.ShowDialogueBox());

            string text = "";
            string author = "";

            if (type == Type.Dialogue)
            {
                GameText.DialogueLine line = dManager.textManager.GetDialogueLine(lineId);
                text = line.content;
                author = line.id.Split('_')[0];
            }
            else if (type == Type.World)
            {
                text = dManager.textManager.GetWorldText(lineId);
            }

            // Start writing dialogue text.
            dManager.StartCoroutine(dManager.WriteText(author, text, false, null, null));
            yield return new WaitUntil(() => dManager.advance == true);

            if (next != null && next.Count != 0)
                yield return dManager.StartCoroutine(base.Process(eManager, dManager));
            else
                yield return dManager.StartCoroutine(dManager.HideDialogueBox());
        }

        public override void Debug()
        {
            UnityEngine.Debug.Log("This is a single line event.");
        }
    }
}
