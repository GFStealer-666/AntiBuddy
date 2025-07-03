using System.Collections.Generic;
using UnityEngine;

public abstract class ImmuneCardSO : CardSO
{
    [Header("Immune Card Settings")]
    public ImmuneCardType cardType = ImmuneCardType.Instant;
    
    [Header("Activation Tracking")]
    public bool hasActivatedThisTurn = false; // Track if effect has been applied this turn
    
    public override void ApplyEffect(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // Check if already activated this turn
        if (hasActivatedThisTurn)
        {
            Debug.LogWarning($"{cardName} has already been activated this turn");
            return;
        }
        
        // Check if card can activate based on its type and field conditions
        if (!CanActivateNow(playedCards))
        {
            Debug.Log($"{cardName} cannot activate - conditions not met");
            return;
        }
        
        // Mark as activated this turn
        hasActivatedThisTurn = true;
        
        // Apply the actual effect (implemented by derived classes)
        DoCardEffect(player, playedCards, target);
    }
    
    // Virtual method that derived classes can override for their specific effects
    protected virtual void DoCardEffect(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // Default implementation - derived classes should override this
        Debug.Log($"{cardName} effect applied");
    }
    
    // Reset activation status at start of new turn
    public virtual void ResetActivation()
    {
        hasActivatedThisTurn = false;
    }
    
    // Check if this card can activate right now based on field conditions
    protected virtual bool CanActivateNow(List<CardSO> cardsInField)
    {
        if (hasActivatedThisTurn) return false;
        
        // Instant cards can always activate
        if (cardType == ImmuneCardType.Instant) return true;
        
        // Combo cards need specific conditions (override in derived classes)
        return CanActivateCombo(cardsInField);
    }
    
    // Override this in derived classes that need specific activation conditions
    protected virtual bool CanActivateCombo(List<CardSO> cardsInField)
    {
        // Default: combo cards can always activate
        return true;
    }
}

public enum ImmuneCardType
{
    Instant,    // Activates immediately when played
    Combo       // Requires specific conditions to activate
}
