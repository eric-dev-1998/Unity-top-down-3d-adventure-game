using Assets.Scripts.Event_system.Events;

namespace Assets.Scripts.Runtime.Editor.Node_properties
{
    public class QuestProperties : NodeProperties
    {
        public string id = "";
        public QuestSet.QuestState state;

        public QuestProperties() { }
    }
}
