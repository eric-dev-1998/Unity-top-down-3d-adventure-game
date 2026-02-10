using Assets.Scripts.Dialogue_System;
using Assets.Scripts.Event_System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Event_system.Events
{
    public class HumanGesture : Event_System.Event
    {
        public string characterId;
        public int gestureType;
        public bool wait;

        HumanGesture(string characterId, int gestureType, bool wait)
        {
            this.characterId = characterId;
            this.gestureType = gestureType;
            this.wait = wait;
        }

        public override IEnumerator Process(EventManager eManager, Manager dManager)
        {
            // Find target character entity.
            Entity e = eManager.FindEntity(characterId);
            if (!e)
                yield break;

            // Play gesture animation.
            if (wait)
                yield return eManager.StartCoroutine(e.entityAnimator.PlayGesture(gestureType));
            else
                eManager.StartCoroutine(e.entityAnimator.PlayGesture(gestureType));

            // Continue.
            yield return next[0].Process(eManager, dManager);
        }
    }
}
