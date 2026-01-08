using UnityEngine;
using Assets.Scripts.Inventory_System;
using Assets.Scripts.Event_System;
using Assets.Scripts.Event_system.Events;

namespace Assets.Scripts.Event_system
{
    public class ItemPickup : MonoBehaviour
    {
        public Item item;
        public int count;

        private bool playerOnTrigger = false;
        public bool triggered = false;

        private EventSequence sequence;

        private void Start()
        {
            if (item == null)
            {
                Debug.LogError($"[Item pickup]: '{name}', item data is null.");
                return;
            }

            if (count <= 0)
            {
                Debug.LogError($"[Item pickup]: '{name}, count value is not valid.'");
            }

            GameObjectEvent objectEvent = ScriptableObject.CreateInstance<GameObjectEvent>();
            objectEvent.type = GameObjectEvent.EventType.Disable;
            objectEvent.name = gameObject.name;
            objectEvent.Load();

            ItemEvent evt = ScriptableObject.CreateInstance<ItemEvent>();
            evt.type = ItemEvent.ItemEventType.Get;
            evt.id = item.item_id;
            evt.count = count;

            GameObjectEvent objectEvent1 = ScriptableObject.CreateInstance<GameObjectEvent>();
            objectEvent1.type = GameObjectEvent.EventType.Destroy;
            objectEvent1.name = gameObject.name;
            objectEvent1.Load();

            objectEvent.next.Add(evt);
            evt.next.Add(objectEvent1);

            sequence = ScriptableObject.CreateInstance<EventSequence>();
            sequence.startEvent = objectEvent;

            if (sequence == null)
            {
                Debug.LogError($"[Item pickup]: '{name}' Sequence could not be created.");
            }
        }

        private void Update()
        {
            if (!triggered) 
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (playerOnTrigger && sequence != null)
                    {
                        triggered = true;
                        Event_System.Manager eManager = FindAnyObjectByType<Event_System.Manager>();
                        eManager.StartSequence(sequence);
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                playerOnTrigger = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
                playerOnTrigger = false;
        }
    }
}
