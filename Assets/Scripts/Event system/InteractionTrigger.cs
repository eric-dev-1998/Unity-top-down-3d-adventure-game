using UnityEngine;

namespace Assets.Scripts.Event_System
{
    public class InteractionTrigger : MonoBehaviour
    {
        public EventSequence sequence;

        private EventManager manager;

        private bool isPlayerOnTrigger = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            manager = FindAnyObjectByType<EventManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isPlayerOnTrigger && !manager.busy)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    manager.questManager.Interacted(name);
                    manager.StartSequence(sequence);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
                isPlayerOnTrigger = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.tag == "Player")
                isPlayerOnTrigger = false;
        }
    }
}
