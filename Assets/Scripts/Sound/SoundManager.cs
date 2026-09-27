using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Events")]
    [SerializeField] private EventReference ambienceEvent;
    [SerializeField] private EventReference plantEvent;
    [SerializeField] private EventReference pullingEvent;
    [SerializeField] private EventReference waterEvent;
    [SerializeField] private EventReference uiLooseEvent;
    [SerializeField] private EventReference uiButtonEvent;
    [SerializeField] private EventReference victoryEvent;

    [Header("Music")]
    [SerializeField] private EventReference musicEvent;

    [Header("Music State Mapping")]
    [Tooltip("Имя сцены -> значение параметра GameState в FMOD")]
    [SerializeField] private string menuSceneName = "StartScreen";
    [SerializeField] private string gameplaySceneName = "FlowersScene";
    [SerializeField] private int menuGameState = 0;
    [SerializeField] private int gameplayGameState = 1;
    [SerializeField] private string gameStateParameterName = "GameState";

    private FMOD.Studio.EventInstance ambienceInstance;
    private FMOD.Studio.EventInstance musicInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        StartAmbience();
        StartMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMusicStateForScene(scene.name);
    }

    private void ApplyMusicStateForScene(string sceneName)
    {
        if (sceneName == menuSceneName)
        {
            SetMusicState(menuGameState);
        }
        else if (sceneName == gameplaySceneName)
        {
            SetMusicState(gameplayGameState);
        }
        // Если сцена не описана — оставляем текущее состояние музыки.
    }

    private void StartAmbience()
    {
        if (ambienceEvent.IsNull) return;

        if (ambienceInstance.isValid())
        {
            ambienceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ambienceInstance.release();
        }

        ambienceInstance = RuntimeManager.CreateInstance(ambienceEvent);
        ambienceInstance.start();
    }

    private void StartMusic()
    {
        if (musicEvent.IsNull) return;

        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }

        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();

        // Применяем состояние для текущей сцены сразу после старта музыки.
        // Start() вызывается после того, как сцена загружена, но sceneLoaded
        // для первой сцены может прийти до того, как SoundManager подписался.
        ApplyMusicStateForScene(SceneManager.GetActiveScene().name);
    }

    public void SetMusicState(int state)
    {
        if (musicInstance.isValid())
            musicInstance.setParameterByName(gameStateParameterName, state);
    }

    private void OnDestroy()
    {
        if (ambienceInstance.isValid())
        {
            ambienceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ambienceInstance.release();
        }

        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }
    }

    public void PlayPlantSound(Vector3 position)
    {
        if (!plantEvent.IsNull)
            RuntimeManager.PlayOneShot(plantEvent, position);
    }

    public void PlayPullingSound(Vector3 position)
    {
        if (!pullingEvent.IsNull)
            RuntimeManager.PlayOneShot(pullingEvent, position);
    }

    public void PlayWaterSound(Vector3 position)
    {
        if (!waterEvent.IsNull)
            RuntimeManager.PlayOneShot(waterEvent, position);
    }

    public void PlayUIButton()
    {
        if (!uiButtonEvent.IsNull)
            RuntimeManager.PlayOneShot(uiButtonEvent);
    }

    public void PlayUILoose()
    {
        if (!uiLooseEvent.IsNull)
            RuntimeManager.PlayOneShot(uiLooseEvent);
    }

    public void PlayVictory()
    {
        if (!victoryEvent.IsNull)
            RuntimeManager.PlayOneShot(victoryEvent);
    }
}