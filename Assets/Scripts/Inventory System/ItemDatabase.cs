using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Inventory_System
{
    public class ItemDatabase
    {
        private Dictionary<string, Item> item_table;

        public ItemDatabase() 
        {
            // Instantiate dictionary:
            item_table = new Dictionary<string, Item>();

            // Fill dictionary:
            Item[] items = Resources.LoadAll<Item>("Items/");
            foreach (Item item in items)
            {
                item_table.Add(item.item_id, item);
            }
        }

        public Item GetItemById(string item_id)
        {
            Item item = item_table[item_id];
            if (item != null)
                return item;

            return null;
        }
    }
}
