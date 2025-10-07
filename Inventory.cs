namespace VendingMachineApp;
public class Inventory
{
    private Dictionary<int, ItemStack> items;

    public Inventory()
    {
        items = new Dictionary<int, ItemStack>();
    }

    public bool AddNew(ItemStack item)
    {
        if (items.ContainsKey(item.Id))
        {
            return false;   
        }
        items.Add(item.Id, item);
        return true;
    }
    

    public ItemStack Find(int id)
    {
        if (items.ContainsKey(id))
            return items[id];
        return null;
    }

    public bool Take(int id, int quantity = 1)
    {
        if (quantity <= 0) return false;
        
        ItemStack stack = Find(id);
        if (stack == null) return false;
        
        bool success = stack.Decrease(quantity);
        if (!success) return false;
        
        if (stack.Quantity == 0) items.Remove(id);

        return true;
    }
    
    public bool Restock(int id, int quantity)
    {
        if (quantity <= 0) return false;
        var item = Find(id);
        if (item == null) return false;
        return item.Increase(quantity);
    }
    
    public List<ItemStack> ListAll()
    {
        return items.Values.ToList();
    }
}
