using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Event_System.Events
{
    [Serializable]
    public class SingleLine : Event
    {
        public string author;
        public string text;

        public SingleLine(string author, string text) 
        {
            this.author = author;
            this.text = text;
        }

        public override IEnumerator Process(Manager eManager, Dialogue_System.Manager dManager)
        {
            // Show dialogue box if is not enabled yet.
            if (!dManager.OnDialogue())
                yield return dManager.StartCoroutine(dManager.ShowDialogueBox());

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
