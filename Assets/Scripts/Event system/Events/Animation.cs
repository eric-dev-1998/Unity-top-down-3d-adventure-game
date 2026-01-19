using Assets.Scripts.Dialogue_System;
using Assets.Scripts.Event_System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Event_system.Events
{
    public class Animation : Event_System.Event
    {
        public string objectName;
        public string booleanName;
        public bool booleanValue;

        public Animation() { }

        public override IEnumerator Process(EventManager eManager, Manager dManager)
        {
            Animator anim = eManager.Find(objectName).GetComponent<Animator>();
            if (anim == null)
            {
                UnityEngine.Debug.LogError($"[Event manager]: No game object with name '{objectName}' was fond in current scene.");
                yield break;
            }

            anim.SetBool(booleanName, booleanValue);
            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            yield return base.Process(eManager, dManager);
        }
    }
}
