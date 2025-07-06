using UnityEngine;
using System.Collections.Generic;

public class CardField : MonoBehaviour
{
    [Header("Field Configuration")]
    public int maxFieldCards = 2;
    
    [SerializeField] private List<CardSO> cardsInField = new List<CardSO>();

    public static System.Action<List<CardSO>> OnFieldChanged;

    void Awake()
    {
        cardsInField = new List<CardSO>();
    }

    public bool TryPlayCardToField(CardSO card)
    {
        if (card == null)
        {
            Debug.LogWarning("Cannot add null card to field");
            return false;
        }
        
        if (cardsInField.Count >= maxFieldCards)
        {
            Debug.LogWarning($"Field is full! Max cards: {maxFieldCards}");
            return false;
        }
        
        cardsInField.Add(card);
        OnFieldChanged?.Invoke(new List<CardSO>(cardsInField)); // Always pass a copy
        Debug.Log($"Added {card.cardName} to field ({cardsInField.Count}/{maxFieldCards})");
        return true;
    }

    public bool RemoveCardFromField(CardSO card)
    {
        if (cardsInField.Remove(card))
        {
            OnFieldChanged?.Invoke(new List<CardSO>(cardsInField));
            Debug.Log($"Removed {card.cardName} from field ({cardsInField.Count}/{maxFieldCards})");
            return true;
        }
        return false;
    }

    public void ClearField()
    {
        cardsInField.Clear();
        OnFieldChanged?.Invoke(new List<CardSO>(cardsInField));
        Debug.Log("Field cleared");
    }

    public List<CardSO> GetCardsInField()
    {
        return new List<CardSO>(cardsInField);
    }

    public bool IsFieldFull()
    {
        return cardsInField.Count >= maxFieldCards;
    }

    public int GetAvailableSlots()
    {
        return maxFieldCards - cardsInField.Count;
    }

    public int GetCardCount()
    {
        return cardsInField.Count;
    }

    // Check for specific card types in field - uses CardEffects utility
    public bool HasCardTypeInField<T>() where T : CardSO
    {
        return CardEffects.HasCardType<T>(cardsInField);
    }

    public int CountCardTypeInField<T>() where T : CardSO
    {
        return CardEffects.CountCardType<T>(cardsInField);
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

        // Add field effects logic here if needed
    }

    #region Debug Methods
    
    [ContextMenu("Debug Field Status")]
    public void DebugFieldStatus()
    {
        Debug.Log($"=== Card Field Status ===");
        Debug.Log($"Cards in field: {cardsInField.Count}/{maxFieldCards}");
        Debug.Log($"Available slots: {GetAvailableSlots()}");
        Debug.Log($"Field full: {IsFieldFull()}");
        
        if (cardsInField.Count > 0)
        {
            Debug.Log("Cards currently in field:");
            for (int i = 0; i < cardsInField.Count; i++)
            {
                Debug.Log($"  [{i}] {cardsInField[i].cardName}");
            }
        }
        else
        {
            Debug.Log("Field is empty");
        }
    }
    
    #endregion
}
