using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Event_System.Events;

namespace Assets.Scripts.Runtime.Editor.Node_properties
{
    public class ItemProperties : NodeProperties
    {
        public ItemEvent.ItemEventType eventType;
        public string id = "";
        public int count = 0;

        public ItemProperties() { }

        public ItemProperties(ItemEvent.ItemEventType eventType, string id, int count)
        {
            this.eventType = eventType;
            this.id = id;
            this.count = count;
        }
    }
}
