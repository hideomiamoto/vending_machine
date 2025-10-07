namespace VendingMachineApp;

public class Program
{
    public static void Main()
    {
        var inventoryBase = new Inventory();
        inventoryBase.AddNew(new ItemStack("Кола", 10, 10, 1));
        inventoryBase.AddNew(new ItemStack("Хлеб", 1, 21, 2));
        inventoryBase.AddNew(new ItemStack("Молоко", 3, 11, 3));
        
        var machine = new VendingMachine(inventoryBase);
        var ui = new UI(machine);
        ui.Run();
    }
}
