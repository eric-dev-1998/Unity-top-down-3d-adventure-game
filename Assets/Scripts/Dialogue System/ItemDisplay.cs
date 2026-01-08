using UnityEngine;

namespace Assets.Scripts.Dialogue_System
{
    public class ItemDisplay : MonoBehaviour
    {
        GameObject container;

        private void Start()
        {
            container = transform.Find("Container").gameObject;
        }

        public void DestroyItemDisplay()
        {
            if(container.transform.childCount != 0)
                Destroy(container.transform.GetChild(0).gameObject);
        }
    }
}
