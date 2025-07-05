using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Player-specific health bar UI
/// Uses composition with SpriteHealthBarUI for shared functionality
/// Handles player-specific logic and event subscriptions
/// </summary>
public class PlayerHealthBarUI : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Player player;
    
    [Header("Health Bar Components")]
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Health Bar Sprites")]
    [SerializeField] private Sprite[] healthBarSprites;
    [SerializeField] private bool reverseOrder = false;
    
    private int lastHP = -1;
    private float lastRefreshTime = 0f;
    
    private void OnEnable()
    {
        SubscribeToPlayerEvents();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromPlayerEvents();
    }
    
    private void SubscribeToPlayerEvents()
    {
        if (player != null && player.PlayerHealth != null)
        {
            player.PlayerHealth.OnHealthChanged += CheckForHealthChanges;
            Debug.Log($"PlayerHealthBarUI: Subscribed to player health events");
        }
        else
        {
            Debug.LogWarning($"PlayerHealthBarUI: Cannot subscribe - player: {(player != null ? "OK" : "NULL")}, PlayerHealth: {(player?.PlayerHealth != null ? "OK" : "NULL")}");
        }
    }
    
    private void UnsubscribeFromPlayerEvents()
    {
        if (player != null && player.PlayerHealth != null)
        {
            player.PlayerHealth.OnHealthChanged -= CheckForHealthChanges;
            Debug.Log($"PlayerHealthBarUI: Unsubscribed from player health events");
        }
    }

    void Awake()
    {
        if (player == null)
        {
            Debug.Log("PlayerHealthBarUI: Player not assigned, searching for GameManager...");
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                player = gameManager.GetPlayer();
                Debug.Log($"PlayerHealthBarUI: Found player via GameManager: {(player != null ? "SUCCESS" : "FAILED")}");
            }
            else
            {
                Debug.LogWarning("PlayerHealthBarUI: GameManager not found!");
            }
        }
        
        // Re-subscribe after finding player
        if (player != null)
        {
            SubscribeToPlayerEvents();
        }
        
        UpdateHealthDisplay();
    }
    
    private void CheckForHealthChanges(int newHP, int maxHP)
    {
        if (player == null) 
        {
            Debug.LogWarning("PlayerHealthBarUI: CheckForHealthChanges called but player is null!");
            return;
        }
        
        Debug.Log($"PlayerHealthBarUI: Health changed - HP: {newHP}/{maxHP} (last: {lastHP})");
        
        if (newHP != lastHP)
        {
            UpdateHealthDisplay();
            lastHP = newHP;
        }
    }
    
    private void UpdateHealthDisplay()
    {
        if (player == null || healthBarSprites == null || healthBarSprites.Length == 0) return;
        
        // Calculate health percentage
        float healthPercentage = (float)player.HP / player.MaxHP;
        
        // Use utility method for sprite selection
        int spriteIndex = GetSpriteIndexFromHealth(healthPercentage);
        
        // Update health bar sprite
        if (healthBarImage != null && spriteIndex >= 0 && spriteIndex < healthBarSprites.Length)
        {
            healthBarImage.sprite = healthBarSprites[spriteIndex];
        }
        
        // Update health text
        if (healthText != null)
        {
            healthText.text = $"{player.HP} HP";
        }
    }
    
    /// <summary>
    /// Convert health percentage to sprite array index (shared logic)
    /// </summary>
    private int GetSpriteIndexFromHealth(float healthPercentage)
    {
        if (healthBarSprites == null || healthBarSprites.Length == 0) return 0;
        
        healthPercentage = Mathf.Clamp01(healthPercentage);
        int maxIndex = healthBarSprites.Length - 1;
        int calculatedIndex = Mathf.RoundToInt(healthPercentage * maxIndex);
        
        if (reverseOrder)
        {
            calculatedIndex = maxIndex - calculatedIndex;
        }
        
        return Mathf.Clamp(calculatedIndex, 0, maxIndex);
    }
    
    public void SetPlayer(Player newPlayer)
    {
        // Unsubscribe from old player
        UnsubscribeFromPlayerEvents();
        
        player = newPlayer;
        Debug.Log($"PlayerHealthBarUI: SetPlayer called - player: {(player != null ? "SET" : "NULL")}");
        
        // Subscribe to new player
        SubscribeToPlayerEvents();
        
        UpdateHealthDisplay();
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
        
        Debug.Log($"Player health bar has {healthBarSprites.Length} sprites");
        for (int i = 0; i < healthBarSprites.Length; i++)
        {
            float testHealth = (float)i / (healthBarSprites.Length - 1);
            int spriteIndex = GetSpriteIndexFromHealth(testHealth);
            Debug.Log($"Health {testHealth:P0} → Sprite Index {spriteIndex}");
        }
    }
    
    [ContextMenu("Debug Player Health Status")]
    private void DebugPlayerHealthStatus()
    {
        Debug.Log("=== Player Health Bar Debug ===");
        Debug.Log($"Player: {(player != null ? "Connected" : "NULL")}");
        
        if (player != null)
        {
            Debug.Log($"Player HP: {player.HP}/{player.MaxHP}");
            Debug.Log($"PlayerHealth: {(player.PlayerHealth != null ? "Connected" : "NULL")}");
            
            if (player.PlayerHealth != null)
            {
                Debug.Log($"Health Percentage: {player.PlayerHealth.HealthPercentage:P1}");
                Debug.Log($"Is Alive: {player.PlayerHealth.IsAlive}");
            }
        }
        
        Debug.Log($"Health Bar Image: {(healthBarImage != null ? "Connected" : "NULL")}");
        Debug.Log($"Health Text: {(healthText != null ? "Connected" : "NULL")}");
        Debug.Log($"Health Bar Sprites: {(healthBarSprites != null ? healthBarSprites.Length.ToString() : "NULL")}");
        
        if (healthText != null)
        {
            Debug.Log($"Current Health Text: '{healthText.text}'");
        }
    }
    
    [ContextMenu("Test Damage Player")]
    private void TestDamagePlayer()
    {
        if (player != null)
        {
            Debug.Log("Testing player damage...");
            player.TakeDamage(10);
        }
        else
        {
            Debug.LogWarning("Cannot test damage - player is null!");
        }
    }
    
    // Validate setup in inspector
    private void OnValidate()
    {
        if (healthBarSprites != null && healthBarSprites.Length > 0)
        {
            for (int i = 0; i < healthBarSprites.Length; i++)
            {
                if (healthBarSprites[i] == null)
                {
                    Debug.LogWarning($"Player health bar sprite at index {i} is null!");
                }
            }
        }
    }
    
    #endregion
}
