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
            pathogenManager.OnAllPathogensDefeated += ClearDisplayOnGameEnd;
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
            pathogenManager.OnAllPathogensDefeated -= ClearDisplayOnGameEnd;
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
    
    
    private void ClearDisplayOnGameEnd()
    {
        Debug.Log("PathogenUI: Game ended - clearing pathogen display");
        if (pathogenImage != null)
        {
        }
        currentPathogen = null;
    }
}
