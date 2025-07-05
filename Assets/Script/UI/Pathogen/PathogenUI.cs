using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathogenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image pathogenImage;
    
    private Pathogen currentPathogen;
    private PathogenManager pathogenManager;

    private void OnEnable()
    {
        // Cache the PathogenManager reference and add null check
        pathogenManager = FindAnyObjectByType<PathogenManager>();
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned += DisplayPathogen;
            pathogenManager.OnPathogenDefeated += ClearDisplay;
        }
        else
        {
            Debug.LogWarning("PathogenUI: PathogenManager not found!");
        }
    }
    
    private void OnDisable()
    {
        // Use cached reference and add null check
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned -= DisplayPathogen;
            pathogenManager.OnPathogenDefeated -= ClearDisplay;
        }
    }
    
    public void DisplayPathogen(Pathogen pathogen)
    {
        currentPathogen = pathogen;
        Debug.Log($"Displaying pathogen: {pathogen.GetPathogenName()}");
        
        if (pathogenImage != null)
        {
            pathogenImage.sprite = pathogen.GetSprite();
        }
        else
        {
            Debug.LogWarning("PathogenUI: pathogenImage is null!");
        }
    }
    
    private void ClearDisplay(Pathogen pathogen)
    {
        // Clear UI when pathogen dies
        if (pathogenImage != null)
        {
            pathogenImage.sprite = null;
        }
        
        currentPathogen = null;
    }
}
