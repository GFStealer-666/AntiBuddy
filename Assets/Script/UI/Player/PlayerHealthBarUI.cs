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
    
    [Header("Settings")]
    [SerializeField] private float refreshRate = 0.1f;
    
    private int lastHP = -1;
    private float lastRefreshTime = 0f;
    
    private void OnEnable()
    {
        if (player != null)
        {
            player.PlayerHealth.OnHealthChanged += CheckForHealthChanges;
        }
    }
    
    private void OnDisable()
    {
        if (player != null)
        {
            player.PlayerHealth.OnHealthChanged -= CheckForHealthChanges;
        }
    }

    void Start()
    {
        if (player == null)
        {
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                player = gameManager.GetPlayer();
            }
        }
        
        UpdateHealthDisplay();
    }
    
    private void CheckForHealthChanges(int newHP, int maxHP)
    {
        if (player == null) return;
        
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
        player = newPlayer;
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
