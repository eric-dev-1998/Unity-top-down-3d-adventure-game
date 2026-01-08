using UnityEngine;

namespace Assets.Scripts.Runtime.Editor
{
    public class NodeSaveData: ScriptableObject
    {
        public string id;
        public string typeName;
        public Vector2 position;

        public NodeProperties properties;
    }
}
