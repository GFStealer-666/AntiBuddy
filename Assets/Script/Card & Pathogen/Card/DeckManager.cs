using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// DeckManager manages a collection of CardSO ScriptableObjects, allowing for drawing and shuffling of cards
// It provides functionality to draw a random card from the deck and shuffle the deck
[System.Serializable]
public class DeckManager : MonoBehaviour
{
    #region Deck Configuration
    [Header("Deck Configuration")]
    [Tooltip("Base cards to use for generating the full deck")]
    public List<CardSO> baseCards = new List<CardSO>();

    [Header("Deck Size")]
    [Tooltip("Total number of cards in the deck, can be adjusted based on player desire")]
    public int totalDeckSize = 50;

    [Tooltip("The actual deck used for drawing (generated from baseCards)")]
    [SerializeField] private List<CardSO> deck = new List<CardSO>();
    
    private Dictionary<string, int> deckComposition = new Dictionary<string, int>();

    [Tooltip("If set to true, every time the gard is drawn, will shuffle the deck")]
    [SerializeField] private bool shuffleOnDraw = false;

    [Header("Advanced Deck Generation (Optional)")]
    [Tooltip("If set to true, uses weighted generation based on percentages of each card type")]
    public bool useWeightedGeneration = false;
    [Tooltip("List of cards with their respective weights for deck generation")]
    [SerializeField]
    [System.Serializable]
    
    public class WeightedCard
    {
        public CardSO card;
        [Range(0.01f, 1.0f)]
        [Tooltip("Percentage of deck (multiple by 100 to get percentage), It will affect the number of cards generated based on totalDeckSize")]
        public float weightPercentage = 0.2f; // What percentage of the deck this card should occupy
    }
    [Tooltip("List of cards with their respective weight percentages for deck generation")]
    public List<WeightedCard> weightedCards = new List<WeightedCard>();

    // Random number generator for shuffling and drawing cards
    // Using System.Random for better performance in Unity
    private readonly System.Random rand = new System.Random();
    #endregion
    private void Start()
    {
        GenerateDeck();
        ShuffleDeck();
    }

    #region Deck Generation
    // Generate a full deck from the base cards
    private void GenerateDeck()
    {
        // Choose generation method based on settings
        if (useWeightedGeneration)
        {
            GenerateWeightedDeck();
            return;
        }

        if (baseCards.Count == 0)
        {
            Debug.LogWarning("No base cards assigned to DeckManager!");
            return;
        }

        // Clear existing deck
        deck.Clear();

        // Generate random cards up to totalDeckSize
        for (int i = 0; i < totalDeckSize; i++)
        {
            // Pick a random base card and add a copy to the deck
            int randomIndex = rand.Next(baseCards.Count);
            deck.Add(baseCards[randomIndex]);
        }
        GenerateDeckComposition(deck);
        Debug.Log($"Generated random deck of {deck.Count} cards from {baseCards.Count} base card types");
    }

    // If boolean useWeightedGeneration is true, generate a deck based on weighted percentages
    // Alternative generation method with percentage-based weights
    private void GenerateWeightedDeck()
    {
        if (weightedCards.Count == 0)
        {
            Debug.LogWarning("No weighted cards assigned! Using random generation instead.");
            GenerateDeck();
            return;
        }

        deck.Clear();

        // Calculate total percentage to normalize if needed
        float totalPercentage = 0f;
        foreach (var weightedCard in weightedCards)
        {
            totalPercentage += weightedCard.weightPercentage;
        }

        // Warn if percentages don't add up to 1.0 (100%)
        if (Mathf.Abs(totalPercentage - 1.0f) > 0.0001f)
        {
            Debug.LogWarning($"Weighted cards total percentage is {totalPercentage:F2}, not 1.0. Results may cause an error.");
        }

        // Generate cards based on percentages
        int cardsAdded = 0;
        
        foreach (WeightedCard weightedCard in weightedCards)
        {
            if (weightedCard.card != null)
            {
                // Calculate how many cards this percentage represents
                // Normalize the percentage to the total deck size
                float normalizedPercentage = weightedCard.weightPercentage / totalPercentage;
                int cardCount = Mathf.RoundToInt(normalizedPercentage * totalDeckSize);

                for (int i = 0; i < cardCount; i++)
                {
                    deck.Add(weightedCard.card);
                    cardsAdded++;
                }

                Debug.Log($"{weightedCard.card.cardName}: {cardCount} cards ({weightedCard.weightPercentage:P1} of deck)");
            }
        }

        // Function that check if weightedCards is not reach maximum deck size //

        // Fill remaining slots with random cards from baseCards if we're short of totalDeckSize
        while (deck.Count < totalDeckSize && baseCards.Count > 0)
        {
            int randomIndex = rand.Next(baseCards.Count);
            deck.Add(baseCards[randomIndex]);
        }

        // Remove excess cards if we're over the totalDeckSize
        while (deck.Count > totalDeckSize)
        {
            int randomIndex = rand.Next(deck.Count);
            deck.RemoveAt(randomIndex);
        }
        GenerateDeckComposition(deck);
        Debug.Log($"Generated weighted deck of {deck.Count} cards from {weightedCards.Count} weighted types");
    }

    public void GenerateDeckComposition(List<CardSO> cards)
    {
        deckComposition.Clear();
        foreach (CardSO card in cards)
        {
            if (card == null) continue;
            if (deckComposition.ContainsKey(card.cardName))
            {
                deckComposition[card.cardName] += 1;
            }
            else
            {
                deckComposition[card.cardName] = 1;
            }
        }
    }
    #endregion

    #region Deck Management
    // Draw a single card from the deck
    public CardSO DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("Deck is empty! Cannot draw more cards.");
            return null;
        }

        int index = rand.Next(deck.Count);
        CardSO drawnCard = deck[index];
        RemoveCard(index);
        if (shuffleOnDraw)
        {
            ShuffleDeck();
        }
        Debug.Log($"Drew {drawnCard.cardName}. Cards remaining: {deck.Count}");
        return drawnCard;
    }

    public void RemoveCard(int index)
    {
        if (index >= 0 && index < deck.Count)
        {
            deck.RemoveAt(index);
        }
        else
        {
            Debug.LogWarning($"Index {index} is out of bounds for deck of size {deck.Count}.");
        }
    }
    // Shuffle the deck (randomize order of cards)
    public void ShuffleDeck()
    {
        deck = new List<CardSO>(deck.OrderBy(x => rand.Next()).ToList());
        Debug.Log($"Deck shuffled. Total cards: {deck.Count}");
    }

    // Regenerate and shuffle the deck (restarting the game without reassigning base cards)
    public void RegenerateDeck()
    {
        GenerateDeck();
        ShuffleDeck();
    }

    #endregion

    #region Deck Information
    public int GetRemainingCards() => deck.Count;
    public bool IsDeckEmpty() => deck.Count == 0;
    public bool IsCardInDeck(CardSO card) => deck.Contains(card);
    public int  GetCardCountInDeck(CardSO card)
    {
        if (card == null) return 0;
        return deck.Count(c => c == card);
    }
    public Dictionary<string, int> GetDeckComposition() => deckComposition;

    #endregion

    #region Editor Context Menu

    // Add all base cards to weighted cards list with equal distribution
    [ContextMenu("Add All Base Cards to Weighted List")]
    public void AddAllBaseCardsToWeightedList()
    {
        if (baseCards.Count == 0)
        {
            Debug.LogWarning("No base cards to add! Please assign base cards first.");
            return;
        }

        // Clear existing weighted cards
        weightedCards.Clear();

        // Calculate equal percentage for each card
        float equalPercentage = 1.0f / baseCards.Count;

        // Add each base card with equal weight
        foreach (var baseCard in baseCards)
        {
            if (baseCard != null)
            {
                var weightedCard = new WeightedCard
                {
                    card = baseCard,
                    weightPercentage = equalPercentage
                };
                weightedCards.Add(weightedCard);
            }
        }

        Debug.Log($"Added {weightedCards.Count} cards to weighted list with {equalPercentage:P1} each");

        // Automatically enable weighted generation
        useWeightedGeneration = true;

        // Validate the setup
        ValidateWeightedCards();
    }

    // Add missing base cards to weighted list (doesn't clear existing)
    [ContextMenu("Add Missing Base Cards to Weighted List")]
    public void AddMissingBaseCardsToWeightedList()
    {
        if (baseCards.Count == 0)
        {
            Debug.LogWarning("No base cards to add! Please assign base cards first.");
            return;
        }

        int addedCount = 0;
        // Check each base card
        foreach (var baseCard in baseCards)
        {
            if (baseCard != null)
            {
                // Check if this card is already in weighted list
                bool alreadyExists = false;
                foreach (var weightedCard in weightedCards)
                {
                    if (weightedCard.card == baseCard)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                // Add if not found
                if (!alreadyExists)
                {
                    var newWeightedCard = new WeightedCard
                    {
                        card = baseCard,
                        weightPercentage = 0.2f
                    };
                    weightedCards.Add(newWeightedCard);
                    addedCount++;
                }
            }
        }
        Debug.Log($"Added {addedCount} missing cards to weighted list. Total now: {weightedCards.Count}");
    }

    // Randomly distribute weighted cards percentages that add up to 100%
    [ContextMenu("Random Weighted Cards")]
    public void RandomWeightedCards()
    {
        if (weightedCards.Count == 0)
        {
            Debug.LogWarning("No weighted cards to balance!");
            return;
        }

        System.Random random = new System.Random();
        float remainingPercentage = 1.0f;
        int remainingCards = weightedCards.Count;

        // First pass: assign random percentages but leave room for remaining cards
        for (int i = 0; i < weightedCards.Count - 1; i++)
        {
            // Calculate max percentage this card can take (leave minimum for remaining cards)
            float minForRemainingCards = (remainingCards - 1) * 0.05f; // Minimum 5% per remaining card
            float maxPercentage = Math.Max(0.05f, remainingPercentage - minForRemainingCards);
            
            // Generate random percentage between 5% and the calculated maximum
            float minPercentage = 0.05f;
            float randomPercentage = (float)(random.NextDouble() * (maxPercentage - minPercentage) + minPercentage);
            
            weightedCards[i].weightPercentage = randomPercentage;
            remainingPercentage -= randomPercentage;
            remainingCards--;
        }

        // Last card gets whatever percentage is remaining
        weightedCards[weightedCards.Count - 1].weightPercentage = remainingPercentage;

        Debug.Log($"Randomly distributed percentages across {weightedCards.Count} cards");
        ValidateWeightedCards();
    }

    // Auto-balance weighted cards to equal percentages
    [ContextMenu("Auto-Balance Weighted Cards")]
    public void AutoBalanceWeightedCards()
    {
        if (weightedCards.Count == 0)
        {
            Debug.LogWarning("No weighted cards to balance!");
            return;
        }
        
        // Calculate equal percentage for each card
        float equalPercentage = 1.0f / weightedCards.Count;
        
        // Set all cards to equal percentage
        foreach (var weightedCard in weightedCards)
        {
            weightedCard.weightPercentage = equalPercentage;
        }
        
        Debug.Log($"Auto-balanced {weightedCards.Count} cards to {equalPercentage:P1} each");
        ValidateWeightedCards();
    }
    #endregion
    #region Weighted Card Validation
    // Validate that weighted cards add up to 100% (or close to it)
    public void ValidateWeightedCards()
    {
        if (weightedCards.Count == 0)
        {
            Debug.Log("No weighted cards to validate.");
            return;
        }

        float totalPercentage = 0f;
        Debug.Log("=== Weighted Card Breakdown ===");

        foreach (var weightedCard in weightedCards)
        {
            if (weightedCard.card != null)
            {
                float percentage = weightedCard.weightPercentage;
                int estimatedCards = Mathf.RoundToInt(percentage * totalDeckSize);
                totalPercentage += percentage;

                Debug.Log($"{weightedCard.card.cardName}: {percentage:P1} ({estimatedCards} cards)");
            }
        }

        Debug.Log($"Total: {totalPercentage:P1} ({totalPercentage * totalDeckSize:F0} cards)");

        if (Mathf.Abs(totalPercentage - 1.0f) > 0.01f)
        {
            Debug.LogWarning($"Total percentage is {totalPercentage:P1}, not 100%. Consider adjusting weights.");
        }
        else
        {
            Debug.Log("Weights are properly balanced!");
        }
    }
    // Add a specific card from base cards to weighted list
    public void AddSpecificCardToWeightedList(CardSO cardToAdd, float weight = 0.2f)
    {
        if (cardToAdd == null)
        {
            Debug.LogWarning("Cannot add null card to weighted list!");
            return;
        }

        // Check if this card is already in weighted list
        foreach (var weightedCard in weightedCards)
        {
            if (weightedCard.card == cardToAdd)
            {
                Debug.LogWarning($"{cardToAdd.cardName} is already in the weighted list!");
                return;
            }
        }

        // Add the card
        var newWeightedCard = new WeightedCard
        {
            card = cardToAdd,
            weightPercentage = weight
        };
        weightedCards.Add(newWeightedCard);

        Debug.Log($"Added {cardToAdd.cardName} to weighted list with {weight:P1} weight");
    }

    // Remove a specific card from weighted list
    public void RemoveCardFromWeightedList(CardSO cardToRemove)
    {
        if (cardToRemove == null) return;

        for (int i = weightedCards.Count - 1; i >= 0; i--)
        {
            if (weightedCards[i].card == cardToRemove)
            {
                weightedCards.RemoveAt(i);
                Debug.Log($"Removed {cardToRemove.cardName} from weighted list");
                return;
            }
        }

        Debug.LogWarning($"{cardToRemove.cardName} was not found in weighted list!");
    }

    // Check if a card is already in the weighted list
    public bool IsCardInWeightedList(CardSO card)
    {
        if (card == null) return false;

        foreach (var weightedCard in weightedCards)
        {
            if (weightedCard.card == card)
                return true;
        }
        return false;
    }
    #endregion
}

