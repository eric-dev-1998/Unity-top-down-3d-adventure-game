using UnityEngine;
using System.Collections;
using Assets.Scripts.Event_System;
using Assets.Scripts.Dialogue_System;
using Assets.Scripts.World;

namespace Assets.Scripts.Event_system.Events
{
	public class ToggleWorldObject: Event_System.Event
	{
		public string objectName = "";

        public ToggleWorldObject(string objectName)
        {
            this.objectName = objectName;
        }

        public override IEnumerator Process(EventManager eManager, Manager dManager)
        {
            // 1. Get toggable:
            Toggable obj;
            try
            {
                obj = eManager.Find(objectName).GetComponent<Toggable>();
            }
            catch
            {
                // Stop this event if the specified object could not be found on scene.

                UnityEngine.Debug.LogError($"[Event manager][Toggle world object]: Object with name '{objectName}' was not found on scene. Event stopped.");
                eManager.currentEventSequence = null;
                eManager.busy = false;

                yield break;
            }

            // 2. Toggle:
            yield return obj.Toggle();

            // 3. Move on to next event:
            if (next.Count != 0 && next[0] != null)
                yield return next[0].Process(eManager, dManager);
        }
	}
}