using UnityEngine;

namespace Assets.Scripts.Runtime.Editor
{
    public class ConnectionSaveData : ScriptableObject
    {
        public string outputNodeId;
        public string outputPortName;

        public string inputNodeId;
        public string inputPortName;
    }
}
