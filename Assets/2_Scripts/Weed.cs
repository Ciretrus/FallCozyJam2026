using DG.Tweening;
using UnityEngine;

public enum WeedType { Common, Large }

public class Weed : MonoBehaviour
{
    [Header("Тип сорняка")]
    [SerializeField] private WeedType weedType = WeedType.Common;

    [Header("Награда")]
    [SerializeField] private int commonReward = 5;
    [SerializeField] private int largeReward = 10;

    [Header("Ссылки")]
    [SerializeField] private Transform visualTransform;
    [SerializeField] private GameObject root;
    [SerializeField] private Rigidbody2D rb;

    [Header("Настройки")]
    [SerializeField] private float pullDistanceThreshold = 2.0f;
    [SerializeField] private float maxVisualLift = 0.6f;
    [SerializeField] private float followSpeed = 0.5f;
    [SerializeField] private float pluckPower = 2f;

    private Collider2D collider;
    private Vector3 initialVisualPos;
    private Vector3 initialVisualScale;
    private Vector3 startMousePos;
    private WeedTaker currentHand;
    private Vector3 handOffset;
    private float personalToughness;
    private bool isGrabbed;
    private bool needFollowHand = false;

    public bool IsPlucked { get; private set; }
    public WeedType Type => weedType;
    public int Reward => weedType == WeedType.Large ? largeReward : commonReward;

    public event System.Action<Weed> OnPlucked;

    private void Awake()
    {
        initialVisualPos = visualTransform.localPosition;
        initialVisualScale = visualTransform.localScale;

        float baseToughness = weedType == WeedType.Large ? 1.3f : 1.0f;
        personalToughness = Random.Range(0.85f, 1.3f) * baseToughness;

        root.SetActive(false);
        if (rb != null) rb.bodyType = RigidbodyType2D.Static;
        collider = GetComponent<Collider2D>();
    }

    public void InitGrab(Vector3 mouseStart, WeedTaker hand)
    {
        if (IsPlucked) return;
        isGrabbed = true;
        startMousePos = mouseStart;
        currentHand = hand;
        visualTransform.DOKill();
        root.SetActive(false);
    }

    public void OnMouseUpdate(Vector3 currentMousePos)
    {
        if (needFollowHand)
        {
            Vector3 targetPos = currentMousePos + handOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
            return;
        }

        if (!isGrabbed || IsPlucked) return;

        float deltaY = Mathf.Max(0, currentMousePos.y - startMousePos.y);
        float progress = Mathf.Clamp01(deltaY / (pullDistanceThreshold * personalToughness));

        float visualY = Mathf.Sin(progress * Mathf.PI * 0.5f) * maxVisualLift;
        float jitter = progress > 0.4f ? Random.Range(-0.02f, 0.02f) * progress : 0f;
        visualTransform.localPosition = initialVisualPos + new Vector3(jitter, visualY, 0);

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
        needFollowHand = true;

        Vector2 randomOffset = Random.insideUnitCircle * currentHand.grabRadius;
        handOffset = new Vector3(randomOffset.x, randomOffset.y, 0f);

        Vector3 popUpOffset = initialVisualPos + new Vector3(Random.Range(-pluckPower, pluckPower), 0.6f, 0f);
        Sequence snapSeq = DOTween.Sequence();
        snapSeq.Append(visualTransform.DOScale(initialVisualScale * 1.15f, 0.1f));
        snapSeq.Append(visualTransform.DOLocalMove(popUpOffset, 0.2f).SetEase(Ease.OutBack));
        snapSeq.Join(visualTransform.DOScale(initialVisualScale, 0.2f));
        snapSeq.Join(visualTransform.DORotate(new Vector3(0, 0, Random.Range(-30f, 30f)), 0.2f));

        OnPlucked?.Invoke(this);
    }

    public void OnRelease()
    {
        isGrabbed = false;
        visualTransform.DOKill();
        needFollowHand = false;

        if (!IsPlucked)
        {
            Sequence bounceBack = DOTween.Sequence();
            bounceBack.Append(visualTransform.DOLocalMove(initialVisualPos, 0.25f).SetEase(Ease.OutBounce));
            bounceBack.Join(visualTransform.DOScale(initialVisualScale, 0.25f).SetEase(Ease.OutBounce));
            bounceBack.OnComplete(() => root.SetActive(false));
        }
        else
        {
            collider.isTrigger = true;
            visualTransform.SetParent(null);

            if (rb != null)
            {
                rb.transform.position = visualTransform.position;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(1f, 3f));
                rb.angularVelocity = Random.Range(-180f, 180f);
            }
            else
            {
                visualTransform.DOMoveY(visualTransform.position.y - 10f, 1.2f).SetEase(Ease.InQuad);
            }
        }
    }
}