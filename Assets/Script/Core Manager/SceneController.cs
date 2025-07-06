using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Scene Manager for handling scene transitions in the Immunity Board Game
/// Manages loading between Main Menu, Game Scene, and other scenes using build indices
/// </summary>
public class SceneController : MonoBehaviour
{
    [Header("Scene Build Indices")]
    [SerializeField] private int mainMenuSceneIndex = 0;
    [SerializeField] private int gameSceneIndex = 1;
    
    [Header("Loading Settings")]
    [SerializeField] private bool useLoadingScreen = false;
    [SerializeField] private float minLoadingTime = 1f; // Minimum time to show loading
    
    // Singleton instance for easy access
    public static SceneController Instance { get; private set; }
    
    // Events for scene transitions
    public static System.Action<int> OnSceneLoadStarted;
    public static System.Action<int> OnSceneLoadCompleted;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    #region Public Scene Loading Methods
    
    /// <summary>
    /// Load the main menu scene
    /// </summary>
    public void LoadMainMenu()
    {
        Debug.Log("SceneController: Loading Main Menu");
        LoadScene(mainMenuSceneIndex);
    }
    
    /// <summary>
    /// Load the main game scene
    /// </summary>
    public void LoadGameScene()
    {
        Debug.Log("SceneController: Loading Game Scene");
        LoadScene(gameSceneIndex);
    }
    
    
    /// <summary>
    /// Load scene by build index
    /// </summary>
    /// <param name="sceneIndex">Build index of the scene to load</param>
    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"SceneController: Invalid scene index {sceneIndex}. Valid range: 0-{SceneManager.sceneCountInBuildSettings - 1}");
            return;
        }
        
        OnSceneLoadStarted?.Invoke(sceneIndex);
        
        if (useLoadingScreen)
        {
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
            OnSceneLoadCompleted?.Invoke(sceneIndex);
        }
    }
    
    /// <summary>
    /// Load scene by name
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    public void LoadScene(string sceneName)
    {
        Debug.Log($"SceneController: Loading scene '{sceneName}'");
        
        if (useLoadingScreen)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    
    /// <summary>
    /// Restart the current scene
    /// </summary>
    public void RestartCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log($"SceneController: Restarting current scene (index: {currentSceneIndex})");
        LoadScene(currentSceneIndex);
    }
    
    /// <summary>
    /// Quit the application
    /// </summary>
    public void QuitApplication()
    {
        Debug.Log("SceneController: Quitting application");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    #endregion
    
    #region Async Loading (with optional loading screen)
    
    /// <summary>
    /// Load scene asynchronously by index
    /// </summary>
    /// <param name="sceneIndex">Build index of scene to load</param>
    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        float startTime = Time.time;
        
        // Start loading the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;
        
        // Wait for loading to complete (but don't activate yet)
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100f}%");
            yield return null;
        }
        
        // Ensure minimum loading time if specified
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < minLoadingTime)
        {
            yield return new WaitForSeconds(minLoadingTime - elapsedTime);
        }
        
        // Activate the scene
        asyncLoad.allowSceneActivation = true;
        
        // Wait for scene to actually load
        yield return asyncLoad;
        
        OnSceneLoadCompleted?.Invoke(sceneIndex);
        Debug.Log($"SceneController: Scene {sceneIndex} loaded successfully");
    }
    
    /// <summary>
    /// Load scene asynchronously by name
    /// </summary>
    /// <param name="sceneName">Name of scene to load</param>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        float startTime = Time.time;
        
        // Start loading the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        // Wait for loading to complete (but don't activate yet)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        
        // Ensure minimum loading time if specified
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < minLoadingTime)
        {
            yield return new WaitForSeconds(minLoadingTime - elapsedTime);
        }
        
        // Activate the scene
        asyncLoad.allowSceneActivation = true;
        
        // Wait for scene to actually load
        yield return asyncLoad;
        
        Debug.Log($"SceneController: Scene '{sceneName}' loaded successfully");
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Get the current scene build index
    /// </summary>
    /// <returns>Current scene build index</returns>
    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    
    /// <summary>
    /// Get the current scene name
    /// </summary>
    /// <returns>Current scene name</returns>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    
    /// <summary>
    /// Check if we're currently in the main menu
    /// </summary>
    /// <returns>True if in main menu scene</returns>
    public bool IsInMainMenu()
    {
        return GetCurrentSceneIndex() == mainMenuSceneIndex;
    }
    
    /// <summary>
    /// Check if we're currently in the game scene
    /// </summary>
    /// <returns>True if in game scene</returns>
    public bool IsInGameScene()
    {
        return GetCurrentSceneIndex() == gameSceneIndex;
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Load Main Menu")]
    public void DebugLoadMainMenu()
    {
        LoadMainMenu();
    }
    
    [ContextMenu("Load Game Scene")]
    public void DebugLoadGameScene()
    {
        LoadGameScene();
    }
    
    [ContextMenu("Restart Current Scene")]
    public void DebugRestartScene()
    {
        RestartCurrentScene();
    }
    
    [ContextMenu("Print Current Scene Info")]
    public void DebugPrintSceneInfo()
    {
        Debug.Log($"Current Scene: {GetCurrentSceneName()} (Index: {GetCurrentSceneIndex()})");
        Debug.Log($"Total Scenes in Build: {SceneManager.sceneCountInBuildSettings}");
    }
    
    #endregion
}
