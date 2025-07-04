using UnityEngine;
using System;

public enum TurnPhase
{
    PlayerTurn,
    PathogenTurn,
    GameOver
}

/// <summary>
/// Manages turn state, card play limits, and turn tracking
/// Notifies other managers about turn changes
/// </summary>
public class TurnManager : MonoBehaviour
{
    [Header("Turn Settings")]
    [SerializeField] private int maxCardsPlayPerTurn = 2;
    [SerializeField] private int cardsToDrawEachTurn = 2;
    
    private TurnPhase currentPhase = TurnPhase.PlayerTurn;
    private int turnNumber = 1;
    private int cardsPlayedThisTurn = 0;
    
    // Events for other managers
    public static event Action<TurnPhase> OnTurnPhaseChanged;
    public static event Action<int> OnTurnNumberChanged;
    public static event Action<CardSO> OnCardPlayedInTurn;
    
    #region Turn Management
    
    public void StartPlayerTurn()
    {
        currentPhase = TurnPhase.PlayerTurn;
        cardsPlayedThisTurn = 0;
        turnNumber++;
        
        Debug.Log($"TurnManager: Player Turn {turnNumber} started");
        
        OnTurnPhaseChanged?.Invoke(currentPhase);
        OnTurnNumberChanged?.Invoke(turnNumber);
    }
    
    public void EndPlayerTurn()
    {
        Debug.Log($"TurnManager: Player Turn {turnNumber} ended - {cardsPlayedThisTurn} cards played");
    }
    
    public void StartPathogenTurn()
    {
        currentPhase = TurnPhase.PathogenTurn;
        Debug.Log("TurnManager: Pathogen Turn started");
        
        OnTurnPhaseChanged?.Invoke(currentPhase);
    }
    
    #endregion
    
    #region Card Play Tracking
    
    public void OnCardPlayed(CardSO card)
    {
        cardsPlayedThisTurn++;
        Debug.Log($"TurnManager: Card played ({cardsPlayedThisTurn}/{maxCardsPlayPerTurn})");
        
        OnCardPlayedInTurn?.Invoke(card);
    }
    
    public bool CanPlayMoreCards()
    {
        return cardsPlayedThisTurn < maxCardsPlayPerTurn && IsPlayerTurn();
    }
    
    #endregion
    
    #region Getters
    
    public bool IsPlayerTurn() => currentPhase == TurnPhase.PlayerTurn;
    public TurnPhase GetCurrentPhase() => currentPhase;
    public int GetTurnNumber() => turnNumber;
    public int GetCardsPlayedThisTurn() => cardsPlayedThisTurn;
    public int GetMaxCardsPerTurn() => maxCardsPlayPerTurn;
    public int GetCardsToDrawEachTurn() => cardsToDrawEachTurn;
    
    #endregion
}
