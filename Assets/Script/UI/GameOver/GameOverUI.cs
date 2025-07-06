using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// UI component for displaying game over/victory screen
/// Shows appropriate message and provides options to restart or quit
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    
    [Header("Text Settings")]
    [SerializeField] private string victoryTitle = "VICTORY!";
    [SerializeField] private string defeatTitle = "GAME OVER";
    [SerializeField] private string victoryMessage = "You have successfully eliminated all pathogens and saved the day!";
    [SerializeField] private string defeatMessage = "You have been defeated by the pathogens. Try again!";
    
    
    void Awake()
    {
        // Setup button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(LoadMainMenu);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        // Subscribe to turn manager events
        TurnManager.OnTurnPhaseChanged += OnTurnPhaseChanged;
        
        // Start hidden
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    
    void OnDestroy()
    {
        // Clean up listeners
        if (restartButton != null)
            restartButton.onClick.RemoveAllListeners();
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveAllListeners();
            
        if (quitButton != null)
            quitButton.onClick.RemoveAllListeners();
            
        // Unsubscribe from events
        TurnManager.OnTurnPhaseChanged -= OnTurnPhaseChanged;
    }
    
    private void OnTurnPhaseChanged(TurnPhase phase)
    {
        if (phase == TurnPhase.GameOver)
        {
            // Determine if victory or defeat
            GameStateManager gameState = GameStateManager.Instance;
            bool isVictory = gameState != null && gameState.GetCurrentState() == GameState.Victory;
            
            ShowGameOver(isVictory);
        }
    }
    
    /// <summary>
    /// Show the game over screen with appropriate victory/defeat message
    /// </summary>
    /// <param name="isVictory">True if player won, false if player lost</param>
    public void ShowGameOver(bool isVictory)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        // Update title
        if (titleText != null)
        {
            titleText.text = isVictory ? victoryTitle : defeatTitle;
        }
        
        // Update message
        if (messageText != null)
        {
            messageText.text = isVictory ? victoryMessage : defeatMessage;
        }
        
        Debug.Log($"GameOverUI: Showing {(isVictory ? "victory" : "defeat")} screen");
    }
    
    /// <summary>
    /// Hide the game over screen
    /// </summary>
    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    
    /// <summary>
    /// Restart the current game scene
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("GameOverUI: Restarting game");
        
        // Use SceneController if available, otherwise fallback to direct SceneManager
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadGameScene();
        }
        else
        {
            // Fallback to direct scene loading
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
    
    /// <summary>
    /// Load main menu scene
    /// </summary>
    public void LoadMainMenu()
    {
        Debug.Log("GameOverUI: Loading main menu");
        
        // Use SceneController if available
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadMainMenu();
        }
        else
        {
            Debug.LogWarning("SceneController not found! Cannot load main menu.");
        }
    }
    
    /// <summary>
    /// Quit the game application
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("GameOverUI: Quitting game");
        
        // Use SceneController if available, otherwise direct quit
        if (SceneController.Instance != null)
        {
            SceneController.Instance.QuitApplication();
        }
        else
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
    
    #region Debug Methods
    
    [ContextMenu("Test Victory Screen")]
    public void TestVictoryScreen()
    {
        ShowGameOver(true);
    }
    
    [ContextMenu("Test Defeat Screen")]
    public void TestDefeatScreen()
    {
        ShowGameOver(false);
    }
    
    [ContextMenu("Hide Screen")]
    public void TestHideScreen()
    {
        HideGameOver();
    }
    
    #endregion
}
