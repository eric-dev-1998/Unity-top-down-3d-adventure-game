using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Scripts.Runtime.Editor.Node_properties;
using Assets.Scripts.GameText;

namespace Editor.DialogueGraph.Nodes
{
    public class Line : Node
    {
        public DialogueLineProperties properties => GetProperties<DialogueLineProperties>();

        private TextField preview;
        private TextField text;

        public Line() { }

        public Line(Vector2 screenPosition, DialogueGraphView parent)
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
            string contentPath = "Assets/Editor/DialogueGraph/uxml/line.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            // Get loaded node fields:
            preview = node_data.extensionContainer.Q<TextField>("Preview");
            text = node_data.extensionContainer.Q<TextField>("Text");

            if (properties != null)
            { 
                text.value = properties.lineId;
            }
        }

        public override void CreateProperties()
        {
            // Create properties instance in save data for serialization:
            saveData.properties = ScriptableObject.CreateInstance<DialogueLineProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();

            SetCallbacks();
        }

        public override void LoadProperties()
        {
            text.value = properties.lineId;

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            // Register ui element callbacks to keep properties updated:
            text.RegisterValueChangedCallback(evt => {
                DialogueLine line = parent.library.dialogueText.Find(t => t.id == evt.newValue);
                if (line != null)
                    preview.value = line.content;
                else
                    preview.value = "";

                properties.lineId = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);               
            });
        }
    }
}
