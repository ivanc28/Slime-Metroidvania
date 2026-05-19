using UnityEngine;

public class PlayerCurrency
{
    private int currency;
    private bool CanPurchase(int cost)
    {
        return currency <= cost;
    }
    /// <summary>
    /// Attempts to purchase item and subtracts cost from currency. Returns true if successful, otherwise returns false
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    public bool OnPurchase(int cost)
    {
        if (CanPurchase(cost))
        {
            currency -= cost;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Increases currency by amount
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseCurrency(int amount)
    {
        currency += amount;
    }
}
