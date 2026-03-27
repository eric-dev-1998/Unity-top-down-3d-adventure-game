using Assets.Scripts.Dialogue_System;
using Assets.Scripts.Event_System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Event_system.Events
{
    public class ItemOwnership : Event
    {
        public string itemId = "";
        public int count = 0;

        public ItemOwnership() { }

        public ItemOwnership(string itemId, int count) 
        {
            this.itemId = itemId;
            this.count = count;
        }

        public override IEnumerator Process(EventManager eManager, Manager dManager)
        {
            bool owned = false;

            if (count <= 0)
            {
                // No count specified, so check that player has at least 1 unit on the inventory.
                owned = eManager.inventoryManager.GetItemCount(itemId) >= 1;
            }
            else
            { 
                owned = eManager.inventoryManager.GetItemCount(itemId) >= count;
            }

            if (next.Count >= 2 && (next[0] != null && next[1] != null))
            {
                if (owned)
                    yield return next[1].Process(eManager, dManager);
                else
                    yield return next[0].Process(eManager, dManager);
            }
        }
    }
}
