using UnityEngine;

namespace Main
{
    public class Water: MonoBehaviour
    {
        public Transform colliderObject;
        public Material material;

        private void Update()
        {
            if (colliderObject != null && material != null)
            {
                material.SetVector("_ColliderPosition", colliderObject.position);
            }
        }
    }
}
