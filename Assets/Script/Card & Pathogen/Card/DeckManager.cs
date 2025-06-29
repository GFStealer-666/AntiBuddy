using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    // Public list to allow assignment of CardSO ScriptableObjects in Unity Inspector
    public List<CardSO> deck;

    // Random number generator for card drawing
    private System.Random rand;

    void Start()
    {
        rand = new System.Random();

        // Optionally, you can shuffle the deck when the game starts
        ShuffleDeck();
    }

    // Draw a card from the deck
    public CardSO DrawCard()
    {
        if (deck.Count == 0) return null;
        int index = rand.Next(deck.Count);
        CardSO drawnCard = deck[index];
        deck.RemoveAt(index);  // Remove the drawn card from the deck
        Debug.Log($"Card drawn: {drawnCard.cardName}");
        return drawnCard;
    }

    // Shuffle the deck (randomize order of cards)
    public void ShuffleDeck()
    {
        deck = new List<CardSO>(deck.OrderBy(x => rand.Next()).ToList());
        Debug.Log("Deck shuffled!");
    }
}

