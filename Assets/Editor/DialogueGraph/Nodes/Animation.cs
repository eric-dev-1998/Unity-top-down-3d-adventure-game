using Assets.Scripts.Runtime.Editor.Node_properties;
using Editor.DialogueGraph;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DialogueGraph.Nodes
{
    public class Animation : Node
    {
        public AnimationProperties properties => GetProperties<AnimationProperties>();

        private TextField objectName;
        private TextField booleanName;
        private Toggle booleanValue;

        public Animation() { }

        public override void LoadVisualContent()
        {
            node_data.title = "Play animation";

            // Load node uxml fields:
            string contentPath = "Assets/Editor/DialogueGraph/uxml/animation.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            // Get loaded node fields:
            objectName = node_data.extensionContainer.Q<TextField>("ObjectName");
            booleanName = node_data.extensionContainer.Q<TextField>("BooleanName");
            booleanValue = node_data.extensionContainer.Q<Toggle>("BooleanValue");

            if (properties != null)
            {
                objectName.value = properties.objectName;
                booleanName.value = properties.booleanName;
                booleanValue.value = properties.booleanValue;
            }
        }

        public override void CreateProperties()
        {
            // Create properties instance in save data for serialization:
            saveData.properties = ScriptableObject.CreateInstance<AnimationProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();

            SetCallbacks();
        }

        public override void LoadProperties()
        {
            objectName.value = properties.objectName;
            booleanName.value = properties.booleanName;
            booleanValue.value = properties.booleanValue;

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            // Register ui element callbacks to keep properties updated:
            objectName.RegisterValueChangedCallback(evt => {
                properties.objectName = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
            booleanName.RegisterValueChangedCallback(evt => {
                properties.booleanName = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
            booleanValue.RegisterValueChangedCallback(evt => {
                properties.booleanValue = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
        }
    }
}
