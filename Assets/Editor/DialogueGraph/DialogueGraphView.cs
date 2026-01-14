using Assets.Editor.DialogueGraph.Nodes;
using Assets.Scripts.Event_System;
using Assets.Scripts.GameText;
using Assets.Scripts.Runtime.Editor;
using Editor.DialogueGraph.Nodes;
using Editor.DialogueGraph.Serialization;
using EventSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DialogueGraph
{
    class Clipboard
    {
        public List<SerializedNode> nodes;
    }

    public class DialogueGraphView : GraphView
    {
        public TextLibrary library;

        private VisualElement root;
        private Label label_ProjectName;
        private Button button_open;
        private Button button_build;
        private TextField textSaveCommand;

        private bool saveValidated;

        // Serialzation:
        public DialogueGraphData currentGraphData;
        public List<Node> runtime_nodes = new List<Node>();
        public List<Connection> runtime_connections = new List<Connection>();

        public DialogueGraphView(VisualElement root)
        {
            library = Resources.Load<TextLibrary>("GameText/English");
            if (library == null)
                Debug.LogError("[Dialogue graph editor]: No text library was found.");

            this.StretchToParentSize();
            this.AddToClassList("graph-view");
            this.root = root;

            // Manipulators:
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            graphViewChanged += OnGraphViewChanged;

            // Copy and paste callbacks:
            serializeGraphElements = Copy;
            canPasteSerializedData = Validate;
            unserializeAndPaste = Paste;

            LoadUIElements();

            // Refresh graph view.
            //RefreshGraphView();
        }

        private void LoadUIElements()
        {
            label_ProjectName = root.Q<Label>("Label_ProjectName");

            button_open = root.Q<Button>("Button_Open");
            button_build = root.Q<Button>("Button_Build");

            button_build.SetEnabled(false);

            button_open.clicked += OpenProject;
            button_build.clicked += BuildProject;

            textSaveCommand = root.Q<TextField>("SaveCommand");
            textSaveCommand.SetEnabled(false);

            textSaveCommand.RegisterValueChangedCallback((evt) => 
            {
                if (string.IsNullOrEmpty(evt.newValue))
                {
                    button_build.SetEnabled(true);
                    textSaveCommand.RemoveFromClassList("text-valid");
                    textSaveCommand.RemoveFromClassList("text-not-valid");
                    return;
                }

                Debug.Log("Validating...");

                if (!ValidateSaveCommand())
                {
                    saveValidated = false;
                    textSaveCommand.AddToClassList("text-not-valid");
                    textSaveCommand.RemoveFromClassList("text-valid");
                    button_build.SetEnabled(false);
                }
                else
                {
                    saveValidated = true;
                    textSaveCommand.AddToClassList("text-valid");
                    textSaveCommand.RemoveFromClassList("text-not-valid");
                    button_build.SetEnabled(true);
                }
            });
        }

        public void OpenGraph(DialogueGraphData graphData)
        {
            if (currentGraphData != null)
            {
                if (currentGraphData == graphData)
                    return;

                bool accept = EditorUtility.DisplayDialog(
                    "Open graph",
                    "You are about to open another graph. Would you like to proceed?",
                    "Yes",
                    "Cancel");

                if (!accept)
                    return;

                // Clear visual nodes and connections:
                DeleteElements(graphElements.ToList());
                runtime_nodes.Clear();
                runtime_connections.Clear();
            }

            // Load nodes and connections:
            currentGraphData = graphData;

            foreach (NodeSaveData saveData in currentGraphData.nodes)
            {
                CreateNode(saveData);
            }

            foreach (ConnectionSaveData connection in currentGraphData.connections)
            {
                CreateConnection(connection);
            }

            label_ProjectName.text = "Graph loaded - " + currentGraphData.name;

            button_build.SetEnabled(true);
            textSaveCommand.SetEnabled(true);
        }

        private void OpenProject()
        {
            // Clear current graph visual content.
            DeleteElements(graphElements.ToList());

            // Get graph project asset path:
            string path = EditorUtility.OpenFilePanel("Open graph project...", Application.dataPath + "/Resources", "asset");
            if (string.IsNullOrEmpty(path))
            {
                // Return if no path was selected.
                Debug.Log("[Dialogue graph editor]: Graph project load operation canceled.");
                return;
            }

            // Prepare the selected path to load the graph asset:
            path = FileUtil.GetProjectRelativePath(path);

            // Load the graph data from the path:
            var graphData = AssetDatabase.LoadAssetAtPath<DialogueGraphData>(path);

            if (graphData is not DialogueGraphData)
            {
                // The loaded asset from the path is not a dialogue graph, notify and cancel the load operation.
                EditorUtility.DisplayDialog("Error", $"The selected asset at '{path}' could not be recognized as a graph project asset.", "Ok");
                return;
            }

            currentGraphData = graphData;

            foreach (NodeSaveData saveData in currentGraphData.nodes)
            { 
                CreateNode(saveData);
            }

            foreach (ConnectionSaveData connection in currentGraphData.connections)
            {
                CreateConnection(connection);
            }

            label_ProjectName.text = "Graph loaded - " + currentGraphData.name;

            button_build.SetEnabled(true);
            textSaveCommand.SetEnabled(true);
        }

        private bool ValidateSaveCommand()
        {
            string[] command = textSaveCommand.text.Split(',');

            string header = command[0];

            switch (header)
            {
                case "npc":
                    // It's a npc standard dialogue.
                    if (command.Count() == 2 || command.Count() == 3)
                        return true;
                    else
                        return false;

                case "npcq":
                    // It's a npc quest dialogue.
                    if (command.Count() == 4)
                        return true;
                    else
                        return false;

                case "cs":
                    // It's a cutscene event.
                    if(command.Count() == 3 || command.Count() == 2)
                        return true;
                    else
                        return false;

                default:
                    return false;
            }
        }

        private void BuildProject()
        {
            EventSequence outputSequence = ScriptableObject.CreateInstance<EventSequence>();

            string path = "";
            string primaryDir = "";
            string secondaryDir = "";

            // Read save command:
            string saveCommand = textSaveCommand.text;

            if (string.IsNullOrEmpty(saveCommand))
            {
                // Open a save file panel to manually pick a place to save
                // the output event sequence if no save command was entered.

                path = EditorUtility.SaveFilePanel("Build event sequence as...",
                    Application.dataPath + "/Resources", "new_event_sequence", ".asset");
            }
            else
            {
                string[] command = saveCommand.Split(',');

                string header = command[0];
                int ce = 0;

                switch (header)
                {
                    case "npc":

                        // It's a npc standard dialogue.

                        primaryDir = $"Assets/Resources/GameText/Dialogues/{command[1]}";
                        secondaryDir = $"Assets/Resources/GameText/Dialogues/{command[1]}/Standard";

                        if (!AssetDatabase.IsValidFolder(primaryDir))
                            AssetDatabase.CreateFolder("Assets/Resources/GameText/Dialogues", command[1]);

                        if (!AssetDatabase.IsValidFolder(secondaryDir))
                            AssetDatabase.CreateFolder($"Assets/Resources/GameText/Dialogues/{command[1]}", "Standard");

                        if (command.Count() == 3)
                            path = $"Assets/Resources/GameText/Dialogues/{command[1]}/Standard/{command[2]}.asset";
                        else
                        {
                            ce = Resources.LoadAll<EventSequence>($"GameText/Dialogues/{command[1]}").Count();
                            path = $"Assets/Resources/GameText/Dialogues/{command[1]}/Standard/standard_{ce}.asset";
                        }
                        break;

                    case "npcq":

                        // It's a npc quest dialogue.

                        primaryDir = $"Assets/Resources/GameText/Dialogues/{command[1]}";
                        secondaryDir = $"Assets/Resources/GameText/Dialogues/{command[1]}/{command[2]}";

                        if (!AssetDatabase.IsValidFolder(primaryDir))
                            AssetDatabase.CreateFolder("Assets/Resources/GameText/Dialogues", command[1]);

                        if (!AssetDatabase.IsValidFolder(secondaryDir))
                            AssetDatabase.CreateFolder($"Assets/Resources/GameText/Dialogues/{command[1]}", command[2]);

                        path = $"Assets/Resources/GameText/Dialogues/{command[1]}/{command[2]}/{command[3]}.asset";
                        break;

                    case "cs":

                        // It's a cutscene event.

                        primaryDir = $"Assets/Resources/GameText/Cutscenes/{command[1]}";

                        if (!AssetDatabase.IsValidFolder(primaryDir))
                            AssetDatabase.CreateFolder("Assets/Resources/GameText/Cutscenes", command[1]);

                        if (command.Count() == 3)
                            path = $"Assets/Resources/GameText/Cutscenes/{command[1]}/{command[2]}.asset";
                        else 
                        {
                            ce = Resources.LoadAll<EventSequence>("GameText/Cutscenes/{command[1]}").Count();
                            path = $"Assets/Resources/GameText/Cutscenes/{command[1]}/cutscene_{ce}.asset";
                        }
                        break;
                }
            }

            // Save resulting sequence to assets:
            //path = Application.dataPath + path;
            AssetDatabase.CreateAsset(outputSequence, path);
            AssetDatabase.Refresh();

            // Convert nodes to events:
            Dictionary<string, Assets.Scripts.Event_System.Event> eventMap = new();

            foreach (var node in currentGraphData.nodes)
            {
                // Do conversion:

                var evt = EventFactory.CreateEvent(node);

                if (evt == null)
                    throw new Exception("The event could not be created.");

                AssetDatabase.AddObjectToAsset(evt, outputSequence);
                eventMap[node.id] = evt;
                outputSequence.events.Add(evt);
            }

            foreach (var connection in currentGraphData.connections)
            {
                // Read and assign connections between events:

                Assets.Scripts.Event_System.Event from;
                Assets.Scripts.Event_System.Event to;

                if (!eventMap.TryGetValue(connection.outputNodeId, out from))
                {
                    Debug.Log($"Missing target event node for node id: {connection.outputNodeId}.");
                    continue;
                }

                if (!eventMap.TryGetValue(connection.inputNodeId, out to))
                {
                    Debug.Log($"Missing target event node for node id: {connection.inputNodeId}.");
                    continue;
                }

                // Here is where i get the null exception:
                if(from.next == null)
                    from.next = new List<Assets.Scripts.Event_System.Event>();

                from.next.Add(to);
            }

            // Find first node to start sequence from it:
            HashSet<string> nodesWithInput = new();
            foreach (var conneciton in currentGraphData.connections)
                nodesWithInput.Add(conneciton.inputNodeId);

            NodeSaveData startNode = currentGraphData.nodes.First(n => !nodesWithInput.Contains(n.id));
            outputSequence.startEvent = eventMap[startNode.id];

            AssetDatabase.SaveAssets();

            Debug.Log("[Dialogue graph editor]: Graph built succesfully.");
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach((port) =>
            {
                if (startPort != port &&
                    startPort.node != port.node &&
                    startPort.direction != port.direction &&
                    startPort.portType == port.portType)
                {
                    compatiblePorts.Add(port);
                }
            });
            return compatiblePorts;
        }

        private void CreateConnection(ConnectionSaveData connection)
        {
            Node input = runtime_nodes.Find(n => n.saveData.id == connection.inputNodeId);
            Node output = runtime_nodes.Find(n => n.saveData.id == connection.outputNodeId);

            Port inputPort = input.GetInputPortByName(connection.inputPortName);
            Port outputPort = output.GetOutputPortByName(connection.outputPortName);

            Edge newConnection = outputPort.ConnectTo(inputPort);

            AddElement(newConnection);
        }

        private void CreateNode(NodeSaveData saveData)
        {
            Type nodeType = Type.GetType(saveData.typeName);
            Node node = (Node)Activator.CreateInstance(nodeType);
            node.parent = this;
            node.saveData = saveData;

            node.CreateNode();
            node.LoadVisualContent();
            node.LoadProperties();

            runtime_nodes.Add(node);
        }

        private void CreateNode(Type nodeType, Vector3 position)
        {
            if (currentGraphData == null)
            {
                // Return if no graph asset is loaded.

                EditorUtility.DisplayDialog("No graph asset loaded.", "No graph asset has been loaded yet, nodes cannot be added.", "Ok");
                return;
            }

            // Create node asset instance:
            Node node = (Node)Activator.CreateInstance(nodeType);
            node.parent = this;
            node.id = Guid.NewGuid().ToString();
            node.screenPosition = position;

            // Create node save data:
            node.saveData = ScriptableObject.CreateInstance<NodeSaveData>();
            node.saveData.id = node.id;
            node.saveData.position = position;

            // Add current node to graph asset.
            AssetDatabase.AddObjectToAsset(node.saveData, currentGraphData);
            currentGraphData.nodes.Add(node.saveData);

            // Create visual node:
            node.CreateNode();
            node.LoadVisualContent();
            node.CreateProperties();

            EditorUtility.SetDirty(node.saveData);
            EditorUtility.SetDirty(node.saveData.properties);
            EditorUtility.SetDirty(currentGraphData);

            runtime_nodes.Add(node);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            // Read mouse position to place the new node.
            Vector2 mousePosition = evt.localMousePosition;
            mousePosition = viewTransform.matrix.inverse.MultiplyPoint(mousePosition);

            // Contextual menu options:
            evt.menu.AppendAction("Add node/Dialogue/Line", (a) => { CreateNode(typeof(Line), mousePosition); }, DropdownMenuAction.Status.Normal);
            evt.menu.AppendAction("Add node/Dialogue/Multiline", (a) => { CreateNode(typeof(Multiline), mousePosition); }, DropdownMenuAction.Status.Normal);
            evt.menu.AppendAction("Add node/Dialogue/Question", (a) => { CreateNode(typeof(Question), mousePosition); }, DropdownMenuAction.Status.Normal);
            evt.menu.AppendAction("Add node/Player/Item", (a) => { CreateNode(typeof(Item), mousePosition); }, DropdownMenuAction.Status.Normal);
            evt.menu.AppendAction("Add node/GameObject", (a) => { CreateNode(typeof(Assets.Editor.DialogueGraph.Nodes.GameObject), mousePosition); }, DropdownMenuAction.Status.Normal);
            evt.menu.AppendAction("Add node/Quest/Set state", (a) => { CreateNode(typeof(QuestSetNode), mousePosition); }, DropdownMenuAction.Status.Normal);
            evt.menu.AppendAction("Add node/Quest/Get state", (a) => { CreateNode(typeof(QuestState), mousePosition); }, DropdownMenuAction.Status.Normal);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // Moved elements:
            if (graphViewChange.movedElements != null)
            {
                foreach (GraphElement element in graphViewChange.movedElements)
                {
                    if (element is UnityEditor.Experimental.GraphView.Node node)
                    {
                        Node runtimeNode = runtime_nodes.Find(n => n.node_data == node);

                        if (runtimeNode == null)
                            Debug.LogError("Node not found.");
                        else
                        {
                            runtimeNode.saveData.position = element.GetPosition().position;
                            EditorUtility.SetDirty(runtimeNode.saveData);
                        }
                    }
                }
            }

            // Connections:
            if (graphViewChange.edgesToCreate != null)
            {
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    //Node outputNode = runtime_nodes.Find(n => n.GetOutputPort() == edge.output);
                    Node outputNode = runtime_nodes.Find(n => n.node_data == edge.output.node);                    
                    Node inputNode = runtime_nodes.Find(n => n.GetInputPort() == edge.input);

                    if (outputNode == null)
                        Debug.LogError("Connection output node is null.");
                    if (inputNode == null)
                        Debug.LogError("Connection input node is null.");

                    if (outputNode != null && inputNode != null)
                    {
                        // Add a new connection data to local list:
                        ConnectionSaveData connection = ScriptableObject.CreateInstance<ConnectionSaveData>();
                        connection.outputNodeId = outputNode.saveData.id;
                        connection.inputNodeId = inputNode.saveData.id;
                        connection.outputPortName = edge.output.portName;
                        connection.inputPortName = edge.input.portName;

                        AssetDatabase.AddObjectToAsset(connection, currentGraphData);
                        currentGraphData.connections.Add(connection);

                        EditorUtility.SetDirty(connection);
                        EditorUtility.SetDirty(currentGraphData);

                        Debug.Log("Edge created.");
                    }
                }
            }

            // Removing elements:
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement element in graphViewChange.elementsToRemove)
                {
                    if (element is UnityEditor.Experimental.GraphView.Node node)
                    {
                        Node nodeToRemove = runtime_nodes.Find(n => n.node_data == node);

                        NodeSaveData saveData = currentGraphData.nodes.Find(n => n.id == nodeToRemove.saveData.id);

                        if(currentGraphData != null)
                            currentGraphData.nodes.Remove(saveData);
                        runtime_nodes.Remove(nodeToRemove);
                    }

                    // Removing an edge:
                    if (element is UnityEditor.Experimental.GraphView.Edge edge)
                    {
                        // Get edge visual nodes.
                        UnityEditor.Experimental.GraphView.Node output = edge.output.node;
                        UnityEditor.Experimental.GraphView.Node input = edge.input.node;

                        // Get nodes data.
                        Node node_from = runtime_nodes.Find(n => n.node_data == output);
                        Node node_to = runtime_nodes.Find(n => n.node_data == input);

                        // Look for matching connection in local references and remove it:
                        Connection connection = runtime_connections.Find(c => c.from.id == node_from.id && c.to.id == node_to.id);
                        ConnectionSaveData saveData = currentGraphData.connections.Find(c => c.outputNodeId == node_from.saveData.id && c.inputNodeId == node_to.saveData.id);

                        currentGraphData.connections.Remove(saveData);
                        runtime_connections.Remove(connection);
                    }
                }
            }

            return graphViewChange;
        }

        private String Copy(IEnumerable<GraphElement> elements)
        {
            Clipboard clipboard = new Clipboard();
            clipboard.nodes = new List<SerializedNode>();

            // Add nodes to the list to copy.
            foreach (GraphElement element in elements)
            {
                if (element is UnityEditor.Experimental.GraphView.Node node)
                {
                    foreach (Node node_instance in runtime_nodes)
                    {
                        if (node_instance.node_data == node)
                        {
                            SerializedNode sn = new SerializedNode();
                            sn.type = node_instance.GetType().AssemblyQualifiedName;
                            sn.json = JsonUtility.ToJson(node_instance);
                            clipboard.nodes.Add(sn);
                        }
                    }
                }
            }

            return JsonUtility.ToJson(clipboard);
        }

        private bool Validate(string data)
        {
            // Return if no data was collected.
            if (string.IsNullOrEmpty(data))
                return false;

            try
            {
                Clipboard clipboard = JsonUtility.FromJson<Clipboard>(data);

                if (clipboard.nodes.Count > 0)
                {
                    foreach (SerializedNode node in clipboard.nodes)
                    {
                        if (node is not SerializedNode)
                        {
                            // A non valid or corrupted node instance was found.
                            // return false to abort operation.
                            return false;
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Paste(string operationName, string data)
        {
            if (operationName == "Paste")
            {
                Clipboard clipboard = JsonUtility.FromJson<Clipboard>(data);
                ClearSelection();

                foreach (SerializedNode sn in clipboard.nodes)
                {
                    Type nodeType = Type.GetType(sn.type);
                    Node node = (Node)JsonUtility.FromJson(sn.json, nodeType);
                    CreateNode(nodeType, node.saveData.position * 1.15f);
                }
            }
        }
    }
}
