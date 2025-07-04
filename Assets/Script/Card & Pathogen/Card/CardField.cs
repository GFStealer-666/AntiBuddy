using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CardFieldSlot
{
    public CardSO card;
    public bool isOccupied;
    public int slotIndex;

    public CardFieldSlot(int index)
    {
        slotIndex = index;
        isOccupied = false;
        card = null;
    }

    public bool TryPlaceCard(CardSO cardToPlace)
    {
        if (!isOccupied && cardToPlace != null)
        {
            card = cardToPlace;
            isOccupied = true;
            return true;
        }
        return false;
    }

    public CardSO RemoveCard()
    {
        CardSO removedCard = card;
        card = null;
        isOccupied = false;
        return removedCard;
    }

    public void ClearSlot()
    {
        card = null;
        isOccupied = false;
    }
}

public class CardField : MonoBehaviour
{
    [Header("Field Configuration")]
    public int maxFieldSlots = 2;
    
    private List<CardFieldSlot> fieldSlots;
    private List<CardSO> activeCards; // Cards currently in field that provide ongoing effects

    public static System.Action<List<CardSO>> OnFieldChanged;

    void Awake()
    {
        InitializeField();
    }

    void InitializeField()
    {
        fieldSlots = new List<CardFieldSlot>();
        activeCards = new List<CardSO>();
        
        for (int i = 0; i < maxFieldSlots; i++)
        {
            fieldSlots.Add(new CardFieldSlot(i));
        }
    }

    public bool TryPlayCardToField(CardSO card)
    {
        // Find the first available slot
        for (int i = 0; i < fieldSlots.Count; i++)
        {
            if (fieldSlots[i].TryPlaceCard(card))
            {
                activeCards.Add(card);
                OnFieldChanged?.Invoke(new List<CardSO>(activeCards));
                Debug.Log($"Placed {card.cardName} in field slot {i}");
                return true;
            }
        }
        
        Debug.LogWarning("No available field slots!");
        return false;
    }

    public void ClearField()
    {
        foreach (var slot in fieldSlots)
        {
            slot.ClearSlot();
        }
        activeCards.Clear();
        OnFieldChanged?.Invoke(new List<CardSO>(activeCards));
        Debug.Log("Field cleared");
    }

    public List<CardSO> GetActiveCards()
    {
        return new List<CardSO>(activeCards);
    }

    public List<CardFieldSlot> GetFieldSlots()
    {
        return new List<CardFieldSlot>(fieldSlots);
    }

    public bool IsFieldFull()
    {
        return activeCards.Count >= maxFieldSlots;
    }

    public int GetAvailableSlots()
    {
        return maxFieldSlots - activeCards.Count;
    }

    // Check for specific card types in field - uses CardEffects utility
    public bool HasCardTypeInField<T>() where T : CardSO
    {
        return CardEffects.HasCardType<T>(activeCards);
    }

    public int CountCardTypeInField<T>() where T : CardSO
    {
        return CardEffects.CountCardType<T>(activeCards);
    }

    // Utility method to check multiple card types at once
    public bool HasAnyCardTypes<T1, T2>() 
        where T1 : CardSO 
        where T2 : CardSO
    {
        return HasCardTypeInField<T1>() || HasCardTypeInField<T2>();
    }
    
    // Check if field has all required card types for combo
    public bool HasAllCardTypes<T1, T2>() 
        where T1 : CardSO 
        where T2 : CardSO
    {
        return HasCardTypeInField<T1>() && HasCardTypeInField<T2>();
    }

    // Special field-wide effects that can be triggered by TurnManager
    // trigger when certain conditions are met on the field
    // trigger in endturn or at specific times 
    public void CheckForFieldEffects(Player player, Pathogen target)
    {
        // Cytokine Storm - when too many immune cells are active
        int immuneCellCount = CountCardTypeInField<BCellCardSO>() +
                            CountCardTypeInField<CytotoxicCellCardSO>() +
                            CountCardTypeInField<MacrophageCardSO>() +
                            CountCardTypeInField<HelperTCellCardSO>() +
                            CountCardTypeInField<NaturalKillerCardSO>();


    }
}
