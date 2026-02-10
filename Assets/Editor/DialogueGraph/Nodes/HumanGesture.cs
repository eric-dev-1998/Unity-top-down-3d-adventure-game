using Assets.Scripts.Runtime.Editor.Node_properties;
using Editor.DialogueGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Editor.DialogueGraph.Nodes
{
    public class HumanGesture : Node
    {
        public GestureProperties properties => GetProperties<GestureProperties>();

        private TextField characterId;
        private DropdownField type;
        private Toggle wait;

        public HumanGesture() { }

        public HumanGesture(Vector2 screenPosition, DialogueGraphView parent)
        {
            name = "Play human gesture";
            this.parent = parent;
            this.screenPosition = screenPosition;

            CreateNode();
            CreateProperties();
            LoadVisualContent();

            AssetDatabase.AddObjectToAsset(saveData, parent.currentGraphData);
        }

        public override void LoadVisualContent()
        {
            node_data.title = "Play human gesture";

            // Load node uxml fields:
            string contentPath = "Assets/Editor/DialogueGraph/uxml/humanGesture.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            // Get loaded node fields:
            characterId = node_data.extensionContainer.Q<TextField>("Character");
            type = node_data.extensionContainer.Q<DropdownField>("GestureType");
            wait = node_data.extensionContainer.Q<Toggle>("Wait");

            if (properties != null)
                LoadProperties();
        }

        public override void CreateProperties()
        {
            // Create properties instance in save data for serialization:
            saveData.properties = ScriptableObject.CreateInstance<GestureProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();

            SetCallbacks();
        }

        public override void LoadProperties()
        {
            characterId.value = properties.characterName;
            type.index = (int)properties.gestureType;
            wait.value = properties.waitUntilFinish;

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            // Register ui element callbacks to keep properties updated:
            characterId.RegisterValueChangedCallback(evt => {
                properties.characterName = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
            type.RegisterValueChangedCallback(evt => {
                Enum.TryParse(evt.newValue, out properties.gestureType);
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
            wait.RegisterValueChangedCallback(evt => {
                properties.waitUntilFinish = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
        }
    }
}
