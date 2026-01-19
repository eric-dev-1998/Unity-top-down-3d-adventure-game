using Assets.Scripts.Dialogue_System;
using Assets.Scripts.Event_System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Event_system.Events
{
    public class Camera : Event_System.Event
    {
        public string target;

        public Camera() { }

        public override IEnumerator Process(EventManager eManager, Manager dManager)
        {
            GameCamera camera = eManager.Find("Main Camera").GetComponent<GameCamera>();

            if (camera == null || string.IsNullOrEmpty(target))
                yield return base.Process(eManager, dManager);
            else
            {
                camera.SwitchFocusTarget(target);

                yield return new WaitUntil(() => camera.inPosition);

                yield return base.Process(eManager, dManager);
            }
        }
    }
}
