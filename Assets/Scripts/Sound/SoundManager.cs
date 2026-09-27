using FMODUnity;
using UnityEngine;

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

    private FMOD.Studio.EventInstance ambienceInstance;

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

    private void Start()
    {
        StartAmbience();
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

    private void OnDestroy()
    {
        if (ambienceInstance.isValid())
        {
            ambienceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ambienceInstance.release();
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