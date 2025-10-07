namespace VendingMachineApp;

public class VendingMachine
{
    private Inventory inventory = new Inventory();
    private CashManager _cashManager = new CashManager();

    public VendingMachine() {}
    public VendingMachine(Inventory inv)
    {
        inventory = inv;
    }

    public bool Buy(int id) => BuyDetailed(id).ok;
    
    public (bool ok, string reason, Dictionary<int,int>? change) BuyDetailed(int id)
    {
        var item = inventory.Find(id);
        if (item == null) return (false, "A", null);
        if (item.Quantity <= 0) return (false, "B", null);

        int price = item.Price;
        int paid  = _cashManager.DepositTotalRub();
        if (paid < price) return (false, "C", null);

        int changeRub = paid - price;
        if (!_cashManager.TryPlanChange(changeRub, out var plan))
            return (false, "D", null);
        
        if (!inventory.Take(id))
            return (false, "B", null);
        
        _cashManager.CommitPurchase(plan);
        revenue += item.Price;
        return (true, "", plan);
    }


    public List<ItemStack> ListAll()
    {
        return inventory.ListAll();
    }
    
    public bool AcceptCoin(int coin)
    {
        if(_cashManager.AcceptCoin(coin)) return true;
        return false;
    }
    
    public int DepositTotalRub() => _cashManager.DepositTotalRub();
    public Dictionary<int,int> ReturnDeposit() => _cashManager.ReturnDeposit();
    
    private int revenue = 0;
    
    public bool AdminRestock(int id, int quantity) => inventory.Restock(id, quantity);
    public bool AdminAddCoins(int denom, int quantity) => _cashManager.AddCoins(denom, quantity);

    public bool AdminAddItem(int id, string name, int price, int quantity)
    {
        if (price < 0 || quantity < 0) return false;
        return inventory.AddNew(new ItemStack(name.Trim(), quantity, price, id));
    }
    public int GetRevenue() => revenue;

    public int CollectRevenue()
    {
        var r = revenue;
        revenue = 0;
        return r;
    }
    

    public Dictionary<int,int> CashboxSnapshot() => _cashManager.SnapshotCashbox();
};
