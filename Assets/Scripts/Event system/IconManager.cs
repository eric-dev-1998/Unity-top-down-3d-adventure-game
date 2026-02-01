using Assets.Scripts.Event_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Event_system
{
    public class IconManager
    {
        public enum IconType { Interaction };

        private EventManager eManager;
        private List<GameObject>availableIcons = new List<GameObject>();
        private List<GameObject>usedIcons = new List<GameObject>();

        Transform interactionParent;

        public IconManager(EventManager eManager)
        {
            this.eManager = eManager;
            LoadIcons();
        }

        private void LoadIcons()
        {
            var icons = Resources.LoadAll<GameObject>("World/Icons");

            foreach (var icon in icons)
            {
                GameObject instance = GameObject.Instantiate(icon, eManager.transform);
                instance.transform.position = Vector3.zero;
                instance.GetComponent<Animator>().SetFloat("Speed", 0);

                availableIcons.Add(instance);
                Debug.Log(instance.name);
            }

            if (availableIcons.Count <= 0)
                Debug.LogError("[Event manager]: Interaction icons could not be loaded.");
        }

        public GameObject GetIcon(IconType type, Transform parent)
        {
            GameObject icon = null;

            switch (type)
            {
                case IconType.Interaction:
                    icon = availableIcons.Find(i => i.name == "Interaction(Clone)");
                    if (interactionParent != parent)
                        interactionParent = parent;
                    break;
            }

            if (icon == null)
            {
                Debug.LogError("[Event][Icon]: No icon could be shared, the desired icon is already on use.");
                return null;
            }

            usedIcons.Add(icon);
            availableIcons.Remove(icon);

            icon.transform.SetParent(interactionParent);
            icon.transform.position = Vector3.zero;

            icon.GetComponent<Animator>().SetFloat("Speed", 1f);
            icon.GetComponent<Animator>().SetBool("Show", true);

            return icon;
        }

        public void RetrieveIcon(IconType type)
        {
            GameObject icon = null;

            switch (type)
            {
                case IconType.Interaction:
                    icon = usedIcons.Find(i => i.name == "Interaction(Clone)");
                    interactionParent = null;
                    break;
            }

            if (icon == null)
            {
                Debug.LogError("[Event manager][Icon]: The icon is not in use.");
                return;
            }

            availableIcons.Add(icon);
            usedIcons.Remove(icon);

            eManager.StartCoroutine(HideIcon(icon));
        }

        IEnumerator HideIcon(GameObject icon)
        {
            icon.GetComponent<Animator>().SetBool("Show", false);

            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => icon.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

            icon.transform.SetParent(eManager.transform);
            icon.transform.localPosition = Vector3.zero;

            icon.GetComponent<Animator>().SetFloat("Speed", 0);
        }
    }
}
