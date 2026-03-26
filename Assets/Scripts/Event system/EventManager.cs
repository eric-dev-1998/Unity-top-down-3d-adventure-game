using System.Collections;
using UnityEngine;
using Assets.Scripts.Event_System;
using System;
using Assets.Scripts.Event_system;
using Assets.Scripts.World.Npc;
using System.Linq;

namespace Assets.Scripts.Event_System
{
    public class EventManager : MonoBehaviour
    {
        public event Action OnEventFinished;

        public EventSequence currentEventSequence;
        public bool busy = false;

        public Quest_System.QuestManager questManager;
        public Inventory_System.InventoryManager inventoryManager;

        private Dialogue_System.Manager dialogueManager;

        public IconManager iconManager;

        public GameObject owner;

        private void Start()
        {
            questManager = FindAnyObjectByType<Quest_System.QuestManager>();
            inventoryManager = FindAnyObjectByType<Inventory_System.InventoryManager>();
            iconManager = new IconManager(this);
            dialogueManager = FindAnyObjectByType<Dialogue_System.Manager>();
        }

        public void StartSequence(EventSequence eventSequence, bool isSynced)
        {
            if (eventSequence == null)
            {
                Debug.LogWarning("[Event manager]: The selected event sequence is null or is corrupted. Operation aborted.");
                return;
            }

            if (isSynced)
            {
                if (busy)
                {
                    Debug.LogWarning("[Event manager]: Event manager is currently bussy.");
                    return;
                }

                currentEventSequence = eventSequence;
                StartCoroutine(ProcessSequence());
            }
            else
            {
                StartCoroutine(eventSequence.startEvent.Process(this, dialogueManager));
            }
        }

        public void StartSequence(EventSequence eventSequence, GameObject owner)
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

            this.owner = owner;
            currentEventSequence = eventSequence;
            StartCoroutine(ProcessSequence());
        }

        public IEnumerator ProcessSequence()
        {
            busy = true;

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

        public GameObject Find(string objectName)
        {
            return GameObject.Find(objectName);
        }

        public Entity FindEntity(string id)
        {
            if (id == "Player" || id == "player")
                return Find("Player").GetComponent<Entity>();

            var entitiesOnScene = FindObjectsByType<NpcCore>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
            if (entitiesOnScene.Count == 0)
                Debug.LogError("[Event manager]: No npc was found on current scene.");

            Entity res = entitiesOnScene.Find(e => e.npc_id == id).GetEntity();
            if (!res)
                Debug.LogErrorFormat($"[Event manager]: No npc with id: '{id}' was found on current scene.");

            return res;
        }
    }
}
