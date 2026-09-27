using System.Collections.Generic;
using UnityEngine;

public class WeedTaker : MonoBehaviour
{
    [SerializeField] private Camera camera;
    public float grabRadius = 1.5f;
    [SerializeField] private LayerMask weedLayer;


    private Collider2D[] hitColliders = new Collider2D[64];
    private readonly List<Weed> activeWeeds = new List<Weed>();
    private Vector3 startGrabPos;
    private bool isPulling;


    private void Awake()
    {
        if (camera == null)
            camera = Camera.main;
        Physics2D.queriesHitTriggers = true;
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos;
        
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"[WeedTaker] ���� � �����: {mouseWorldPos}");
            startGrabPos = mouseWorldPos;
            isPulling = true;
            activeWeeds.Clear();

            int count = Physics2D.OverlapCircleNonAlloc(mouseWorldPos, grabRadius, hitColliders, weedLayer);
            Debug.Log($"[WeedTaker] ������� �������� � �������: {count}");
            for (int i = 0; i < count; i++)
            {
                if (hitColliders[i].TryGetComponent<Weed>(out var weed) && !weed.IsPlucked)
                {
                    weed.InitGrab(startGrabPos, this);
                    activeWeeds.Add(weed);
                }
            }
        }

        // 2. ����� � ����� ���������� ���������� ���� � ����������� �����
        if (isPulling && Input.GetMouseButton(0))
        {
            for (int i = 0; i < activeWeeds.Count; i++)
            {
                if (activeWeeds[i] != null)
                {
                    activeWeeds[i].OnMouseUpdate(mouseWorldPos);
                }
            }
        }

        // 3. ��������� � ������� ���� �������� (� ���������, � ��� ��������� � ����) �����������
        if (isPulling && Input.GetMouseButtonUp(0))
        {
            isPulling = false;
            for (int i = 0; i < activeWeeds.Count; i++)
            {
                if (activeWeeds[i] != null)
                {
                    activeWeeds[i].OnRelease();
                }
            }
            activeWeeds.Clear();
        }
    }


    private Vector3 GetMouseWorldPosition()
    {
        // ������� ��� �� ������ ����� �������� ����� ������� ����� � ��������� Z = 0
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.forward, Vector3.zero); // ��������� XY �� Z = 0

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            worldPoint.z = 0f; // �������� ���� Z
            return worldPoint;
        }

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        // ������ ����� ���, ��� ������ ������� ���� �������
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grabRadius);
    }

}
