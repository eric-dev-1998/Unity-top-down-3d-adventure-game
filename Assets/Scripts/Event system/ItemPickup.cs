using UnityEngine;
using Assets.Scripts.Inventory_System;
using Assets.Scripts.Event_System;
using Assets.Scripts.Event_system.Events;
using EventSystem;
using Assets.Scripts.Player;

namespace Assets.Scripts.Event_system
{
    public class ItemPickup : MonoBehaviour
    {
        public Item item;
        public int count;

        private bool playerOnTrigger = false;
        public bool triggered = false;

        private EventSequence sequence;
        private PlayerCore player;

        private void Start()
        {
            triggered = false;

            if (item == null)
            {
                Debug.LogError($"[Item pickup]: '{name}', item data is null.");
                return;
            }

            if (count <= 0)
            {
                Debug.LogError($"[Item pickup]: '{name}, count value is not valid.'");
            }
        }

        private void Update()
        {
            if (!triggered) 
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (playerOnTrigger)
                    {
                        sequence = GeneratePickupSequence();
                        if (sequence == null)
                            return;

                        triggered = true;
                        Event_System.EventManager eManager = FindAnyObjectByType<Event_System.EventManager>();
                        eManager.StartSequence(sequence, true);
                    }
                }
            }
        }

        public EventSequence GeneratePickupSequence()
        {
            EventSequence sequence = new EventSequence();

            GameObjectEvent objectEvent = ScriptableObject.CreateInstance<GameObjectEvent>();
            objectEvent.type = GameObjectEvent.EventType.Disable;
            objectEvent.name = gameObject.name;
            objectEvent.Load();

            HumanGesture gestureEvent = ScriptableObject.CreateInstance<HumanGesture>();
            gestureEvent.characterId = "Player";
            gestureEvent.gestureType = 3;
            gestureEvent.wait = true;

            ItemEvent evt = ScriptableObject.CreateInstance<ItemEvent>();
            evt.type = ItemEvent.ItemEventType.Get;
            evt.id = item.item_id;
            evt.count = count;

            GameObjectEvent objectEvent1 = ScriptableObject.CreateInstance<GameObjectEvent>();
            objectEvent1.type = GameObjectEvent.EventType.Destroy;
            objectEvent1.name = gameObject.name;
            objectEvent1.Load();


            if (transform.position.y <= player.transform.position.y + 0.25f)
            {
                // This pickup is on the floor.

                gestureEvent.gestureType = 3;
            }
            else
            {
                // This pickup is not on the floor.

                gestureEvent.gestureType = 4;
            }

            gestureEvent.next.Add(objectEvent);
            objectEvent.next.Add(evt);
            evt.next.Add(objectEvent1);

            sequence = ScriptableObject.CreateInstance<EventSequence>();
            sequence.startEvent = gestureEvent;

            return sequence;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                player = other.GetComponent<PlayerCore>();
                playerOnTrigger = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
                playerOnTrigger = false;
        }
    }
}
