using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Event_System;
using Assets.Scripts.Event_System.Events;
using Assets.Scripts.GameText;
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
        public string worldTextId;

        private string plainText;

        private bool onTrigger = false;

        private EventManager eManager;

        private void Start()
        {
            TextManager textManager = FindAnyObjectByType<TextManager>();
            if (textManager == null)
            {
                Debug.LogError($"[Door][{name}]: Text manager was not found on scene.");
                this.enabled = false;
                return;
            }

            plainText = textManager.GetWorldText(worldTextId);
            if (plainText == "")
            {
                this.enabled = false;
                return;
            }

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
            var lines = plainText.Split('\n');

            EventSequence result = new EventSequence();
            Multiline multiline = new Multiline(null, SingleLine.Type.World);
            multiline.altLines = lines.ToList();
            result.startEvent = multiline;

            return result;
        }

        private void TriggerSequence()
        {
            if (eManager != null && !eManager.busy)
                eManager.StartSequence(Sequence(), true);
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
