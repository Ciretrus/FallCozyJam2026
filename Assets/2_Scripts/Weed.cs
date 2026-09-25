using DG.Tweening;
using UnityEngine;

public class Weed : MonoBehaviour
{
    [SerializeField] private Transform visualTransform;
    [SerializeField] private SpriteMask pullMask;
    [SerializeField] private Rigidbody2D rb; 

    [Header("Настройки")]
    [SerializeField] private float pullDistanceThreshold = 2.0f; 
    [SerializeField] private float maxVisualLift = 0.6f;

    private Vector3 initialVisualPos;
    private Vector3 initialVisualScale;
    private Vector3 startMousePos;
    private WeedTaker currentHand;

    private float personalToughness;
    private bool isGrabbed;
    public bool IsPlucked { get; private set; }

    private void Awake()
    {
        initialVisualPos = visualTransform.localPosition;
        initialVisualScale = visualTransform.localScale;
        personalToughness = Random.Range(0.85f, 1.3f);

        if (pullMask != null) pullMask.enabled = false;
        if (rb != null) rb.bodyType = RigidbodyType2D.Static;
    }

    // 1. Рука передает старт и себя
    public void InitGrab(Vector3 mouseStart, WeedTaker hand)
    {
        if (IsPlucked) return;

        isGrabbed = true;
        startMousePos = mouseStart;
        currentHand = hand;

        visualTransform.DOKill();
        if (pullMask != null) pullMask.enabled = true;
    }

    // 2. Куст сам считает прогресс по координатам руки
    public void OnMouseUpdate(Vector3 currentMousePos)
    {
        if (!isGrabbed || IsPlucked) return;

        float deltaY = Mathf.Max(0, currentMousePos.y - startMousePos.y);
        float progress = Mathf.Clamp01(deltaY / (pullDistanceThreshold * personalToughness));

        // Вытягивание корня из-под маски
        float visualY = Mathf.Sin(progress * Mathf.PI * 0.5f) * maxVisualLift;
        float jitter = progress > 0.4f ? Random.Range(-0.02f, 0.02f) * progress : 0f;
        visualTransform.localPosition = initialVisualPos + new Vector3(jitter, visualY, 0);

        // Squash & Stretch
        visualTransform.localScale = new Vector3(
            initialVisualScale.x * (1f - progress * 0.2f),
            initialVisualScale.y * (1f + progress * 0.35f),
            initialVisualScale.z
        );

        if (progress >= 1f)
        {
            RipAndAttachToHand();
        }
    }

    // 3. Срыв: отключаем маску, отстреливаем и примагничиваемся к руке
    private void RipAndAttachToHand()
    {
        IsPlucked = true;
        visualTransform.DOKill();
        if (pullMask != null) pullMask.enabled = false;

        // Отцепляем визуал от корней и цепляем к руке со случайным сдвигом по радиусу
        visualTransform.SetParent(currentHand.transform);

        Vector2 randomOffset = Random.insideUnitCircle * (currentHand.grabRadius * 0.7f);
        Vector3 targetLocalPos = new Vector3(randomOffset.x, randomOffset.y, 0f);

        Sequence snapSeq = DOTween.Sequence();
        // Взрывной отскок и притягивание к руке
        snapSeq.Append(visualTransform.DOScale(initialVisualScale * 1.15f, 0.1f));
        snapSeq.Append(visualTransform.DOLocalMove(targetLocalPos, 0.2f).SetEase(Ease.OutBack));
        snapSeq.Join(visualTransform.DOScale(initialVisualScale, 0.2f));
        snapSeq.Join(visualTransform.DORotate(new Vector3(0, 0, Random.Range(-30f, 30f)), 0.2f));
    }

    // 4. Отпустили хват
    public void OnRelease()
    {
        isGrabbed = false;
        visualTransform.DOKill();

        if (!IsPlucked)
        {
            // Не сорвали — пружинит обратно в землю
            Sequence bounceBack = DOTween.Sequence();
            bounceBack.Append(visualTransform.DOLocalMove(initialVisualPos, 0.25f).SetEase(Ease.OutBounce));
            bounceBack.Join(visualTransform.DOScale(initialVisualScale, 0.25f).SetEase(Ease.OutBounce));
            bounceBack.OnComplete(() =>
            {
                if (pullMask != null) pullMask.enabled = false;
            });
        }
        else
        {
            // Сорванный пучок — отцепляем от руки, включаем 2D-физику, он летит вниз
            visualTransform.SetParent(null);

            if (rb != null)
            {
                rb.transform.position = visualTransform.position;
                rb.simulated = true;
                rb.linearVelocity = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(1f, 3f)); // Легкий подброс
                rb.angularVelocity = Random.Range(-180f, 180f);
            }
            else
            {
                // Если без Rigidbody — просто роняем твином вниз
                visualTransform.DOMoveY(visualTransform.position.y - 10f, 1.2f).SetEase(Ease.InQuad);
            }
        }
    }
}
