using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Runtime.Editor.Node_properties
{
    public class PathProperties : NodeProperties
    {
        public string who = "";
        public string path = "";
        public bool sync = false;

        public PathProperties() { }
    }
}