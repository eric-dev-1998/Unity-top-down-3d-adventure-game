using Assets.Scripts.Runtime.Editor.Node_properties;
using Editor.DialogueGraph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Editor.DialogueGraph.Node;

namespace Assets.Editor.DialogueGraph.Nodes
{
    public class Question : Node
    {
        public QuestionProperties properties => GetProperties<QuestionProperties>();

        private TextField question;

        public Question() { }

        public Question(Vector2 screenPosition, DialogueGraphView parent)
        {
            name = "Question";
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
            node_data.title = "Question";

            // Load node uxml fields:
            string contentPath = "Assets/Editor/DialogueGraph/uxml/question.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            // Get loaded node fields:
            question = node_data.extensionContainer.Q<TextField>("Text_Question");

            if (properties != null)
            {
                question.value = properties.question;
            }

            OverwriteOutputPorts();
        }

        public override void CreateProperties()
        {
            // Create properties instance in save data for serialization:
            saveData.properties = ScriptableObject.CreateInstance<QuestionProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();

            SetCallbacks();

        }

        public override void LoadProperties()
        {
            question.value = properties.question;

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            question.RegisterValueChangedCallback(evt => {
                properties.question = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
        }
    }
}
