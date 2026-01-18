using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Event_system.Events
{
    public class ItemEvent : Event_System.Event
    {
        public enum ItemEventType
        {
            Get, Give, Buy, Sell
        };

        public ItemEventType type;
        public string id;
        public int count;

        public override IEnumerator Process(Event_System.EventManager eManager, Dialogue_System.Manager dManager)
        {
            if (!dManager.OnDialogue())
                yield return dManager.StartCoroutine(dManager.ShowDialogueBox());

            if(type == ItemEventType.Get)
                yield return dManager.StartCoroutine(Get(dManager));
            else if(type == ItemEventType.Give)
                yield return dManager.StartCoroutine(Give());
            else if(type == ItemEventType.Buy)
                yield return dManager.StartCoroutine(Buy(dManager));
            else if(type == ItemEventType.Sell)
                yield return dManager.StartCoroutine(Sell());

            yield return new WaitUntil(() => dManager.advance == true);

            yield return new WaitForSeconds(0.25f);
            dManager.EndItemDisplay();

            yield return new WaitForSeconds(0.25f);

            if (next != null && next.Count != 0)
                yield return dManager.StartCoroutine(base.Process(eManager, dManager));
            else
                yield return dManager.StartCoroutine(dManager.HideDialogueBox());
        }

        private IEnumerator Get(Dialogue_System.Manager dManager)
        {
            UnityEngine.Debug.Log($"[Event manager]: Item -> Get: {id}, count: {count}");

            Inventory_System.InventoryManager inventoryManager = GameObject.FindAnyObjectByType<Inventory_System.InventoryManager>();
            inventoryManager.AddItem(id, count);

            yield return dManager.StartCoroutine(dManager.DisplayItem(id));
        }

        private IEnumerator Give()
        {
            UnityEngine.Debug.Log($"[Event manager]: Item -> Give: {id}, count: {count}");

            Inventory_System.InventoryManager inventoryManager = GameObject.FindAnyObjectByType<Inventory_System.InventoryManager>();
            inventoryManager.ConsumeItem(id, count);

            yield break;
        }

        private IEnumerator Buy(Dialogue_System.Manager dManager)
        {
            UnityEngine.Debug.Log($"[Event manager]: Item -> Buy: {id}, count: {count}");

            yield return dManager.StartCoroutine(dManager.DisplayItem(id));

            Inventory_System.InventoryManager inventoryManager = GameObject.FindAnyObjectByType<Inventory_System.InventoryManager>();
            inventoryManager.AddItem(id, count);
        }

        private IEnumerator Sell()
        {
            UnityEngine.Debug.Log($"[Event manager]: Item -> Sell: {id}, count: {count}");

            Inventory_System.InventoryManager inventoryManager = GameObject.FindAnyObjectByType<Inventory_System.InventoryManager>();
            inventoryManager.ConsumeItem(id, count);

            Inventory_System.Item item = inventoryManager.itemDatabase.GetItemById(id);

            inventoryManager.money += item.item_price * count;

            yield break;
        }
    }
}
