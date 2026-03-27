using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Runtime.Editor.Node_properties
{
    public class ItemOwnershipProperties : NodeProperties
    {
        public string itemId = "";
        public int count = 0;

        public ItemOwnershipProperties() { }

        public ItemOwnershipProperties(string itemId, int count)
        {
            this.itemId = itemId;
            this.count = count;
        }
    }
}
