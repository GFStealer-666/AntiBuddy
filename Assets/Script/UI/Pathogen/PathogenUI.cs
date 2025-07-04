using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathogenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image pathogenImage;
    [SerializeField] private Slider healthBar;
    
    private Pathogen currentPathogen;
    
    private void Start()
    {
        // Subscribe to PathogenManager events
        FindAnyObjectByType<PathogenManager>().OnPathogenSpawned += DisplayPathogen;
        FindAnyObjectByType<PathogenManager>().OnPathogenDefeated += ClearDisplay;
    }
    
    public void DisplayPathogen(Pathogen pathogen)
    {
        currentPathogen = pathogen;
        
        // Subscribe to pathogen events
        pathogen.OnHealthChanged += UpdateHealth;
        
        // Update UI
        pathogenImage.sprite = pathogen.GetSprite();
        
        UpdateHealth(pathogen.GetCurrentHealth());
    }
    
    private void UpdateHealth(int currentHealth)
    {
        if (currentPathogen == null) return;
        
        healthBar.value = currentPathogen.GetHealthPercentage();
    }

    
    private void ClearDisplay(Pathogen pathogen)
    {
        // Clear UI when pathogen dies
        pathogenImage.sprite = null;
        healthBar.value = 0;
    }
}
