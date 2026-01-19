using Assets.Scripts.Runtime.Editor;
using Assets.Scripts.Runtime.Editor.Node_properties;
using Editor.DialogueGraph;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Editor.DialogueGraph.Nodes
{
    public class Camera : Node
    {
        public CameraProperties properties => GetProperties<CameraProperties>();

        public TextField target;

        public Camera() { }

        public override void LoadVisualContent()
        {
            node_data.title = "Set camera target";

            // Load node uxml fields:
            string contentPath = "Assets/Editor/DialogueGraph/uxml/camera.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            // Get loaded node fields:
            target = node_data.extensionContainer.Q<TextField>("Target");

            if (properties != null)
            {
                target.value = properties.target;
            }
        }

        public override void CreateProperties()
        {
            // Create properties instance in save data for serialization:
            saveData.properties = ScriptableObject.CreateInstance<CameraProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();

            SetCallbacks();
        }

        public override void LoadProperties()
        {
            target.value = properties.target;

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            // Register ui element callbacks to keep properties updated:
            target.RegisterValueChangedCallback(evt => {
                properties.target = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
        }
    }
}
