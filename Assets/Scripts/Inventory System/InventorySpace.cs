using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Inventory_System
{
    public class InventorySpace
    {
        public Item data;
        public int count = 0;

        public InventorySpace(Item itemData) 
        {
            data = itemData;
        }

        public InventorySpace(Item itemData, int count)
        {
            data = itemData;
            this.count = count;
        }
    }
}
