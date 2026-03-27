using Assets.Scripts.Runtime.Editor.Node_properties;
using Editor.DialogueGraph;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Editor.DialogueGraph.Node;

namespace Assets.Editor.DialogueGraph.Nodes
{
    public class ItemOwnership : Node
    {
        public ItemOwnershipProperties properties => GetProperties<ItemOwnershipProperties>();

        private TextField itemId;
        private IntegerField count;

        public ItemOwnership() { }

        public ItemOwnership(Vector2 screenPosition, DialogueGraphView parent)
        {
            name = "Player owns item";
            this.parent = parent;
            this.screenPosition = screenPosition;

            CreateNode();
            CreateProperties();
            LoadVisualContent();

            AssetDatabase.AddObjectToAsset(saveData, parent.currentGraphData);
        }

        public override void OverwriteOutputPorts()
        {
            // Clear current output ports.
            base.OverwriteOutputPorts();

            Port a = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Node));
            a.portName = "On option A";

            Port b = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Node));
            b.portName = "On option B";

            node_data.outputContainer.Add(a);
            node_data.outputContainer.Add(b);

            node_data.RefreshPorts();
            node_data.RefreshExpandedState();

            Debug.Log($"[Question node]: Output ports updated, current port count: {node_data.outputContainer.childCount}");
        }

        public override void LoadVisualContent()
        {
            node_data.title = "Player owns item";

            // Load node uxml fields:
            string contentPath = "Assets/Editor/DialogueGraph/uxml/itemOwned.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            // Get loaded node fields:
            itemId = node_data.extensionContainer.Q<TextField>("ItemId");
            count = node_data.extensionContainer.Q<IntegerField>("ItemCount");

            if (properties != null)
            {
                itemId.value = properties.itemId;
                count.value = properties.count;
            }

            OverwriteOutputPorts();
        }

        public override void CreateProperties()
        {
            // Create properties instance in save data for serialization:
            saveData.properties = ScriptableObject.CreateInstance<ItemOwnershipProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();

            SetCallbacks();

        }

        public override void LoadProperties()
        {
            itemId.value = properties.itemId;
            count.value = properties.count;

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            itemId.RegisterValueChangedCallback(evt => {
                properties.itemId = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });

            count.RegisterValueChangedCallback(evt => {
                properties.count = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
        }
    }
}
