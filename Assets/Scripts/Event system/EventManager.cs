using System.Collections;
using UnityEngine;
using Assets.Scripts.Event_System;
using System;

namespace Assets.Scripts.Event_System
{
    public class EventManager : MonoBehaviour
    {
        public event Action OnEventFinished;

        public EventSequence currentEventSequence;
        public bool busy = false;

        public Quest_System.QuestManager questManager;
        public Inventory_System.Manager inventoryManager;

        private void Start()
        {
            questManager = FindAnyObjectByType<Quest_System.QuestManager>();
            inventoryManager = FindAnyObjectByType<Inventory_System.Manager>();
        }

        public void StartSequence(EventSequence eventSequence)
        {
            if (busy)
            {
                Debug.LogWarning("[Event manager]: Event manager is currently bussy.");
                return;
            }

            if (eventSequence == null)
            {
                Debug.LogWarning("[Event manager]: The selected event sequence is null or is corrupted. Operation aborted.");
                return;
            }

            currentEventSequence = eventSequence;
            StartCoroutine(ProcessSequence());
        }

        public IEnumerator ProcessSequence()
        {
            busy = true;

            Dialogue_System.Manager dialogueManager = FindAnyObjectByType<Dialogue_System.Manager>();
            yield return StartCoroutine(currentEventSequence.startEvent.Process(this, dialogueManager));

            // Finish event sequence:
            yield return StartCoroutine(FinishSequence());
        }

        public IEnumerator FinishSequence()
        {
            OnEventFinished?.Invoke();

            busy = false;
            yield return null;
        }
    }
}
