using UnityEngine;

namespace Assets.Scripts.Event_system
{
    public class InteractionIcon
    {
        private Transform parent;
        private GameObject iconObject;
        private Animator anim;

        public InteractionIcon(Transform parent)
        { 
            this.parent = parent;
        }

        private void LoadPrefab()
        {
            var resource = Resources.Load<GameObject>("World/Interaction_Icon");
            iconObject = GameObject.Instantiate(resource, parent.Find("Icon"));
            anim = iconObject.GetComponent<Animator>();
        }

        public void Show()
        {
            if (iconObject == null)
                LoadPrefab();

            anim.SetBool("Show", true);
        }

        public void Hide()
        {
            if (!iconObject)
                return;

            anim.SetBool("Show", false);
        }
    }
}
