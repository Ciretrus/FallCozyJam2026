using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.StartScene {

public class StartWindow : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    private void OnEnable()
    {
        VisualElement root = _uiDocument.rootVisualElement;

        // set clicks
        var playBtn = root.Q<Button>("Play");
        if (playBtn != null)
        {
            playBtn.clicked += OnStartClicked; 
        } else
        {
            Debug.LogError("Button 'Play' not found.");
        }

        // sliders
        var soundSlider = root.Q<SliderInt>("Sound");
        var musicSlider = root.Q<SliderInt>("Music");
        soundSlider?.RegisterValueChangedCallback(OnSoundChanged);
        musicSlider?.RegisterValueChangedCallback(OnMusicChanged);
    }

    private void OnStartClicked()
    {
        //Debug.Log("STARTED");
    }

    private void OnSoundChanged(ChangeEvent<int> evt)
    {
        //Debug.Log($"Sound: {evt.newValue}");
    }

    private void OnMusicChanged(ChangeEvent<int> evt)
    {
        //Debug.Log($"Music: {evt.newValue}");
    }
}

}
