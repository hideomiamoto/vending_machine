namespace VendingMachineApp;

public class CashManager
{
    private Dictionary<int, int> cashbox;
    private Dictionary<int, int> deposit;
    private static readonly int[] Denoms = { 10, 5, 2, 1 };
    public CashManager()
    {
        cashbox = new Dictionary<int, int>();
        deposit = new Dictionary<int, int>();
        foreach (int denom in Denoms)
        {
            cashbox[denom] = 0;   
            deposit[denom] = 0;   
        }
    }

    public bool AcceptCoin(int denom)
    {   
        if (!Denoms.Contains(denom)) return false;
        deposit[denom]++;
        return true;
    }

    public Dictionary<int, int> ReturnDeposit()
    {   
        var returnDep = new Dictionary<int, int>();
        foreach (int denom in Denoms)
        {   
            returnDep[denom] = deposit[denom];
            deposit[denom] = 0;   
        }
        return returnDep;
    }

    public int DepositTotalRub()
    {
        int total = 0;
        foreach (int denom in Denoms)
        {
            total += (denom*deposit[denom]);   
        }
        return total;
    }
    
    public bool TryPlanChange(int changeRub, out Dictionary<int,int> plan)
    {
        plan = new Dictionary<int, int>();
        if (changeRub == 0) return true;
        
        var virtualBank = new Dictionary<int,int>();
        foreach (var d in Denoms)
            virtualBank[d] = cashbox[d] + deposit[d];
        
        foreach (var d in Denoms)
        {
            if (changeRub <= 0) break;
            int need = changeRub / d;
            if (need <= 0) continue;

            int can = Math.Min(need, virtualBank[d]);
            if (can > 0)
            {
                plan[d] = can;
                changeRub -= can * d;
                virtualBank[d] -= can;
            }
        }
        return changeRub == 0;
    }

    public void CommitPurchase(Dictionary<int,int> plan)
    {
        foreach (var d in Denoms)
            cashbox[d] += deposit[d];
        
        foreach (var k in plan)
            cashbox[k.Key] -= k.Value;
        
        foreach (var d in Denoms)
            deposit[d] = 0;
    }
    
    public bool AddCoins(int denom, int quantity)
    {
        if (!Denoms.Contains(denom) || quantity <= 0) return false;
        cashbox[denom] += quantity;
        return true;
    }

    public Dictionary<int,int> SnapshotCashbox()
    {
        return new Dictionary<int, int>(cashbox);
    }


}
