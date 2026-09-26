using System.Collections.Generic;
using UnityEngine;

public class FlowerTouch : MonoBehaviour
{
    [Header("Настройки зоны касания")]
    [SerializeField] private float touchRadius = 1.6f;
    [SerializeField] private LayerMask flowerLayer;

    [Header("Камера")]
    [SerializeField] private Camera targetCamera;

    // Статический буфер для коллайдеров без выделения мусора в памяти
    private readonly Collider2D[] hitBuffer = new Collider2D[32];

    // Хранилища для отслеживания входа/выхода из зоны
    private readonly HashSet<Flower> currentFrameFlowers = new HashSet<Flower>();
    private readonly HashSet<Flower> previousFrameFlowers = new HashSet<Flower>();

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        transform.position = mouseWorldPos;

        ProcessFlowerInteraction(mouseWorldPos);
    }

    private void ProcessFlowerInteraction(Vector3 mouseWorldPos)
    {
        // 1. Переносим цветы прошлого кадра в буфер проверки на выход
        previousFrameFlowers.Clear();
        foreach (var f in currentFrameFlowers)
        {
            previousFrameFlowers.Add(f);
        }
        currentFrameFlowers.Clear();

        // 2. Ищем все цветы в радиусе касания
        int count = Physics2D.OverlapCircleNonAlloc(mouseWorldPos, touchRadius, hitBuffer, flowerLayer);

        for (int i = 0; i < count; i++)
        {
            if (hitBuffer[i].TryGetComponent<Flower>(out var flower))
            {
                // Наклоняем цветок в реальном времени
                flower.Push(mouseWorldPos, touchRadius);
                currentFrameFlowers.Add(flower);

                // Цветок все еще под курсором — удаляем из списка на сброс
                previousFrameFlowers.Remove(flower);
            }
        }

        // 3. Все цветы, которые были под курсором в прошлом кадре, но вышли из радиуса сейчас — пружинят обратно
        foreach (var leftFlower in previousFrameFlowers)
        {
            if (leftFlower != null)
            {
                leftFlower.ReleaseBend();
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(targetCamera.transform.position.z);
        Vector3 worldPoint = targetCamera.ScreenToWorldPoint(mouseScreen);
        worldPoint.z = 0f;
        return worldPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, touchRadius);
    }
}
