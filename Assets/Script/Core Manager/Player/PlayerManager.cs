using System;
public class PlayerManager
{
    // reference to the Player instance
    // This class manages player actions such as healing, attacking pathogens, and drawing cards.

    private Player player;

    public PlayerManager(Player p)
    {
        player = p;
    }

    public void HealPlayer(int amount)
    {
        player.Heal(amount);
    }

    public void Attack(PathogenSO pathogen)
    {
        // Use cards or logic to attack the pathogen
        pathogen.TakeDamage(10); // Example attack logic
    }

    public void DrawCard(DeckManager deck)
    {
        CardSO drawnCard = deck.DrawCard();
        Console.WriteLine($"Player drew a card: {drawnCard.cardName}");
    }
}
