using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float duration = 0.5f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 1);

        fadeImage
            .DOFade(0f, duration)
            .SetEase(Ease.OutQuad);
    }

    public void LoadScene(string sceneName)
    {
        fadeImage
            .DOFade(1f, duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                SceneManager.LoadScene(sceneName);
            });
    }
}
}