using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Event_System;
using Assets.Scripts.Event_System.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Event_system
{
    public class InformationTrigger : MonoBehaviour
    {
        // Text to be displayed, it's text id's from the text library, plain text won't work.
        public string[] text;

        private bool onTrigger = false;

        private EventManager eManager;

        private void Start()
        {
            eManager = FindAnyObjectByType<EventManager>();
            if (eManager == null)
                Debug.LogError("[Information trigger]: No event manager was found on scene.");
        }

        private void Update()
        {
            if (onTrigger)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    TriggerSequence();
            }
        }

        private EventSequence Sequence()
        {
            if (text.Length <= 0)
            {
                Debug.LogError("[Information trigger]: No text was entered. Operation aborted.");
                return null;
            }

            EventSequence result = new EventSequence();
            if (text.Length == 1)
            {
                SingleLine line = new SingleLine(text[0], SingleLine.Type.World);
                result.startEvent = line;
            }
            else
            {
                Multiline multiline = new Multiline(text.ToList(), SingleLine.Type.World);
                result.startEvent = multiline;
            }

            return result;
        }

        private void TriggerSequence()
        {
            if (eManager != null && !eManager.busy)
                eManager.StartSequence(Sequence());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                onTrigger = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
                onTrigger = false;
        }
    }
}
