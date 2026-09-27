using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform visualTransform;

    [Header("Точка контакта (Макушка)")]
    [SerializeField] private float flowerHeight = 1.2f;    // Высота бутона от корня

    [Header("Настройки поглаживания")]
    [SerializeField] private float maxTiltAngle = 25f;      // Максимальный угол наклона стебля
    [SerializeField] private float smoothSpeed = 7f;        // Скорость сглаживания
    [SerializeField] private float deadZoneWidth = 0.25f;   // Мягкая зона перехода через центр

    [Header("Возврат в покой")]
    [SerializeField] private float returnDuration = 0.55f;
    [SerializeField] private float returnOvershoot = 1.15f;

    private Quaternion initialLocalRot;
    private Tween returnTween;
    private bool isBent;

    // Мировая точка верхушки цветка
    public Vector3 HeadPosition => transform.position + transform.up * flowerHeight;

    private void Awake()
    {
        if (visualTransform == null) visualTransform = transform;
        initialLocalRot = visualTransform.localRotation;
    }

    public void Push(Vector3 mouseWorldPos, float influenceRadius)
    {
        // Считаем вектор от мыши строго к верхушке цветка, а не к его корню
        Vector2 dirFromMouse = HeadPosition - mouseWorldPos;
        float distance = dirFromMouse.magnitude;

        if (distance >= influenceRadius) return;

        isBent = true;
        returnTween?.Kill();

        // Сила нажима относительно бутона
        float normalizedDist = distance / influenceRadius;
        float force = Mathf.SmoothStep(1f, 0f, normalizedDist);

        // Мягкий расчет стороны отклонения
        float side = Mathf.Clamp(dirFromMouse.x / deadZoneWidth, -1f, 1f);

        // Вращаем вокруг основания (Pivot), наклоняя макушку от руки
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

    // Отображение точки верхушки в окне Scene для удобной настройки высоты
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(HeadPosition, 0.15f);
        Gizmos.DrawLine(transform.position, HeadPosition);
    }
}
