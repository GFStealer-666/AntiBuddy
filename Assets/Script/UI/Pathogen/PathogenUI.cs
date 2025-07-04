using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathogenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image pathogenImage;
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
        // Update UI
        pathogenImage.sprite = pathogen.GetSprite();
    }
    

    
    private void ClearDisplay(Pathogen pathogen)
    {
        // Clear UI when pathogen dies
        pathogenImage.sprite = null;
    }
}
