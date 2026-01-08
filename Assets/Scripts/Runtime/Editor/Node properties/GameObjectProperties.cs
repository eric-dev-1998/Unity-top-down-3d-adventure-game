using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Event_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Runtime.Editor.Node_properties
{
    public class GameObjectProperties : NodeProperties
    {
        public GameObjectEvent.EventType type;
        public new string name;

        public GameObjectProperties() { }

        public GameObjectProperties(string name, GameObjectEvent.EventType type)
        {
            this.name = name;
            this.type = type;
        }
    }
}
