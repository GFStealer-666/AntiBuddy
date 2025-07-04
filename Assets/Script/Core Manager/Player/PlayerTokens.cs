using System;

public class PlayerTokens
{
    public int Tokens { get; private set; }

    public event Action<int> OnTokensChanged;

    public PlayerTokens()
    {
        Tokens = 0;
    }

    public void AddTokens(int amount)
    {
        Tokens += amount;
        OnTokensChanged?.Invoke(Tokens);
        Console.WriteLine($"Player received {amount} tokens. Current tokens: {Tokens}");
    }

    public bool SpendTokens(int amount)
    {
        if (Tokens >= amount)
        {
            Tokens -= amount;
            OnTokensChanged?.Invoke(Tokens);
            Console.WriteLine($"Player spent {amount} tokens. Remaining tokens: {Tokens}");
            return true;
        }
        Console.WriteLine($"Not enough tokens! Required: {amount}, Available: {Tokens}");
        return false;
    }

    public bool CanAfford(int cost)
    {
        return Tokens >= cost;
    }
}
