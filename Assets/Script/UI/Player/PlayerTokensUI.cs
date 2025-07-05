using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerTokensUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI tokensText;
    [SerializeField] private Image tokensIcon;
    
    
    private int lastTokenCount = -1;
    private float lastRefreshTime = 0f;

    private void OnEnable()
    {
        if (player != null)
        {
            player.PlayerTokens.OnTokensChanged += CheckForTokenChanges;
        }
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.PlayerTokens.OnTokensChanged -= CheckForTokenChanges;
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
                
                // Subscribe to events if player was found
                if (player != null)
                {
                    player.PlayerTokens.OnTokensChanged += CheckForTokenChanges;
                }
            }
        }
        
        UpdateTokensDisplay();
    }
    
    private void CheckForTokenChanges(int newTokenCount)
    {
        if (player == null) return;
        
        if (newTokenCount != lastTokenCount)
        {
            UpdateTokensDisplay();
            lastTokenCount = newTokenCount;
        }
    }
    
    private void UpdateTokensDisplay()
    {
        if (player == null || tokensText == null) return;
        
        tokensText.text = $"{player.Tokens.ToString()} Tokens";
    }
    
    public void SetPlayer(Player newPlayer)
    {
        // Unsubscribe from old player if any
        if (player != null)
        {
            player.PlayerTokens.OnTokensChanged -= CheckForTokenChanges;
        }
        
        player = newPlayer;
        
        // Subscribe to new player if not null
        if (player != null)
        {
            player.PlayerTokens.OnTokensChanged += CheckForTokenChanges;
        }
        
        UpdateTokensDisplay();
    }
}
