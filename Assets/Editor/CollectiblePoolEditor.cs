using Assets.Scripts.World;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [CustomEditor(typeof(CollectiblePool))]
    public class CollectiblePoolEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Pressing the following button will find every collectible in the scene and will add it to this pool and will organize them on separete groups depending on their type.", EditorStyles.boldLabel);

            if (GUILayout.Button("Fill pool"))
            {
                Refill();
            }

            base.OnInspectorGUI();
        }

        private void Refill()
        {
            Debug.Log("Refilled");
        }
    }
}
