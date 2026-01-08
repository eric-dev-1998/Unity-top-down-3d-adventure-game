using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Runtime.Editor.Node_properties;
using Editor.DialogueGraph;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Editor.DialogueGraph.Nodes
{
    public class GameObject : Node
    {
        public GameObjectProperties properties => GetProperties<GameObjectProperties>();

        private DropdownField eventType;
        private TextField objectName;

        public GameObject() { }

        public GameObject(Vector2 screenPosition, DialogueGraphView parent) 
        {
            name = "Line";
            this.parent = parent;
            this.screenPosition = screenPosition;

            CreateNode();
            CreateProperties();
            LoadVisualContent();

            AssetDatabase.AddObjectToAsset(saveData, parent.currentGraphData);
        }

        public override void LoadVisualContent()
        {
            node_data.title = "Dialogue line";

            // Load node uxml fields:
            string contentPath = "Assets/Editor/DialogueGraph/uxml/gameObject.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            // Get loaded node fields:
            eventType = node_data.extensionContainer.Q<DropdownField>("Type");
            objectName = node_data.extensionContainer.Q<TextField>("Target");

            if (properties != null)
            {
                eventType.value = properties.type.ToString();
                objectName.value = properties.name;
            }
        }

        public override void CreateProperties()
        {
            // Create properties instance in save data for serialization:
            saveData.properties = ScriptableObject.CreateInstance<GameObjectProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();

            SetCallbacks();
        }

        public override void LoadProperties()
        {
            eventType.index = (int)properties.type;
            objectName.value = properties.name;

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            // Register ui element callbacks to keep properties updated:
            eventType.RegisterValueChangedCallback(evt => {
                properties.type = (GameObjectEvent.EventType)Enum.Parse(typeof(GameObjectEvent.EventType), evt.newValue);
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
            objectName.RegisterValueChangedCallback(evt => {
                properties.name = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
        }
    }
}
