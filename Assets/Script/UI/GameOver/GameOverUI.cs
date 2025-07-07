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
    [SerializeField] private GameObject victoryImage; // GameObject for victory screen
    [SerializeField] private GameObject defeatImage; // GameObject for defeat screen
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    
    [Header("Text Settings")]
    [SerializeField] private string victoryTitle = "VICTORY!";
    [SerializeField] private string defeatTitle = "GAME OVER";
    [SerializeField] private string victoryMessage = "You have successfully eliminated all pathogens and saved the day!";
    [SerializeField] private string defeatMessage = "You have been defeated by the pathogens. Try again!";
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 2f; // Duration for color fade from black to white
    
    private PathogenManager pathogenManager;
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
        if(pathogenManager == null)
        {
            // Try to find PathogenManager if not assigned
            pathogenManager = FindAnyObjectByType<PathogenManager>();
            if (pathogenManager == null)
            {
                Debug.LogError("PathogenManager not found! GameOverUI will not function correctly.");
            }
        }
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
            bool isVictory = !pathogenManager.HasActivePathogens();
            
            ShowGameOver(isVictory);
        }
    }
    
    /// <summary>
    /// Show the game over screen with appropriate victory/defeat message
    /// </summary>
    /// <param name="isVictory">True if player won, false if player lost</param>
    public void ShowGameOver(bool isVictory)
    {
        // Update content first (while panel is still hidden)
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
        
        // Update images
        // Activate the appropriate image
        if (victoryImage != null)
        {
            victoryImage.SetActive(isVictory);
        }
        if (defeatImage != null)
        {
            defeatImage.SetActive(!isVictory);
        }
        
        // Show panel with fade animation
        if (gameOverPanel != null)
        {
            StartCoroutine(FadeInGameOverPanel());
        }
        
        Debug.Log($"GameOverUI: Showing {(isVictory ? "victory" : "defeat")} screen");
    }
    
    /// <summary>
    /// Show the game over screen with custom message
    /// </summary>
    /// <param name="isVictory">True if player won, false if player lost</param>
    /// <param name="customMessage">Custom message to display instead of default</param>
    public void ShowGameOver(bool isVictory, string customMessage)
    {
        // Update content first (while panel is still hidden)
        // Update title
        if (titleText != null)
        {
            titleText.text = isVictory ? victoryTitle : defeatTitle;
        }
        
        // Update message with custom text
        if (messageText != null)
        {
            messageText.text = customMessage ?? (isVictory ? victoryMessage : defeatMessage);
        }
        
        // Update images
        // Activate the appropriate image
        if (victoryImage != null)
        {
            victoryImage.SetActive(isVictory);
        }
        if (defeatImage != null)
        {
            defeatImage.SetActive(!isVictory);
        }
        
        // Show panel with fade animation
        if (gameOverPanel != null)
        {
            StartCoroutine(FadeInGameOverPanel());
        }
        
        Debug.Log($"GameOverUI: Showing {(isVictory ? "victory" : "defeat")} screen with custom message: {customMessage}");
    }
    
    /// <summary>
    /// Hide the game over screen
    /// </summary>
    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            
            // Get the active image (victory or defeat)
            Image activeImage = victoryImage.activeSelf ? victoryImage.GetComponent<Image>() : defeatImage.GetComponent<Image>();
            if (activeImage != null)
            {
                // Reset color to white
                activeImage.color = Color.white;
            }
        }
        
        // Deactivate both images
        if (victoryImage != null)
        {
            victoryImage.SetActive(false);
        }
        if (defeatImage != null)
        {
            defeatImage.SetActive(false);
        }
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
    
    #region Animation Methods
    
    /// <summary>
    /// Fade in the entire game over panel from black to visible over the specified duration
    /// </summary>
    private System.Collections.IEnumerator FadeInGameOverPanel()
    {
        if (gameOverPanel == null) yield break;
        
        // Activate the panel first
        gameOverPanel.SetActive(true);
        
        // Get the active image (victory or defeat)
        Image activeImage = victoryImage.activeSelf ? victoryImage.GetComponent<Image>() : defeatImage.GetComponent<Image>();
        if (activeImage == null) yield break;

        float elapsedTime = 0f;

        // Start with black color
        Color startColor = Color.black;
        Color endColor = Color.white;
        activeImage.color = startColor;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time in case game is paused
            float progress = elapsedTime / fadeInDuration;

            // Smooth lerp from black to white
            activeImage.color = Color.Lerp(startColor, endColor, progress);

            yield return null; // Wait for next frame
        }

        // Ensure final color is exactly white
        activeImage.color = endColor;
    }
    
    #endregion

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
    
    [ContextMenu("Test Fade Animation")]
    public void TestFadeAnimation()
    {
        if (gameOverPanel != null)
        {
            StartCoroutine(FadeInGameOverPanel());
        }
    }
    
    #endregion
}
