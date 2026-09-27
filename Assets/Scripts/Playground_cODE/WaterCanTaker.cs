using System.Collections.Generic;
using UnityEngine;

public class WaterCanTaker : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    public float grabRadius = 10.5f;
    [SerializeField] private LayerMask bushLayer;
    private readonly HashSet<Bush> activeBushes = new();
    private void Awake()
    {
        if (camera == null)
            camera = Camera.main;
        Physics2D.queriesHitTriggers = true;
    }
    public void TriggerUpdate()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos;

        if (Input.GetMouseButton(0))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(
                mouseWorldPos,
                grabRadius,
                bushLayer
            );
            HashSet<Bush> detectedBushes = new();
            
            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent<Bush>(out var bush))
                {
                    detectedBushes.Add(bush);
                }
            }

            // Stop bushes that are no longer inside the area
            foreach (Bush bush in activeBushes)
            {
                if (bush == null)
                    continue;

                if (!detectedBushes.Contains(bush))
                {
                    bush.flower.StopGrowing();
                }
            }

            // Replace active bushes with the currently detected ones
            activeBushes.Clear();

            foreach (Bush bush in detectedBushes)
            {
                activeBushes.Add(bush);
                bush.flower.Grow();
            }
        }
        else
        {
            foreach (Bush bush in activeBushes)
            {
                if (bush == null)
                    continue;

                bush.flower.StopGrowing();
            }

            activeBushes.Clear();
        }
    }


    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            worldPoint.z = 0f;
            return worldPoint;
        }

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grabRadius);
    }

    bool IsPointInsideEllipse(
        Vector2 point,
        Vector2 center,
        float radiusX
    )
    {
        float dx = point.x - center.x;
        float dy = point.y - center.y;

        // TODO NNPRMAL ELLIPSOID
        float radiusY = radiusX * 0.3f;

        return (dx * dx) / (radiusX * radiusX)
             + (dy * dy) / (radiusY * radiusY) <= 1f;
    }
}
