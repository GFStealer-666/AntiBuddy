using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component for displaying pathogen information and handling card targeting
/// </summary>
public class PathogenUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PathogenSO pathogenData;
    
    [Header("UI Components")]
    [SerializeField] private Image pathogenImage;
    [SerializeField] private TextMeshProUGUI pathogenNameText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private GameObject targetHighlight;
    
    [Header("Visual Effects")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color targetedColor = Color.red;
    [SerializeField] private Color damagedColor = Color.red;
    [SerializeField] private float damageFlashDuration = 0.5f;
    
    private int maxHealth;
    private bool isTargeted = false;
    
    public System.Action<PathogenSO> OnPathogenClicked;
    public System.Action<PathogenSO> OnPathogenDestroyed;
    
    #region Initialization
    
    public void Initialize(PathogenSO pathogen)
    {
        pathogenData = pathogen;
        if (pathogenData != null)
        {
            maxHealth = pathogenData.maxHitPoints;
            UpdateDisplay();
        }
    }
    
    #endregion
    
    #region Display Updates
    
    public void UpdateDisplay()
    {
        if (pathogenData == null) return;
        
        // Update text displays
        if (pathogenNameText != null)
            pathogenNameText.text = pathogenData.pathogenName;
            
        if (healthText != null)
            healthText.text = $"{pathogenData.maxHitPoints}/{maxHealth}";
            
        if (attackText != null)
            attackText.text = pathogenData.attackPower.ToString();
        
        // Update health bar
        if (healthBar != null)
        {
            float healthPercentage = maxHealth > 0 ? (float)pathogenData.maxHitPoints / maxHealth : 0f;
            healthBar.value = healthPercentage;
            
            // Update health bar color based on health percentage
            if (healthBarFill != null)
            {
                if (healthPercentage > 0.6f)
                    healthBarFill.color = Color.green;
                else if (healthPercentage > 0.3f)
                    healthBarFill.color = Color.yellow;
                else
                    healthBarFill.color = Color.red;
            }
        }
        
        // Check if pathogen is destroyed
        if (pathogenData.maxHitPoints <= 0)
        {
            OnPathogenDestroyed?.Invoke(pathogenData);
        }
    }
    
    #endregion
    
    #region Targeting
    
    public void SetTargeted(bool targeted)
    {
        isTargeted = targeted;
        
        if (targetHighlight != null)
            targetHighlight.SetActive(isTargeted);
            
        // Update visual state
        UpdateVisualState();
    }
    
    private void UpdateVisualState()
    {
        if (pathogenImage == null) return;
        
        Color targetColor = isTargeted ? targetedColor : normalColor;
        pathogenImage.color = targetColor;
    }
    
    #endregion
    
    #region Damage Effects
    
    public void OnTakeDamage(int damage)
    {
        UpdateDisplay();
        
        // Flash damage effect
        StartCoroutine(DamageFlashEffect());
        
        // You can add more visual effects here like:
        // - Damage number popup
        // - Screen shake
        // - Particle effects
    }
    
    private System.Collections.IEnumerator DamageFlashEffect()
    {
        if (pathogenImage == null) yield break;
        
        Color originalColor = pathogenImage.color;
        pathogenImage.color = damagedColor;
        
        yield return new WaitForSeconds(damageFlashDuration);
        
        pathogenImage.color = originalColor;
    }
    
    #endregion
    
    #region Input Handling
    
    public void OnClick()
    {
        OnPathogenClicked?.Invoke(pathogenData);
    }
    
    #endregion
    
    #region Public Methods
    
    public PathogenSO GetPathogen()
    {
        return pathogenData;
    }
    
    public bool IsAlive()
    {
        return pathogenData != null && pathogenData.maxHitPoints > 0;
    }
    
    #endregion
}
