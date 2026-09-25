using DG.Tweening;
using UnityEngine;

public class Weed : MonoBehaviour
{
    [SerializeField] private Transform visualTransform;
    [SerializeField] private GameObject root;
    [SerializeField] private Rigidbody2D rb; 

    [Header("Настройки")]
    [SerializeField] private float pullDistanceThreshold = 2.0f; 
    [SerializeField] private float maxVisualLift = 0.6f;
    [SerializeField] private float followSpeed = 0.5f;
    [SerializeField] private float pluckPower = 2f;

    private Vector3 initialVisualPos;
    private Vector3 initialVisualScale;
    private Vector3 startMousePos;
    private WeedTaker currentHand;
    private Vector3 handOffset;
    private float personalToughness;
    private bool isGrabbed;
    private bool needFollowHand = false;
    public bool IsPlucked { get; private set; }

    private void Awake()
    {
        initialVisualPos = visualTransform.localPosition;
        initialVisualScale = visualTransform.localScale;
        personalToughness = Random.Range(0.85f, 1.3f);

        root.SetActive(false);
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
        root.SetActive(false);
    }

    // 2. Куст сам считает прогресс по координатам руки
    public void OnMouseUpdate(Vector3 currentMousePos)
    {
        if (needFollowHand)
        {
            Vector2 randomOffset = Random.insideUnitCircle * (currentHand.grabRadius * 2f);
            Vector3 targetPos = currentMousePos + handOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
            return;
        }

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

    private void RipAndAttachToHand()
    {
        IsPlucked = true;
        visualTransform.DOKill();
        root.SetActive(true);
        //visualTransform.SetParent(currentHand.transform);
        needFollowHand = true;
        Vector2 randomOffset = Random.insideUnitCircle * (currentHand.grabRadius * 2f);
        handOffset = new Vector3(randomOffset.x, randomOffset.y, 0f);
        Vector3 popUpOffset = initialVisualPos + new Vector3(Random.Range(-pluckPower, pluckPower), 0.6f, 0f);
        Sequence snapSeq = DOTween.Sequence();
        snapSeq.Append(visualTransform.DOScale(initialVisualScale * 1.15f, 0.1f));
        snapSeq.Append(visualTransform.DOLocalMove(popUpOffset, 0.2f).SetEase(Ease.OutBack));
        snapSeq.Join(visualTransform.DOScale(initialVisualScale, 0.2f));
        snapSeq.Join(visualTransform.DORotate(new Vector3(0, 0, Random.Range(-30f, 30f)), 0.2f));
    }

    // 4. Отпустили хват
    public void OnRelease()
    {
        isGrabbed = false;
        visualTransform.DOKill();
        needFollowHand = false;
        if (!IsPlucked)
        {
            // Не сорвали — пружинит обратно в землю
            Sequence bounceBack = DOTween.Sequence();
            bounceBack.Append(visualTransform.DOLocalMove(initialVisualPos, 0.25f).SetEase(Ease.OutBounce));
            bounceBack.Join(visualTransform.DOScale(initialVisualScale, 0.25f).SetEase(Ease.OutBounce));
            bounceBack.OnComplete(() =>
            {
                root.SetActive(false);
            });
        }
        else
        {
            // Сорванный пучок — отцепляем от руки, включаем 2D-физику, он летит вниз
            visualTransform.SetParent(null);

            if (rb != null)
            {
                rb.transform.position = visualTransform.position;
                rb.bodyType = RigidbodyType2D.Dynamic;
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
