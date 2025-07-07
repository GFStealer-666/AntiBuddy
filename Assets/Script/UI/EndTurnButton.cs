using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndTurnButton : MonoBehaviour  
{
    [Header("UI Components")]
    [SerializeField] private Button endTurnButton;
    
    
    private GameManager gameManager;
    private TurnManager turnManager;
    private bool isInitialized = false;
    
    #region Unity Lifecycle
    
    void Awake()
    {
        // Get button component if not assigned
        if (endTurnButton == null)
        {
            endTurnButton = GetComponent<Button>();
        }
        
        // Setup button click listener
        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }
    }
    
    void Start()
    {
        InitializeManagers();
        UpdateButtonState();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (turnManager != null)
        {
            TurnManager.OnTurnPhaseChanged -= OnTurnPhaseChanged;
        }
        
        // Remove button listener
        if (endTurnButton != null)
        {
            endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
        }
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeManagers()
    {
        // Find GameManager
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogWarning("EndTurnButton: GameManager not found!");
            }
        }
        
        // Find TurnManager
        if (turnManager == null)
        {
            turnManager = FindFirstObjectByType<TurnManager>();
            if (turnManager == null)
            {
                Debug.LogWarning("EndTurnButton: TurnManager not found!");
            }
        }
        
        // Subscribe to turn phase changes
        if (turnManager != null)
        {
            TurnManager.OnTurnPhaseChanged += OnTurnPhaseChanged;
            Debug.Log("EndTurnButton: Successfully subscribed to TurnManager events");
        }
        
        isInitialized = true;
    }
    
    #endregion
    
    #region Button Logic
    
    private void OnEndTurnClicked()
    {
        // Play click sound if available
        
        // Check if we can end the turn
        if (turnManager == null || gameManager == null)
        {
            Debug.LogWarning("EndTurnButton: Missing managers, cannot end turn");
            return;
        }
        
        // Only allow ending turn during player turn
        if (turnManager.GetCurrentPhase() != TurnPhase.PlayerTurn)
        {
            Debug.LogWarning("EndTurnButton: Cannot end turn - not player's turn");
            return;
        }
        
        Debug.Log("EndTurnButton: Player manually ending turn");
        
        // Call GameManager to end turn
        gameManager.EndTurnButtonPressed();
        
        // Provide immediate visual feedback
        UpdateButtonState();
    }
    
    #endregion
    
    #region Event Handlers
    
    private void OnTurnPhaseChanged(TurnPhase newPhase)
    {
        Debug.Log($"EndTurnButton: Turn phase changed to {newPhase}");
        UpdateButtonState();
    }
    
    #endregion
    
    #region UI Updates
    
    private void UpdateButtonState()
    {
        if (!isInitialized || turnManager == null) return;
        
        TurnPhase currentPhase = turnManager.GetCurrentPhase();

        if (currentPhase == TurnPhase.PlayerTurn)
        {
            SetButtonState(true, "End Turn");
        }
        else
        {
            SetButtonState(false, "Not Your Turn");
        }
    }
    
    private void SetButtonState(bool interactable, string text)
    {
        // Update button interactability
        if (endTurnButton != null)
        {
            endTurnButton.interactable = interactable;
        }
    }
    
    #endregion
    
    #region Public Methods
    [ContextMenu("Refresh Button State")]
    public void RefreshButtonState()
    {
        UpdateButtonState();
    }
    
    public bool IsInteractable()
    {
        return endTurnButton != null && endTurnButton.interactable;
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Test End Turn")]
    public void TestEndTurn()
    {
        Debug.Log("EndTurnButton: Testing end turn functionality");
        OnEndTurnClicked();
    }
    
    [ContextMenu("Debug Button State")]
    public void DebugButtonState()
    {
        Debug.Log("=== End Turn Button State ===");
        Debug.Log($"Button Interactable: {endTurnButton?.interactable ?? false}");
        Debug.Log($"Current Phase: {turnManager?.GetCurrentPhase() ?? TurnPhase.GameOver}");
        Debug.Log($"Managers Found - GameManager: {gameManager != null}, TurnManager: {turnManager != null}");
    }
    
    #endregion
}
