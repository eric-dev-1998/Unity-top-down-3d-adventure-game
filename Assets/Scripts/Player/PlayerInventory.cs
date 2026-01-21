// This class was intended to manage player inventory.
// Now there is a different complete class to fo that: "InventoryManager" and its complementary
// clases. The difference is, this class managed player powers, this has to be implemented to the
// inventory manager class, probably on a separate class.

[System.Serializable]
static class PlayerInventory
{
    public static int selectedPowerLeft = -1;
    public static int selectedPowerRight = -1;
    public static bool power_wind = false;
    public static bool power_earth = false;
    public static bool power_water = false;
    public static bool power_fire = false;
    public static bool power_light = false;
    public static bool power_dark = false;

    public static bool gotFirtsCoin = false;
    public static int coins = 0;
    public static int health = 3;
    public static int keys = 0;

    public static int inventorySize = 3;
    public static int inventoryLevel = 1;

    public static InventorySlot[] slots = new InventorySlot[] { new InventorySlot(), new InventorySlot(), new InventorySlot() };

    public static bool AddItem(string item_id, int count)
    {
        return false;
    }

    public static bool AddItem(string item_id)
    {
        return false;
    }

    public static bool ConsumeItem(string item_id, int count)
    {
        // Consume an item from inventory, if it exist.

        int index = GetItemIndex(item_id);

        if (index < 0)
            return false;

        return slots[index].Remove(count); 
    }

    public static void Upgrade()
    {
        
    }

    private static int GetItemIndex(string item_id)
    {
        return -1;
    }

    public static InventorySlot FindItemByName(string name)
    {
        return null;
    }

    public static bool Owns(string id, int quantity)
    {
        return false;
    }

    public static bool BuyItem(string item_id, int quantity)
    {
        return false;
    }
}

[System.Serializable]
public class InventorySlot
{
    public int count = 0;

    public InventorySlot() { }

    public InventorySlot(int item_id, int count)
    {
        
    }

    public void Add(int count)
    { 
        this.count += count;
    }

    public bool Remove(int count)
    {
        return false;  
    }
}
