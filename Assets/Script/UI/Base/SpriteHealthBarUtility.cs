using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Utility class for sprite-based health bar functionality
/// Provides static methods that can be used by any health bar UI
/// Follows SOLID principles by providing reusable functionality
/// </summary>
public static class SpriteHealthBarUtility
{
    /// <summary>
    /// Update the health bar display with current health values
    /// </summary>
    /// <param name="currentHealth">Current health value</param>
    /// <param name="maxHealth">Maximum health value</param>
    /// <param name="healthBarImage">Image component to update</param>
    /// <param name="healthText">Text component to update</param>
    /// <param name="healthBarSprites">Array of health bar sprites</param>
    /// <param name="reverseOrder">Whether sprite order is reversed</param>
    /// <param name="displayText">Text to display (optional)</param>
    public static void UpdateHealthBarDisplay(
        int currentHealth, 
        int maxHealth, 
        Image healthBarImage, 
        TextMeshProUGUI healthText, 
        Sprite[] healthBarSprites, 
        bool reverseOrder = false,
        string displayText = null)
    {
        if (healthBarSprites == null || healthBarSprites.Length == 0) return;
        
        // Calculate health percentage
        float healthPercentage = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
        
        // Update sprite
        UpdateHealthBarSprite(healthBarImage, healthBarSprites, healthPercentage, reverseOrder);
        
        // Update text
        UpdateHealthText(healthText, displayText ?? $"{currentHealth} HP");
    }
    
    /// <summary>
    /// Update health bar sprite based on health percentage
    /// </summary>
    /// <param name="healthBarImage">Image component to update</param>
    /// <param name="healthBarSprites">Array of health bar sprites</param>
    /// <param name="healthPercentage">Health as percentage (0.0 to 1.0)</param>
    /// <param name="reverseOrder">Whether sprite order is reversed</param>
    public static void UpdateHealthBarSprite(Image healthBarImage, Sprite[] healthBarSprites, float healthPercentage, bool reverseOrder = false)
    {
        if (healthBarImage == null || healthBarSprites == null || healthBarSprites.Length == 0) return;
        
        int spriteIndex = GetSpriteIndexFromHealth(healthPercentage, healthBarSprites.Length, reverseOrder);
        
        if (spriteIndex >= 0 && spriteIndex < healthBarSprites.Length)
        {
            healthBarImage.sprite = healthBarSprites[spriteIndex];
        }
    }
    
    /// <summary>
    /// Update the health text display
    /// </summary>
    /// <param name="healthText">Text component to update</param>
    /// <param name="text">Text to display</param>
    public static void UpdateHealthText(TextMeshProUGUI healthText, string text)
    {
        if (healthText != null)
        {
            healthText.text = text;
        }
    }
    
    /// <summary>
    /// Convert health percentage to sprite array index
    /// </summary>
    /// <param name="healthPercentage">Health as percentage (0.0 to 1.0)</param>
    /// <param name="spriteCount">Number of sprites in array</param>
    /// <param name="reverseOrder">Whether sprite order is reversed</param>
    /// <returns>Index in the sprite array</returns>
    public static int GetSpriteIndexFromHealth(float healthPercentage, int spriteCount, bool reverseOrder = false)
    {
        if (spriteCount <= 0) return 0;
        
        // Clamp health percentage between 0 and 1
        healthPercentage = Mathf.Clamp01(healthPercentage);
        
        // Calculate index based on health percentage
        // If we have 5 sprites: 0%, 25%, 50%, 75%, 100%
        int maxIndex = spriteCount - 1;
        int calculatedIndex = Mathf.RoundToInt(healthPercentage * maxIndex);
        
        // Handle reverse order if needed
        if (reverseOrder)
        {
            calculatedIndex = maxIndex - calculatedIndex;
        }
        
        return Mathf.Clamp(calculatedIndex, 0, maxIndex);
    }
    
    /// <summary>
    /// Validate sprite array and log warnings for null sprites
    /// </summary>
    /// <param name="healthBarSprites">Array to validate</param>
    /// <param name="objectName">Name of the object for logging</param>
    public static void ValidateSpriteArray(Sprite[] healthBarSprites, string objectName = "HealthBar")
    {
        if (healthBarSprites != null && healthBarSprites.Length > 0)
        {
            // Check for null sprites
            for (int i = 0; i < healthBarSprites.Length; i++)
            {
                if (healthBarSprites[i] == null)
                {
                    Debug.LogWarning($"{objectName} health bar sprite at index {i} is null!");
                }
            }
        }
    }
    
    /// <summary>
    /// Test and log sprite mapping for debugging
    /// </summary>
    /// <param name="healthBarSprites">Array to test</param>
    /// <param name="objectName">Name of the object for logging</param>
    /// <param name="reverseOrder">Whether sprite order is reversed</param>
    public static void TestSpriteMapping(Sprite[] healthBarSprites, string objectName = "HealthBar", bool reverseOrder = false)
    {
        if (healthBarSprites == null || healthBarSprites.Length == 0)
        {
            Debug.LogWarning($"No health bar sprites assigned on {objectName}!");
            return;
        }
        
        Debug.Log($"{objectName} health bar has {healthBarSprites.Length} sprites");
        for (int i = 0; i < healthBarSprites.Length; i++)
        {
            float testHealth = (float)i / (healthBarSprites.Length - 1);
            int spriteIndex = GetSpriteIndexFromHealth(testHealth, healthBarSprites.Length, reverseOrder);
            Debug.Log($"Health {testHealth:P0} → Sprite Index {spriteIndex}");
        }
    }
}
