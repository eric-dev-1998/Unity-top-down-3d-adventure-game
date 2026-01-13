using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using Assets.Scripts.Runtime.Editor;

namespace Editor.DialogueGraph
{
    [Serializable]
    public class Node
    {
        // Unity node instance.
        [NonSerialized]
        public UnityEditor.Experimental.GraphView.Node node_data;

        // Unity node visual content.
        [NonSerialized]
        public VisualElement visual_content;

        // This id will be used to identify this node on the graph viewer and handle connections when
        // loading a graph from a file.
        public string id = "";

        // Node name, used as title.
        public string name = string.Empty;

        // Node position on screen.
        public Vector2 screenPosition;

        // UI parent (GraphView)
        public DialogueGraphView parent;

        // A unity node itself cannot be serialized, and as aconsecuence, its connections wont be saved either.
        // A serializable list 
        public List<int> input_connections;
        public List<int> output_connection;

        // Serializable data:
        public NodeSaveData saveData;

        public T GetProperties<T>() where T : NodeProperties
        {
            return saveData.properties as T;
        }

        public void CreateNode()
        {
            if (parent == null)
            {
                Debug.LogError("Node creation aborted, no parent graph was declared.");
                return;
            }

            // Instantiate visual node:
            node_data = new UnityEditor.Experimental.GraphView.Node();
            node_data.title = name;
            node_data.extensionContainer.style.paddingTop = 6;
            node_data.extensionContainer.style.paddingBottom = 6;
            node_data.extensionContainer.style.paddingLeft = 6;
            node_data.extensionContainer.style.paddingRight = 6;

            // Create main ports:
            Port inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Node));
            inputPort.portName = "Input";
            
            Port outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Node));
            outputPort.portName = "Output";

            node_data.inputContainer.Add(inputPort);
            node_data.outputContainer.Add(outputPort);

            // Apply position.
            Vector2 startPosition = Vector2.zero;
            if (saveData != null)
                startPosition = saveData.position;
            else
                startPosition = screenPosition;

                Rect position = new Rect(startPosition.x, startPosition.y, 128, 128);
            node_data.SetPosition(position);

            // Add to graph view:
            parent.AddElement(node_data);

            // Refresh node:
            node_data.RefreshPorts();
            node_data.RefreshExpandedState();
        }

        public virtual void CreateProperties()
        {
            AssetDatabase.AddObjectToAsset(saveData.properties, saveData);
        }

        public virtual void LoadProperties() { }

        public virtual void LoadVisualContent()
        {
            // Load visual content.
            node_data.extensionContainer.Add(visual_content);
            node_data.RefreshExpandedState();
        }

        public void MakeOutputOnly(string outputPortName)
        {
            Port inputPort;
            Port outputPort;

            inputPort = node_data.inputContainer.ElementAt(0) as UnityEditor.Experimental.GraphView.Port;
            outputPort = node_data.outputContainer.ElementAt(0) as UnityEditor.Experimental.GraphView.Port;

            node_data.inputContainer.Remove(inputPort);
            outputPort.portName = outputPortName;

            node_data.RefreshPorts();
            node_data.RefreshExpandedState();
        }

        public void MakeTrueOrFalseOuputOnly()
        {
            node_data.inputContainer.RemoveAt(0);

            GetOutputPort().portName = "On true";

            Port secondOuputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Node));
            secondOuputPort.portName = "On false";
            node_data.outputContainer.Add(secondOuputPort);

            node_data.RefreshPorts();
            node_data.RefreshExpandedState();
        }

        public virtual void OverwriteOutputPorts()
        {
            // Clear current nodes to create new ports.

            node_data.outputContainer.Clear();
        }

        public Port GetInputPort()
        {
            if (node_data.inputContainer.childCount <= 0)
                return null;

            return node_data.inputContainer.ElementAt(0) as Port;
        }

        public Port GetOutputPort()
        {
            return node_data.outputContainer.ElementAt(0) as Port;
        }

        public Port GetOutputPort(int index)
        { 
            return node_data.outputContainer.ElementAt(index) as Port;
        }
    }
}