using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Base class for all sprite-based health bars
/// Handles common sprite selection and rendering logic
/// Follows SOLID principles by extracting shared functionality
/// </summary>
public abstract class SpriteHealthBarUI : MonoBehaviour
{
    [Header("Health Bar Components")]
    [SerializeField] protected Image healthBarImage; // Single image that will display different sprites
    [SerializeField] protected TextMeshProUGUI healthText;
    
    [Header("Health Bar Sprites")]
    [SerializeField] protected Sprite[] healthBarSprites; // Array of sprites from full to empty health
    [SerializeField] protected bool reverseOrder = false; // If true: index 0 = empty, last index = full
    
    #region Protected Methods - For Derived Classes
    
    /// <summary>
    /// Update the health bar display with current health values
    /// </summary>
    /// <param name="currentHealth">Current health value</param>
    /// <param name="maxHealth">Maximum health value</param>
    /// <param name="healthText">Text to display (e.g., "100 HP")</param>
    protected void UpdateHealthBarDisplay(int currentHealth, int maxHealth, string healthText = null)
    {
        if (healthBarSprites == null || healthBarSprites.Length == 0) return;
        
        // Calculate health percentage
        float healthPercentage = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
        
        // Update sprite
        UpdateHealthBarSprite(healthPercentage);
        
        // Update text
        UpdateHealthText(healthText ?? $"{currentHealth} HP");
    }
    
    /// <summary>
    /// Update health bar sprite based on health percentage
    /// </summary>
    /// <param name="healthPercentage">Health as percentage (0.0 to 1.0)</param>
    protected void UpdateHealthBarSprite(float healthPercentage)
    {
        if (healthBarImage == null || healthBarSprites == null || healthBarSprites.Length == 0) return;
        
        int spriteIndex = GetSpriteIndexFromHealth(healthPercentage);
        
        if (spriteIndex >= 0 && spriteIndex < healthBarSprites.Length)
        {
            healthBarImage.sprite = healthBarSprites[spriteIndex];
        }
    }
    
    /// <summary>
    /// Update the health text display
    /// </summary>
    /// <param name="text">Text to display</param>
    protected void UpdateHealthText(string text)
    {
        if (healthText != null)
        {
            healthText.text = text;
        }
    }
    
    #endregion
    
    #region Private Methods
    
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
    
    #endregion
    
    #region Validation & Debug
    
    [ContextMenu("Test Health Bar Sprites")]
    private void TestHealthBarSprites()
    {
        if (healthBarSprites == null || healthBarSprites.Length == 0)
        {
            Debug.LogWarning($"No health bar sprites assigned on {gameObject.name}!");
            return;
        }
        
        Debug.Log($"{gameObject.name} health bar has {healthBarSprites.Length} sprites");
        for (int i = 0; i < healthBarSprites.Length; i++)
        {
            float testHealth = (float)i / (healthBarSprites.Length - 1);
            int spriteIndex = GetSpriteIndexFromHealth(testHealth);
            Debug.Log($"Health {testHealth:P0} → Sprite Index {spriteIndex}");
        }
    }
    
    // Validate setup in inspector
    protected virtual void OnValidate()
    {
        if (healthBarSprites != null && healthBarSprites.Length > 0)
        {
            // Check for null sprites
            for (int i = 0; i < healthBarSprites.Length; i++)
            {
                if (healthBarSprites[i] == null)
                {
                    Debug.LogWarning($"{gameObject.name} health bar sprite at index {i} is null!");
                }
            }
        }
    }
    
    #endregion
}
