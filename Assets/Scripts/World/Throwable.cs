using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Throwable : MonoBehaviour
    {
        private bool playerOnTrigger = false;
        private bool grabbed = false;
        private float visionThreshold = 0.7f;

        private PlayerCore player;

        private void Start()
        {
            player = FindAnyObjectByType<PlayerCore>();
        }

        private void Update()
        {
            if (!player.onGrab)
            {
                if (playerOnTrigger)
                {
                    if (Input.GetMouseButtonDown(0) && IsPlayerLooking())
                        Pickup();
                }
            }
            else
            {
                if (grabbed)
                {
                    if (Input.GetMouseButtonDown(0))
                        if (player.GetEntity().currentVelocity.magnitude >= 0.25f)
                            Throw();
                        else
                            PutDown();
                }
            }
        }

        public void Pickup()
        { 
            grabbed = true;
            player.onGrab = true;
            Debug.Log("Picked up.");
        }

        public void PutDown()
        {
            grabbed = false;
            player.onGrab = false;
            Debug.Log("Put down.");
        }

        public void Throw()
        { 
            grabbed = false;
            player.onGrab = false;
            Debug.Log("Thrown away.");
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
                playerOnTrigger = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
                playerOnTrigger = false;
        }

        private bool IsPlayerLooking()
        {
            RaycastHit hit;
            Ray ray = new Ray(player.transform.position + (Vector3.up * 0.25f), player.transform.forward);
            if (Physics.Raycast(ray, out hit, 1f))
            {
                if (hit.collider.gameObject == this.gameObject)
                    return true;
            }

            return false;
        }
    }
}
