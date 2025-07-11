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
    [Header("Debug Configuration")]
    [SerializeField] private int pathogenHealth;
    
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
        
        // Get initial list of pathogens (use ToList() to avoid modification during iteration)
        var pathogensToProcess = activePathogens.ToList();
        
        foreach (var pathogen in pathogensToProcess)
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
        
        // If a pathogen died and a new one was spawned during this turn, let the new one act too
        var newPathogens = activePathogens.Where(p => !pathogensToProcess.Contains(p) && p.IsAlive()).ToList();
        if (newPathogens.Count > 0)
        {
            Debug.Log($"PathogenManager: New pathogen(s) spawned during turn, letting them act immediately");
            foreach (var newPathogen in newPathogens)
            {
                // New pathogens get to act immediately on their spawn turn
                newPathogen.ProcessTurnStart(playedCards);
                newPathogen.AttackPlayer(player);
                Debug.Log($"PathogenManager: New pathogen {newPathogen.GetPathogenName()} completed immediate turn actions");
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
        
        Debug.Log($"Pathogen destroyed: {pathogen.GetPathogenName()}");
        OnPathogenDefeated?.Invoke(pathogen);
        
        // Check victory condition first
        if (activePathogens.Count == 0 && pathogenQueue.Count == 0)
        {
            OnAllPathogensDefeated?.Invoke();
            return;
        }
        
        // If no active pathogens but more in queue, spawn next immediately
        if (activePathogens.Count == 0 && pathogenQueue.Count > 0)
        {
            Debug.Log("PathogenManager: Spawning next pathogen immediately to continue turn");
            SpawnNextPathogen();
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
    
    public bool HasMorePathogens()
    {
        return pathogenQueue.Count > 0;
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
    
    [ContextMenu("Debug Pathogen Turn Flow")]
    public void DebugPathogenTurnFlow()
    {
        Debug.Log("=== Pathogen Turn Flow Debug ===");
        Debug.Log($"Active Pathogens: {GetActivePathogenCount()}");
        Debug.Log($"Pathogens in Queue: {pathogenQueue.Count}");
        Debug.Log($"Current Targeted: {(currentTargetedPathogen?.GetPathogenName() ?? "None")}");
        
        if (activePathogens.Count > 0)
        {
            for (int i = 0; i < activePathogens.Count; i++)
            {
                var pathogen = activePathogens[i];
                Debug.Log($"  [{i}] {pathogen.GetPathogenName()} - HP: {pathogen.GetCurrentHealth()}/{pathogen.GetMaxHealth()} - Alive: {pathogen.IsAlive()}");
            }
        }
    }
    
    #endregion

    #region Health Debug Methods
    
    [ContextMenu("Debug Current Pathogen Health")]
    public void DebugCurrentPathogenHealth()
    {
        if (currentTargetedPathogen == null)
        {
            Debug.Log("No pathogen currently targeted");
            return;
        }
        
        var health = currentTargetedPathogen.GetHealth();
        Debug.Log($"=== {currentTargetedPathogen.GetPathogenName()} Health Debug ===");
        Debug.Log($"Current Health: {health.GetCurrentHealth()}");
        Debug.Log($"Max Health: {health.GetMaxHealth()}");
        Debug.Log($"Health Percentage: {health.GetHealthPercentage():P1}");
        Debug.Log($"Is Alive: {currentTargetedPathogen.IsAlive()}");
    }
    
    [ContextMenu("Debug All Active Pathogens Health")]
    public void DebugAllPathogensHealth()
    {
        Debug.Log($"=== All Active Pathogens Health ({activePathogens.Count} total) ===");
        
        for (int i = 0; i < activePathogens.Count; i++)
        {
            var pathogen = activePathogens[i];
            var health = pathogen.GetHealth();
            
            Debug.Log($"[{i}] {pathogen.GetPathogenName()}: " +
                     $"{health.GetCurrentHealth()}/{health.GetMaxHealth()} HP " +
                     $"({health.GetHealthPercentage():P1}) - " +
                     $"Alive: {pathogen.IsAlive()}");
        }
    }
    
    /// <summary>
    /// Get current pathogen health info for external debugging
    /// </summary>
    /// <returns>Health info string or null if no pathogen</returns>
    public string GetCurrentPathogenHealthInfo()
    {
        if (currentTargetedPathogen == null) return "No targeted pathogen";
        
        var health = currentTargetedPathogen.GetHealth();
        return $"{currentTargetedPathogen.GetPathogenName()}: " +
               $"{health.GetCurrentHealth()}/{health.GetMaxHealth()} HP " +
               $"({health.GetHealthPercentage():P1})";
    }
    
    #endregion
}