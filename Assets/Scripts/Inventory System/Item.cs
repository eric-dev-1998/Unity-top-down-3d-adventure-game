using UnityEngine;

namespace Assets.Scripts.Inventory_System
{
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory system/Item")]
    public class Item : ScriptableObject
    {
        public string item_id;
        public int item_price;
        public GameObject item_display;
    }
}
