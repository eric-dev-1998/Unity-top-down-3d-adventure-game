using Assets.Scripts.Runtime.Editor.Node_properties;
using Editor.DialogueGraph;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Editor.DialogueGraph.Nodes
{
	public class FollowPath: Node
	{
        public PathProperties properties => GetProperties<PathProperties>();

        private TextField who;
        private TextField path;
        private Toggle sync;

        public FollowPath() { }

        public FollowPath(Vector2 screenPosition, DialogueGraphView parent)
        {
            name = "Follow path";
            this.parent = parent;
            this.screenPosition = screenPosition;

            CreateNode();
            CreateProperties();
            LoadVisualContent();

            AssetDatabase.AddObjectToAsset(saveData, parent.currentGraphData);
        }

        public override void LoadVisualContent()
        {
            node_data.title = "Follow path";

            // Load node uxml fields:
            string contentPath = "Assets/Editor/DialogueGraph/uxml/path.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            // Get loaded node fields:
            who = node_data.extensionContainer.Q<TextField>("Who");
            path = node_data.extensionContainer.Q<TextField>("PathID");
            sync = node_data.extensionContainer.Q<Toggle>("Sync");

            if (properties != null)
                LoadProperties();
        }

        public override void CreateProperties()
        {
            // Create properties instance in save data for serialization:
            saveData.properties = ScriptableObject.CreateInstance<PathProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();

            SetCallbacks();
        }

        public override void LoadProperties()
        {
            who.value = properties.who;
            path.value = properties.path;
            sync.value = properties.sync;

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            // Register ui element callbacks to keep properties updated:
            who.RegisterValueChangedCallback(evt => {
                properties.who = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
            path.RegisterValueChangedCallback(evt => {
                properties.path = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
            sync.RegisterValueChangedCallback(evt => {
                properties.sync = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });

        }
    }
}