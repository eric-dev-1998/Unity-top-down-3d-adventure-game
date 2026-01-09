using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Inventory_System
{
    public class Manager : MonoBehaviour
    {
        private Quest_System.QuestManager questManager;

        // Logic properties:
        public bool open = false;
        private MainPlayer player;
        private Animator animator;
        private Animator animator_blackscreen;

        public int money = 0;

        public ItemDatabase itemDatabase;
        private List<Inventory_System.InventorySpace> spaces;
        private int inventorySpace = 5;

        private void Start()
        {
            questManager = FindAnyObjectByType<Quest_System.QuestManager>();

            itemDatabase = new ItemDatabase();
            animator = GetComponent<Animator>();
            player = FindFirstObjectByType<MainPlayer>();

            spaces = new List<InventorySpace>();
        }

        private void Update()
        {
            bool result;

            if (Input.GetKeyDown(KeyCode.Keypad0))
                result = AddItem("key_small", 1);
            else if (Input.GetKeyDown(KeyCode.Keypad1))
                result = AddItem("key_boss", 1);
            else if (Input.GetKeyDown(KeyCode.Keypad2))
                result = AddItem("negumi_cachete", 1);
        }

        public bool AddItem(string id, int count)
        {
            // Find item data in database:
            Item item = itemDatabase.GetItemById(id);
            if (item == null)
                return false;

            InventorySpace targetSpace = null;
            foreach (InventorySpace space in spaces)
            {
                if (space.data.item_id == item.item_id)
                    targetSpace = space;
            }

            if (targetSpace != null)
            {
                // Just add to counter if a space for the item already exist.
                targetSpace.count += count;

                // Tell the quest manager an item was collected.
                questManager.ItemInventoryChanged(item, targetSpace.count);

                return true;
            }
            else
            {
                if (spaces.Count < inventorySpace)
                {
                    // Create a space for this item if there is room available.
                    InventorySpace space = new InventorySpace(item, count);
                    spaces.Add(space);

                    // Tell the quest manager an item was collected.
                    questManager.ItemInventoryChanged(item, space.count);

                    return true;
                }
                else
                    // Return false if there is no space to add this item.
                    return false;
            }
        }

        public bool ConsumeItem(string id, int count)
        {
            InventorySpace space = spaces.Find(s => s.data.item_id == id);
            if (space == null) 
                return false;

            if (space.count < count)
                return false;

            space.count -= count;

            // Tell quest manager an item has been consumed:
            questManager.ItemInventoryChanged(space.data, space.count);

            if (space.count <= 0)
                spaces.Remove(space);

            return true;
        }

        public int GetItemCount(string id)
        {
            InventorySpace space = spaces.Find(s => s.data.item_id == id);
            if (space == null)
                return -1;

            return space.count;
        }

        public bool CanPlayerBuyItem(string id)
        { 
            Item itemData = itemDatabase.GetItemById(id);
            if (itemData == null)
                return false;

            if (itemData.item_price > money)
                return false;

            return true;
        }

        public List<InventorySpace> GetInventory()
        {
            return spaces;
        }
    }
}
