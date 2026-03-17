using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Runtime.Editor.Node_properties;
using Editor.DialogueGraph;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Editor.DialogueGraph.Nodes
{
    public class Toggable : Node
    {
        public ToggableObjectProperties properties => GetProperties<ToggableObjectProperties>();

        private TextField objectName;

        public Toggable() { }

        public Toggable(Vector2 screenPosition, DialogueGraphView parent)
        {
            this.parent = parent;
            this.screenPosition = screenPosition;

            CreateNode();
            CreateProperties();
            LoadVisualContent();

            AssetDatabase.AddObjectToAsset(saveData, parent.currentGraphData);
        }

        public override void LoadVisualContent()
        {
            node_data.title = "Toggle world object";

            string contentPath = "Assets/Editor/DialogueGraph/uxml/toggable.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            objectName = node_data.extensionContainer.Q<TextField>("ObjectName");

            if (properties != null)
                LoadProperties();
        }

        public override void CreateProperties()
        {
            saveData.properties = ScriptableObject.CreateInstance<ToggableObjectProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();

            SetCallbacks();
        }

        public override void LoadProperties()
        {
            objectName.value = properties.name;

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            objectName.RegisterValueChangedCallback(evt => {
                properties.objectName = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
        }
    }
}
