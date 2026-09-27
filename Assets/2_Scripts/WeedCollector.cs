using System;
using UnityEngine;

public class WeedCollector : MonoBehaviour
{
    [SerializeField] private LayerMask weedLayer;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private bool matchCameraX = false;

    public event Action<Weed> OnWeedCollected;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Update()
    {
        if (targetCamera == null) return;

        float bottomY = targetCamera.transform.position.y - targetCamera.orthographicSize + yOffset;

        float targetX = transform.position.x;
        if (matchCameraX)
            targetX = targetCamera.transform.position.x;

        transform.position = new Vector3(targetX, bottomY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & weedLayer) == 0) return;

        Weed weed = collision.GetComponent<Weed>();
        if (weed == null) return;

        if (!weed.IsPlucked) return;

        OnWeedCollected?.Invoke(weed);
        Destroy(weed.gameObject);
    }
}