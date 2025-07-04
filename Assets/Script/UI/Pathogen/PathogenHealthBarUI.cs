using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathogenHealthBarUI : MonoBehaviour
{
    [Header("Health Bar Components")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Health Bar Colors")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.3f;
    
    private PathogenHealth currentPathogenHealth;
    private PathogenData currentPathogenData;
    
    private void Start()
    {
        // Subscribe to PathogenManager events
        var pathogenManager = FindAnyObjectByType<PathogenManager>();
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned += OnPathogenSpawned;
            pathogenManager.OnPathogenDefeated += OnPathogenDefeated;
        }
        
        // Initially hide the UI
        gameObject.SetActive(false);
    }
    
    private void OnPathogenSpawned(Pathogen pathogen)
    {
        // Unsubscribe from previous pathogen if any
        UnsubscribeFromCurrentPathogen();
        
        currentPathogenHealth = pathogen.PathogenHealth;
        currentPathogenData = pathogen.PathogenData;
        
        // Subscribe to health events
        currentPathogenHealth.OnHealthChanged += UpdateHealthBar;
        currentPathogenHealth.OnDamageTaken += OnDamageTaken;
        currentPathogenHealth.OnPathogenDied += OnPathogenDied;
        
        // Initialize UI
        UpdatePathogenInfo();
        UpdateHealthBar(currentPathogenHealth.GetCurrentHealth());
        
        // Show the UI
        gameObject.SetActive(true);
    }
    
    private void OnPathogenDefeated(Pathogen pathogen)
    {
        UnsubscribeFromCurrentPathogen();
        
        // Hide the UI
        gameObject.SetActive(false);
    }
    
    private void UpdateHealthBar(int currentHealth)
    {
        if (currentPathogenHealth == null) return;
        
        int maxHealth = currentPathogenHealth.GetMaxHealth();
        float healthPercentage = currentPathogenHealth.GetHealthPercentage();
        
        // Update health bar
        if (healthBar != null)
        {
            healthBar.value = healthPercentage;
        }
        
        // Update health text
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
        
        // Update health bar color
        UpdateHealthBarColor(healthPercentage);
    }
    
    private void UpdateHealthBarColor(float healthPercentage)
    {
        if (healthBarFill != null)
        {
            if (healthPercentage <= lowHealthThreshold)
            {
                healthBarFill.color = lowHealthColor;
            }
            else
            {
                // Interpolate between low health and full health colors
                float normalizedHealth = (healthPercentage - lowHealthThreshold) / (1f - lowHealthThreshold);
                healthBarFill.color = Color.Lerp(lowHealthColor, fullHealthColor, normalizedHealth);
            }
        }
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
            currentPathogenHealth.OnHealthChanged -= UpdateHealthBar;
            currentPathogenHealth.OnDamageTaken -= OnDamageTaken;
            currentPathogenHealth.OnPathogenDied -= OnPathogenDied;
        }
        
        currentPathogenHealth = null;
        currentPathogenData = null;
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromCurrentPathogen();
    }
}
