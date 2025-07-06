using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI controller for the main menu scene
/// Handles navigation to game scene, settings, and quit
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    
    void Awake()
    {
        // Setup button listeners
        if (playButton != null)
            playButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }
    
    void OnDestroy()
    {
        // Clean up listeners
        if (playButton != null)
            playButton.onClick.RemoveAllListeners();
            
        if (settingsButton != null)
            settingsButton.onClick.RemoveAllListeners();
            
        if (quitButton != null)
            quitButton.onClick.RemoveAllListeners();
    }
    
    /// <summary>
    /// Start the game - load the game scene
    /// </summary>
    public void StartGame()
    {
        Debug.Log("MainMenuUI: Starting game");
        
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadGameScene();
        }
        else
        {
            Debug.LogError("SceneController not found! Cannot start game.");
        }
    }
    
    /// <summary>
    /// Open settings menu
    /// </summary>
    
    /// <summary>
    /// Quit the application
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("MainMenuUI: Quitting game");
        
        if (SceneController.Instance != null)
        {
            SceneController.Instance.QuitApplication();
        }
        else
        {
            // Fallback quit
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
    
    #region Debug Methods
    
    [ContextMenu("Test Start Game")]
    public void TestStartGame()
    {
        StartGame();
    }
    

    
    [ContextMenu("Test Quit")]
    public void TestQuit()
    {
        QuitGame();
    }
    
    #endregion
}
