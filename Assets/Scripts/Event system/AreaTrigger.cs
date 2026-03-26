using Assets.Scripts.Event_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Event_system
{
    public class AreaTrigger : MonoBehaviour
    {
        public EventSequence onEnterSequence;
        public bool isEnterSequenceSynced;
        public EventSequence onExitSequence;
        public bool isExitSequenceSynced;

        // This property exist for debuging purposes.
        private bool isPlayerOnTrigger = false;

        private void TriggerEvent(EventSequence sequence, bool synced)
        { 
            EventManager manager = FindAnyObjectByType<EventManager>();
            if (manager != null && !manager.busy)
            {
                manager.questManager.ReachedArea(name);
                manager.StartSequence(sequence, synced);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (!isPlayerOnTrigger)
                {
                    isPlayerOnTrigger = true;

                    if (onEnterSequence != null)
                        TriggerEvent(onEnterSequence, isEnterSequenceSynced);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                isPlayerOnTrigger = false;

                if (onExitSequence != null)
                    TriggerEvent(onExitSequence, isExitSequenceSynced);
            }
        }
    }
}
