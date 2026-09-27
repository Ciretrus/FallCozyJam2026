using DG.Tweening;
using UnityEngine;

public class FlowerGrow : MonoBehaviour
{

    [SerializeField] private Transform visualTransform;
    [SerializeField] private float growDuration = 1.5f;

    [SerializeField] private float popOvershoot = 1.25f; 
    [SerializeField] private float popDuration = 0.35f;  

    private Vector3 initialScale;
    private float currentProgress = 0f;
    private bool isGrowing = false;
    private bool isFullyGrown = false;

    public bool IsFullyGrown => isFullyGrown;

    private void Awake()
    {
        if (visualTransform == null)
            visualTransform = transform;

        initialScale = visualTransform.localScale;
    }

    private void Start()
    {
        visualTransform.localScale = Vector3.zero;
    }

    public void StartGrowing()
    {
        if (isFullyGrown) return;
        if (currentProgress*growDuration >= growDuration*0.95)
        {
            Debug.Log(currentProgress);
            isFullyGrown = true;
            PlayBloomPopEffect();
            return;
        }
        
            currentProgress += Time.deltaTime;
            currentProgress = Mathf.Clamp01(currentProgress);
            //Debug.Log(currentProgress);
            visualTransform.localScale = Vector3.Lerp(Vector3.zero, initialScale, currentProgress);

    }


    private void PlayBloomPopEffect()
    {
        visualTransform.DOKill();
        visualTransform.localScale = initialScale;
        visualTransform
            .DOScale(initialScale * popOvershoot, popDuration * 0.4f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                visualTransform
                    .DOScale(initialScale, popDuration * 0.6f)
                    .SetEase(Ease.OutBack, 2f);
            });
    }

    private void OnDestroy()
    {
        visualTransform.DOKill();
    }

}
