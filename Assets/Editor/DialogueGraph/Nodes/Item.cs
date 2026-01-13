using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Runtime.Editor.Node_properties;
using Editor.DialogueGraph;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Editor.DialogueGraph.Nodes
{
    public class Item : Node
    {
        public ItemProperties properties => GetProperties<ItemProperties>();

        private DropdownField eventType;
        private TextField item_id;
        private IntegerField item_count;

        public Item() { }

        public Item(Vector2 screenPosition, DialogueGraphView parent) 
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
            node_data.title = "Item node";

            string contentPath = "Assets/Editor/DialogueGraph/uxml/item.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            eventType = node_data.extensionContainer.Q<DropdownField>("EventType");
            item_id = node_data.extensionContainer.Q<TextField>("Item_Id");
            item_count = node_data.extensionContainer.Q<IntegerField>("Item_Count");

            if (properties != null)
            {
                eventType.index = ((int)properties.eventType);
                item_id.value = properties.id;
                item_count.value = properties.count;
            }    
        }

        public override void CreateProperties()
        {
            saveData.properties = ScriptableObject.CreateInstance<ItemProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();

            SetCallbacks();
        }

        public override void LoadProperties()
        {
            eventType.index = ((int)properties.eventType);
            item_id.value = properties.id;
            item_count.value = properties.count;

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            eventType.RegisterValueChangedCallback(evt => {
                properties.eventType = (ItemEvent.ItemEventType)Enum.Parse(typeof(ItemEvent.ItemEventType), evt.newValue);
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
            item_id.RegisterValueChangedCallback(evt => {
                properties.id = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
            item_count.RegisterValueChangedCallback<int>(evt => {
                properties.count = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
        }
    }
}
