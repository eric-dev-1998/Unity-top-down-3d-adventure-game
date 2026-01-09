using EventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using Assets.Scripts.Event_System.Events;
using Assets.Scripts.Event_system.Events;

namespace Assets.Scripts.Event_System
{
    [Serializable]
    public class Event : ScriptableObject
    {
        public List<Event> next = new List<Event>();

        public virtual void Load()
        { 
            
        }

        public virtual IEnumerator Process(EventManager eManager, Dialogue_System.Manager dManager)
        {
            if (next != null && next.Count != 0)
            {
                foreach (Event e in next)
                {
                    if (e is not SingleLine && e is not Multiline && e is not Question)
                        yield return dManager.StartCoroutine(dManager.HideDialogueBox());
                    yield return eManager.StartCoroutine(e.Process(eManager, dManager));
                }
            }
        }

        public virtual void Debug() {}
    }
}
