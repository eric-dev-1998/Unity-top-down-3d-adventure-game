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
        public EventSequence sequence;

        private bool isPlayerOnTrigger = false;

        private void TriggerEvent()
        { 
            EventManager manager = FindAnyObjectByType<EventManager>();
            if (manager != null && !manager.busy)
            {
                manager.questManager.ReachedArea(name);
                manager.StartSequence(sequence);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                if (!isPlayerOnTrigger)
                { 
                    isPlayerOnTrigger = true;
                    TriggerEvent();
                }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.tag == "Player")
                isPlayerOnTrigger = false;
        }
    }
}
