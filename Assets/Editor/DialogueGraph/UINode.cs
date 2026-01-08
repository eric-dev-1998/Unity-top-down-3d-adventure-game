namespace Editor.DialogueGraph
{
    public class UINode : UnityEditor.Experimental.GraphView.Node
    {
        public string id;
        public UINode(Node node) 
        {
            id = node.id;
            node.node_data = this;
        }
    }
}
