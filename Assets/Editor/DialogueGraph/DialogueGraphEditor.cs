using Assets.Scripts.Runtime.Editor;
using Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DialogueGraph
{
    public class DialogueGraphEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        private DialogueGraphView graphView;

        [MenuItem("My tools/Dialogue graph editor")]
        public static void ShowExample()
        {
            DialogueGraphEditor wnd = GetWindow<DialogueGraphEditor>();
            wnd.titleContent = new GUIContent("Dialogue graph editor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            root.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            root.RegisterCallback<DragPerformEvent>(OnDragPerform);

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            labelFromUXML.StretchToParentSize();
            root.Add(labelFromUXML);

            // Load style sheet:
            string stylesheet_path = "Assets/Editor/DialogueGraph/styles.uss";
            StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylesheet_path);
            root.styleSheets.Add(stylesheet);

            // Load graph view:
            graphView = new DialogueGraphView(root);
            root.Q<VisualElement>("GraphContainer").Add(graphView);
        }

        public static void Open(DialogueGraphData graphData)
        {
            var window = GetWindow<DialogueGraphEditor>();
            window.graphView.OpenGraph(graphData);
            window.Show();
        }

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            if (DragAndDrop.objectReferences.Length == 1)
            {
                if (DragAndDrop.objectReferences[0] is DialogueGraphData)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    evt.StopPropagation();
                }
            }
        }

        private void OnDragPerform(DragPerformEvent evt)
        { 
            var graph = DragAndDrop.objectReferences[0] as DialogueGraphData;
            if (graph == null)
                return;

            DragAndDrop.AcceptDrag();
            if (graphView != null)
                graphView.OpenGraph(graph);
            evt.StopPropagation();
        }
    }
}
