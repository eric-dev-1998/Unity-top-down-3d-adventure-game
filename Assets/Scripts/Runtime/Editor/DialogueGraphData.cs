using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Editor
{
    [CreateAssetMenu(fileName = "New dialogue graph", menuName = "DialogueGraph/New graph")]
    public class DialogueGraphData : ScriptableObject
    {
        public string projectName = "";
        public List<NodeSaveData> nodes = new List<NodeSaveData>();
        public List<ConnectionSaveData> connections = new List<ConnectionSaveData>();
    }
}
