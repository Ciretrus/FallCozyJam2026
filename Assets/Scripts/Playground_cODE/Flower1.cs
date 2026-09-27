using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Flower1 : MonoBehaviour
{
    [Header("������")]
    [SerializeField] private Transform visualTransform;

    [Header("����� �������� (�������)")]
    [SerializeField] private float flowerHeight = 1.2f;    // ������ ������ �� �����

    [Header("��������� ������������")]
    [SerializeField] private float maxTiltAngle = 25f;      // ������������ ���� ������� ������
    [SerializeField] private float smoothSpeed = 7f;        // �������� �����������
    [SerializeField] private float deadZoneWidth = 0.25f;   // ������ ���� �������� ����� �����

    [Header("������� � �����")]
    [SerializeField] private float returnDuration = 0.55f;
    [SerializeField] private float returnOvershoot = 1.15f;

    private Quaternion initialLocalRot;
    private Tween returnTween;
    private bool isBent;

    // ������� ����� �������� ������
    public Vector3 HeadPosition => transform.position + transform.up * flowerHeight;

    private void Awake()
    {
        if (visualTransform == null) visualTransform = transform;
        initialLocalRot = visualTransform.localRotation;
        initialScale = visualTransform.localScale;
    }

    public void Push(Vector3 mouseWorldPos, float influenceRadius)
    {
        // ������� ������ �� ���� ������ � �������� ������, � �� � ��� �����
        Vector2 dirFromMouse = HeadPosition - mouseWorldPos;
        float distance = dirFromMouse.magnitude;

        if (distance >= influenceRadius) return;

        isBent = true;
        returnTween?.Kill();

        // ���� ������ ������������ ������
        float normalizedDist = distance / influenceRadius;
        float force = Mathf.SmoothStep(1f, 0f, normalizedDist);

        // ������ ������ ������� ����������
        float side = Mathf.Clamp(dirFromMouse.x / deadZoneWidth, -1f, 1f);

        // ������� ������ ��������� (Pivot), �������� ������� �� ����
        float targetAngle = -side * maxTiltAngle * force;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        visualTransform.localRotation = Quaternion.Slerp(
            visualTransform.localRotation,
            targetRotation,
            smoothSpeed * Time.deltaTime
        );
    }

    public void ReleaseBend()
    {
        if (!isBent) return;
        isBent = false;

        returnTween?.Kill();

        returnTween = visualTransform
            .DOLocalRotateQuaternion(initialLocalRot, returnDuration)
            .SetEase(Ease.OutBack, returnOvershoot);
    }

    [Header("Growth")]
    [SerializeField] private float growDuration = 0.8f;
    private Vector3 initialScale;
    private Tween growTween;

    public void Grow()
    {
        gameObject.SetActive(true);
        if (growTween != null && growTween.IsComplete())
            return;

        if (growTween == null)
        {
            visualTransform.localScale = new Vector3(
                initialScale.x,
                0f,
                initialScale.z
            );

            growTween = visualTransform
                .DOScale(initialScale, growDuration)
                .SetEase(Ease.OutCubic)
                .SetAutoKill(false);
        }
        growTween.Play();
    }

    public void StopGrowing()
    {
        if (growTween != null && growTween.IsActive())
        {
            growTween.Pause();
        }
    }

    // ����������� ����� �������� � ���� Scene ��� ������� ��������� ������
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(HeadPosition, 0.15f);
        Gizmos.DrawLine(transform.position, HeadPosition);
    }
}
