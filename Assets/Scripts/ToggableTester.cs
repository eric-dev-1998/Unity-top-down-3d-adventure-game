using Assets.Scripts.World;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class ToggableTester : MonoBehaviour
    {
        private Toggable toggable;

        // Use this for initialization
        void Start()
        {
            toggable = GameObject.Find("LabGate").GetComponent<Toggable>();
            if (toggable != null)
                Debug.Log("Found toggable.");

            toggable.Toggle();
        }
    }
}