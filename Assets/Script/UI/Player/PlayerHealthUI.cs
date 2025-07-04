using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Settings")]
    [SerializeField] private float refreshRate = 0.1f;
    
    private int lastHP = -1;
    private float lastRefreshTime = 0f;
    
    private void OnEnable()
    {
        if (player != null)
        {
            player.PlayerHealth.OnHealthChanged += CheckForHealthChanges;
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
            }
        }
        
        UpdateHealthDisplay();
    }
    
    private void CheckForHealthChanges(int newHP , int maxHP)
    {
        if (player == null) return;
        
        if (newHP != lastHP)
        {
            UpdateHealthDisplay();
            lastHP = newHP;
        }
    }
    
    private void UpdateHealthDisplay()
    {
        if (player == null) return;
        
        if (healthBar != null)
        {
            healthBar.maxValue = player.MaxHP;
            healthBar.value = player.HP;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{player.HP} HP";
        }
    }
    
    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
        UpdateHealthDisplay();
    }
}
