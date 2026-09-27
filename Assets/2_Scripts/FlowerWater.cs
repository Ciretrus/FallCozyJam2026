using System;
using UnityEngine;

public class FlowerWater : MonoBehaviour
{
    public float waterRadius = 1.2f;
    [SerializeField] private LayerMask flowerLayer;
    public event Action OnFlowerGrown;


    private readonly Collider2D[] hitColliders = new Collider2D[32];

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            WaterNearbyFlowers();
        }
    }

    public void WaterNearbyFlowers()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, waterRadius, hitColliders, flowerLayer);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayWaterSound(transform.position);

        for (int i = 0; i < count; i++)
        {
            if (hitColliders[i].TryGetComponent<FlowerGrow>(out var flowerGrow))
            {
                flowerGrow.StartGrowing();
            }
        }
    }

    public void FlowerGrown()
    {
        OnFlowerGrown?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, waterRadius);
    }
}
