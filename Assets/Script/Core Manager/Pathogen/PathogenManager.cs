using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Manages pathogen spawning, lifecycle, and combat
/// Handles all pathogen-related actions and abilities
/// </summary>
public class PathogenManager : MonoBehaviour
{
    [Header("Pathogen Configuration")]
    [SerializeField] private List<PathogenSO> basePathogens = new List<PathogenSO>();
    [SerializeField] private int maxActivePathogens = 1;
    
    private List<PathogenSO> pathogenQueue = new List<PathogenSO>();
    private List<Pathogen> activePathogens = new List<Pathogen>();
    private Pathogen currentTargetedPathogen;
    
    public event Action<Pathogen> OnPathogenSpawned;
    public event Action<Pathogen> OnPathogenDefeated;
    public event Action OnAllPathogensDefeated;
    public event Action<Pathogen> OnPathogenTargeted;
    
    #region Initialization 
    private void Start()
    {
        // Subscribe to TurnManager events
        
        InitializePathogenQueue();
        SpawnNextPathogen();
    }
    
    private void InitializePathogenQueue()
    {
        if (basePathogens.Count == 0)
        {
            Debug.LogError("No base pathogens configured!");
            return;
        }
        
        pathogenQueue.Clear();
        pathogenQueue.AddRange(basePathogens);
        ShufflePathogenQueue();
        
        Debug.Log($"Pathogen queue initialized with {pathogenQueue.Count} pathogens");
    }
    
    private void ShufflePathogenQueue()
    {
        for (int i = 0; i < pathogenQueue.Count; i++)
        {
            PathogenSO temp = pathogenQueue[i];
            int randomIndex = UnityEngine.Random.Range(i, pathogenQueue.Count);
            pathogenQueue[i] = pathogenQueue[randomIndex];
            pathogenQueue[randomIndex] = temp;
        }
    }
    
    #endregion
    
    #region Pathogen Spawning
    
    public void SpawnNextPathogen()
    {
        if (activePathogens.Count >= maxActivePathogens)
        {
            Debug.Log("Maximum pathogens already active");
            return;
        }
        
        if (pathogenQueue.Count == 0)
        {
            Debug.Log("No more pathogens in queue - Victory condition!");
            OnAllPathogensDefeated?.Invoke();
            return;
        }
        
        PathogenSO nextPathogenTemplate = pathogenQueue[0];
        pathogenQueue.RemoveAt(0);
        
        Pathogen newPathogen = new Pathogen(nextPathogenTemplate);
        newPathogen.OnPathogenDied += HandlePathogenDied;
        
        activePathogens.Add(newPathogen);
        SetTargetedPathogen(newPathogen);
        
        OnPathogenSpawned?.Invoke(newPathogen);
        Debug.Log($"Spawned pathogen: {newPathogen.GetPathogenName()}");
    }
    
    #endregion
    
    #region Targeting System
    
    public void SetTargetedPathogen(Pathogen pathogen)
    {
        currentTargetedPathogen = pathogen;
        OnPathogenTargeted?.Invoke(pathogen);
        Debug.Log($"Targeted pathogen: {pathogen?.GetPathogenName() ?? "None"}");
    }
    
    public Pathogen GetCurrentPathogen()
    {
        return currentTargetedPathogen;
    }
    
    #endregion
    
    #region Combat System
    
    public void ExecutePathogenTurn(Player player, List<CardSO> playedCards)
    {
        Debug.Log("PathogenManager: Executing pathogen turn");
        
        foreach (var pathogen in activePathogens.ToList())
        {
            if (pathogen.IsAlive())
            {
                // Process pathogen abilities first
                pathogen.ProcessTurnStart(playedCards);
                
                // Then attack player
                pathogen.AttackPlayer(player);
                
                Debug.Log($"PathogenManager: {pathogen.GetPathogenName()} completed turn actions");
            }
        }
    }
    
    #endregion
    
    #region Pathogen Management
    
    private void HandlePathogenDied(Pathogen pathogen)
    {
        activePathogens.Remove(pathogen);
        
        if (currentTargetedPathogen == pathogen)
        {
            currentTargetedPathogen = null;
        }
        
        OnPathogenDefeated?.Invoke(pathogen);
        Debug.Log($"Pathogen destroyed: {pathogen.GetPathogenName()}");
        
        // Check victory condition
        if (activePathogens.Count == 0 && pathogenQueue.Count == 0)
        {
            OnAllPathogensDefeated?.Invoke();
        }
        else if (activePathogens.Count == 0)
        {
            // Spawn next pathogen after delay
            Invoke(nameof(SpawnNextPathogen), 2f);
        }
    }
    
    #endregion
    
    #region Public Accessors
    
    public List<Pathogen> GetActivePathogens()
    {
        return activePathogens.Where(p => p.IsAlive()).ToList();
    }
    
    public int GetActivePathogenCount()
    {
        return activePathogens.Count(p => p.IsAlive());
    }
    
    public int GetRemainingPathogenCount()
    {
        return pathogenQueue.Count + GetActivePathogenCount();
    }
    
    public bool HasActivePathogens()
    {
        return GetActivePathogenCount() > 0;
    }
    
    public bool IsCardBlocked(System.Type cardType)
    {
        return activePathogens.Any(p => p.IsAlive() && p.IsCardBlocked(cardType));
    }
    public bool IsPathogenAllDefeated()
    {
        return GetActivePathogenCount() == 0 && pathogenQueue.Count == 0;
    }
    
    #endregion

    #region Debug Methods

    [ContextMenu("Spawn Next Pathogen")]
    public void ForceSpawnNext()
    {
        SpawnNextPathogen();
    }
    
    
    [ContextMenu("Shuffle Queue")]
    public void ReshuffleQueue()
    {
        ShufflePathogenQueue();
    }
    
    #endregion
}