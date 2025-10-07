namespace VendingMachineApp;
public class ItemStack
{
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public int Price { get; private set; }
    public int Id { get; }

    public ItemStack(string name, int quantity, int price, int id)
    {
        Name = name;
        Id = id;
        Price = price >= 0 ? price : 0;
        Quantity = quantity >= 0 ? quantity : 0;
    }

    public bool SetPrice(int price)
    {
        if (price >= 0)
        {
            Price = price;
            return true;
        }
        return false;
    }

    public bool SetName(string name)
    {
        Name = name;
        return true;
    }

    public bool Increase(int amount = 1)
    {
        if (amount <= 0)
            return false;

        Quantity += amount;
        return true;
    }

    public bool Decrease(int amount = 1)
    {
        if (amount <= 0)
            return false;

        if (Quantity - amount < 0)
            return false;

        Quantity -= amount;
        return true;
    }
}
