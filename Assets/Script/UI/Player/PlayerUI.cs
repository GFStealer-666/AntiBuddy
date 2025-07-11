using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private PlayerHealthBarUI healthUI;
    [SerializeField] private PlayerTokensUI tokensUI;
    [SerializeField] private PlayerHandUI handUI;
    
    private Player player;
    
    void Start()
    {
        // Find player
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            player = gameManager.GetPlayer();
            SetPlayerForAllComponents();
        }
        
        // Auto-find components if not assigned
        if (healthUI == null) healthUI = GetComponentInChildren<PlayerHealthBarUI>();
        if (tokensUI == null) tokensUI = GetComponentInChildren<PlayerTokensUI>();
        if (handUI == null) handUI = GetComponentInChildren<PlayerHandUI>();
    }
    
    private void SetPlayerForAllComponents()
    {
        if (player == null) return;
        
        if (healthUI != null) healthUI.SetPlayer(player);
        if (tokensUI != null) tokensUI.SetPlayer(player);
        if (handUI != null) handUI.SetPlayer(player);
    }
    
    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
        SetPlayerForAllComponents();
    }

    public PlayerHandUI HandUI => handUI;
}
