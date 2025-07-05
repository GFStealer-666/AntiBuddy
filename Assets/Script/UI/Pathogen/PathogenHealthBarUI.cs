using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PathogenHealthBarUI : MonoBehaviour
{
    [Header("Health Bar Components")]
    [SerializeField] private Image healthBarImage; // Single image that will display different sprites
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Health Bar Sprites")]
    [SerializeField] private Sprite[] healthBarSprites; // Array of sprites from full to empty health
    [SerializeField] private bool reverseOrder = false; // If true: index 0 = empty, last index = full
    
    private PathogenHealth currentPathogenHealth;
    private PathogenData currentPathogenData;
    private PathogenManager pathogenManager;
    
    private void Awake()
    {
        // Cache PathogenManager reference and subscribe to events
        pathogenManager = FindAnyObjectByType<PathogenManager>();
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned += OnPathogenSpawned;
            pathogenManager.OnPathogenDefeated += OnPathogenDefeated;
        }
        else
        {
            Debug.LogWarning("PathogenHealthBarUI: PathogenManager not found!");
        }
        
        // Initially hide the UI
        // gameObject.SetActive(false);
    }
    
    private void OnPathogenSpawned(Pathogen pathogen)
    {
        currentPathogenHealth = pathogen.GetHealth();
        currentPathogenData = pathogen.GetData();
        
        Debug.Log($"PathogenHealthBarUI: Subscribing to health events for {pathogen.GetPathogenName()}");
        
        // Subscribe to health events
        currentPathogenHealth.OnHealthChanged += UpdateHealthBar;
        currentPathogenHealth.OnDamageTaken += OnDamageTaken;
        currentPathogenHealth.OnPathogenDied += OnPathogenDied;
        
        // Initialize UI
        UpdateHealthBar(currentPathogenHealth.GetCurrentHealth());
        
        // Show the UI
    }
    
    private void OnPathogenDefeated(Pathogen pathogen)
    {
        // Only process if this is the pathogen we're currently tracking
        if (currentPathogenHealth != null && currentPathogenHealth == pathogen.GetHealth())
        {
            // Delay unsubscription to ensure all death event handlers complete
            if (this != null && gameObject != null)
            {
                StartCoroutine(DelayedUnsubscribe());
            }
        }
    }
    
    private System.Collections.IEnumerator DelayedUnsubscribe()
    {
        yield return new WaitForEndOfFrame();
        UnsubscribeFromCurrentPathogen();
    }
    
    private void UpdateHealthBar(int currentHealth)
    {
        if (currentPathogenHealth == null || healthBarSprites == null || healthBarSprites.Length == 0) return;
        
        int maxHealth = currentPathogenHealth.GetMaxHealth();
        float healthPercentage = currentPathogenHealth.GetHealthPercentage();
        
        // Calculate which sprite to show based on health percentage
        int spriteIndex = GetSpriteIndexFromHealth(healthPercentage);
        
        // Update health bar sprite
        if (healthBarImage != null && spriteIndex >= 0 && spriteIndex < healthBarSprites.Length)
        {
            healthBarImage.sprite = healthBarSprites[spriteIndex];
        }
        
        // Update health text
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} HP";
        }
    }
    
    /// <summary>
    /// Convert health percentage to sprite array index
    /// </summary>
    /// <param name="healthPercentage">Health as percentage (0.0 to 1.0)</param>
    /// <returns>Index in the sprite array</returns>
    private int GetSpriteIndexFromHealth(float healthPercentage)
    {
        if (healthBarSprites == null || healthBarSprites.Length == 0) return 0;
        
        // Clamp health percentage between 0 and 1
        healthPercentage = Mathf.Clamp01(healthPercentage);
        
        // Calculate index based on health percentage
        // If we have 5 sprites: 0%, 25%, 50%, 75%, 100%
        int maxIndex = healthBarSprites.Length - 1;
        int calculatedIndex = Mathf.RoundToInt(healthPercentage * maxIndex);
        
        // Handle reverse order if needed
        if (reverseOrder)
        {
            calculatedIndex = maxIndex - calculatedIndex;
        }
        
        return Mathf.Clamp(calculatedIndex, 0, maxIndex);
    }
    
    private void OnDamageTaken(int damage)
    {
        Debug.Log($"Pathogen took {damage} damage!");
    }
    
    private void OnPathogenDied()
    {
        Debug.Log("Pathogen has been defeated!");
    }
    
    private void UnsubscribeFromCurrentPathogen()
    {
        if (currentPathogenHealth != null)
        {
            Debug.Log($"PathogenHealthBarUI: Unsubscribing from pathogen health events for {currentPathogenData?.PathogenName ?? "Unknown"}");
            
            currentPathogenHealth.OnHealthChanged -= UpdateHealthBar;
            currentPathogenHealth.OnDamageTaken -= OnDamageTaken;
            currentPathogenHealth.OnPathogenDied -= OnPathogenDied;
        }
        
        currentPathogenHealth = null;
        currentPathogenData = null;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from PathogenManager events first
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned -= OnPathogenSpawned;
            pathogenManager.OnPathogenDefeated -= OnPathogenDefeated;
        }
        
        // Then unsubscribe from current pathogen
        UnsubscribeFromCurrentPathogen();
    }
    
    #region Validation & Debug
    
    [ContextMenu("Test Health Bar Sprites")]
    private void TestHealthBarSprites()
    {
        if (healthBarSprites == null || healthBarSprites.Length == 0)
        {
            Debug.LogWarning("No health bar sprites assigned!");
            return;
        }
        
        Debug.Log($"Health bar has {healthBarSprites.Length} sprites");
        for (int i = 0; i < healthBarSprites.Length; i++)
        {
            float testHealth = (float)i / (healthBarSprites.Length - 1);
            int spriteIndex = GetSpriteIndexFromHealth(testHealth);
            Debug.Log($"Health {testHealth:P0} → Sprite Index {spriteIndex}");
        }
    }
    
    // Validate setup in inspector
    private void OnValidate()
    {
        if (healthBarSprites != null && healthBarSprites.Length > 0)
        {
            // Check for null sprites
            for (int i = 0; i < healthBarSprites.Length; i++)
            {
                if (healthBarSprites[i] == null)
                {
                    Debug.LogWarning($"Health bar sprite at index {i} is null!");
                }
            }
        }
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// Manually set the PathogenManager reference (alternative to auto-finding)
    /// </summary>
    /// <param name="manager">The PathogenManager to connect to</param>
    public void SetPathogenManager(PathogenManager manager)
    {
        // Unsubscribe from old manager if any
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned -= OnPathogenSpawned;
            pathogenManager.OnPathogenDefeated -= OnPathogenDefeated;
        }
        
        // Subscribe to new manager
        pathogenManager = manager;
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned += OnPathogenSpawned;
            pathogenManager.OnPathogenDefeated += OnPathogenDefeated;
        }
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Check Event Subscription Status")]
    public void DebugEventSubscriptionStatus()
    {
        Debug.Log("=== PathogenHealthBarUI Event Subscription Status ===");
        Debug.Log($"currentPathogenHealth: {(currentPathogenHealth != null ? "Connected" : "NULL")}");
        Debug.Log($"currentPathogenData: {(currentPathogenData != null ? currentPathogenData.PathogenName : "NULL")}");
        Debug.Log($"GameObject Active: {gameObject.activeInHierarchy}");
        
        if (currentPathogenHealth != null)
        {
            Debug.Log($"Pathogen Health: {currentPathogenHealth.GetCurrentHealth()}/{currentPathogenHealth.GetMaxHealth()}");
            Debug.Log($"Pathogen Alive: {currentPathogenHealth.IsAlive()}");
        }
    }
    
    #endregion
}
