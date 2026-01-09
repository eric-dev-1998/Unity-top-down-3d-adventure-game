using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Event_system.Events
{
    public class GameObjectEvent : Event_System.Event
    {
        public enum EventType { Destroy, Enable, Disable };

        public EventType type;
        public new string name;

        private GameObject target;

        public override void Load()
        {
            target = GameObject.Find(name);
        }

        public override IEnumerator Process(Event_System.EventManager eManager, Dialogue_System.Manager dManager)
        {
            if (string.IsNullOrEmpty(name))
            {
                UnityEngine.Debug.LogError($"[Event manager, GameObject event]: No target name was specified or is not valid.");
                yield return base.Process(eManager, dManager);
            }

            target = GameObject.Find(name);

            if (type == EventType.Destroy)
            {
                yield return dManager.StartCoroutine(DestroyObject());
            }
            else if (type == EventType.Enable)
            {
                yield return dManager.StartCoroutine(EnableObject());
            }
            else if (type == EventType.Disable)
            {
                yield return dManager.StartCoroutine(DisableObject());
            }

            yield return base.Process(eManager, dManager);
        }

        private IEnumerator DestroyObject()
        {
            Destroy(target);
            yield break;
        }

        private IEnumerator EnableObject()
        {
            target.SetActive(true);
            yield break;
        }

        private IEnumerator DisableObject()
        {
            target.SetActive(false);
            yield break;
        }
    }
}
