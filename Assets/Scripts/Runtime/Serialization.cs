using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.DialogueGraph.Serialization
{
    [Serializable]
    public class SerializedNode
    {
        // This class is used for copy/paste functions on the graph editor.

        public string type;
        public string json;
    }

    [Serializable]
    public class PortInfo
    {
        public string id;
        public string name;

        public PortInfo(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    [Serializable]
    public class Connection
    {
        public PortInfo from;
        public PortInfo to;

        public Color color = Color.white;
        public bool isActive = false;

        public Connection() { }

        public Connection(PortInfo from, PortInfo to)
        {
            this.from = from;
            this.to = to;
        }
    }
}
