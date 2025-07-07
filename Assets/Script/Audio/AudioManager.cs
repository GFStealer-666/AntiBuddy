using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamageAudioClips
{
    [Header("Player Damage Sounds")]
    public AudioClip[] playerDamageSounds;
    
    [Header("Pathogen Damage Sounds")]
    public AudioClip[] pathogenDamageSounds;

}

/// <summary>
/// Manages all audio in the game with separate audio sources for different audio types.
/// 
/// SETUP:
/// 1. Add this script to a GameObject in your scene
/// 2. Assign audio clips in the inspector (DamageAudioClips section)
/// 3. AudioSources are created automatically - no need to assign them manually
/// 4. Volume is set to 0.08 by default (good for most systems)
/// 5. Pitch variation is disabled by default (set to 0)
/// 
/// FEATURES:
/// - Separate AudioSources for Player, Pathogen, and UI sounds
/// - Volume controls for different sound types
/// - Optional pitch variation (disabled by default)
/// - Singleton pattern - automatically creates itself if not found
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioSource pathogenAudioSource;
    
    [Header("Audio Clips")]
    [SerializeField] private DamageAudioClips damageClips;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 0.08f;
    [Range(0f, 1f)] public float playerDamageVolume = 1f;
    [Range(0f, 1f)] public float pathogenDamageVolume = 1f;
    [Range(0f, 1f)] public float healingVolume = 1f;
    
    [Header("Pitch Variation")]
    [Range(0f, 0.5f)] public float pitchVariation = 0f;
    
    [Header("Background Music")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioClip defaultBGM;
    [SerializeField] private AudioClip victoryBGM;
    [SerializeField] private AudioClip defeatBGM;
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<AudioManager>();
                if (instance == null)
                {
                    GameObject audioManagerObj = new GameObject("AudioManager");
                    instance = audioManagerObj.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }
    void OnEnable()
    {
        TurnManager.OnTurnPhaseChanged += OnTurnPhaseChanged;
        
    }
    void OnDisable()
    {
        TurnManager.OnTurnPhaseChanged -= OnTurnPhaseChanged;
    }

    void Awake()
    {
        PlayDefaultBGM();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void OnTurnPhaseChanged(TurnPhase phase)
    {
        Debug.Log("AudioManager: Entering OnTurnPhaseChanged method");
        
        if(phase == TurnPhase.GameOver)
        {
            // Handle game over phase
            GameStateManager gameState = GameStateManager.Instance;
            Debug.Log($"AudioManager: Current game state = {gameState?.GetCurrentState()}");
            
            if (gameState != null && gameState.GetCurrentState() == GameState.Victory)
            {
                PlayVictoryBGM();
            }
            else
            {
                PlayDefeatBGM();
            }
        }
        Debug.Log($"AudioManager: Turn phase changed to {phase}");
    }
    
    private void InitializeAudioSources()
    {
        // Create audio sources if they don't exist
        if (playerAudioSource == null)
        {
            playerAudioSource = CreateAudioSource("PlayerAudioSource");
        }

        if (pathogenAudioSource == null)
        {
            pathogenAudioSource = CreateAudioSource("PathogenAudioSource");
        }


        if (bgmAudioSource == null)
        {
            bgmAudioSource = CreateAudioSource("BGMAudioSource");
        }
    }
    
    private AudioSource CreateAudioSource(string name)
    {
        GameObject audioObj = new GameObject(name);
        audioObj.transform.SetParent(transform);
        AudioSource source = audioObj.AddComponent<AudioSource>();
        
        // Configure audio source
        source.playOnAwake = false;
        source.volume = masterVolume;
        source.pitch = 1f; // No pitch variation by default
        
        return source;
    }
    void Start()
    {
        PlayDefaultBGM();
    }
    #region Player Damage Audio

    public void PlayPlayerDamageSound(int damage)
    {
        if (damageClips.playerDamageSounds == null || damageClips.playerDamageSounds.Length == 0)
        {
            Debug.LogWarning("No player damage sounds assigned!");
            return;
        }
        
        // Choose random damage sound
        AudioClip clip = damageClips.playerDamageSounds[Random.Range(0, damageClips.playerDamageSounds.Length)];
        
        PlayAudioClip(playerAudioSource, clip, playerDamageVolume);
        
        Debug.Log($"AudioManager: Playing player damage sound for {damage} damage");
    }
    

    
    #endregion
    
    #region Pathogen Damage Audio
    
    public void PlayPathogenDamageSound(string pathogenName, int damage)
    {
        if (damageClips.pathogenDamageSounds == null || damageClips.pathogenDamageSounds.Length == 0)
        {
            Debug.LogWarning("No pathogen damage sounds assigned!");
            return;
        }
        
        // Choose random damage sound
        AudioClip clip = damageClips.pathogenDamageSounds[Random.Range(0, damageClips.pathogenDamageSounds.Length)];
        
        PlayAudioClip(pathogenAudioSource, clip, pathogenDamageVolume);
        
        Debug.Log($"AudioManager: Playing pathogen damage sound for {pathogenName} taking {damage} damage");
    }
    
    #endregion
    
    #region Audio Playback
    
    private void PlayAudioClip(AudioSource source, AudioClip clip, float volume, bool randomPitch = false)
    {
        if (source == null || clip == null) return;
        
        // Set volume
        source.volume = volume * masterVolume;
        
        // Add pitch variation for more dynamic sound (disabled by default)
        if (randomPitch && pitchVariation > 0f)
        {
            source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        }
        else
        {
            source.pitch = 1f;
        }
        
        // Play the clip
        source.PlayOneShot(clip);
    }
    
    #endregion
    
    #region Volume Controls
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAudioSourceVolumes();
    }
    
    public void SetPlayerDamageVolume(float volume)
    {
        playerDamageVolume = Mathf.Clamp01(volume);
    }
    
    public void SetPathogenDamageVolume(float volume)
    {
        pathogenDamageVolume = Mathf.Clamp01(volume);
    }
    
    private void UpdateAudioSourceVolumes()
    {
        if (playerAudioSource != null) playerAudioSource.volume = masterVolume;
        if (pathogenAudioSource != null) pathogenAudioSource.volume = masterVolume;
        if (bgmAudioSource != null) bgmAudioSource.volume = masterVolume;
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Test Player Damage Sound")]
    public void TestPlayerDamageSound()
    {
        PlayPlayerDamageSound(10);
    }
    
    [ContextMenu("Test Pathogen Damage Sound")]
    public void TestPathogenDamageSound()
    {
        PlayPathogenDamageSound("Test Pathogen", 15);
    }
    

    #endregion

    #region Background Music

    public void PlayDefaultBGM()
    {
        PlayBGM(defaultBGM);
    }

    public void PlayVictoryBGM()
    {
        PlayBGM(victoryBGM);
    }

    public void PlayDefeatBGM()
    {
        PlayBGM(defeatBGM);
    }

    private void PlayBGM(AudioClip bgmClip)
    {
        if (bgmAudioSource == null || bgmClip == null) return;

        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.volume = masterVolume;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    #endregion
}
