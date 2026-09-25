using System;
using UnityEngine;

[Serializable]
public class CameraBounds : ICamera
{
    [SerializeField]
    private BoxCollider2D boundsColl;

    private Camera _camera;
    private Transform _transform;

    public void Init(Camera cam, Transform cameraTransform)
    {
        _camera = cam;
        _transform = cameraTransform;
    }

    public void LateUpdate()
    {
        CheckBounds();
    }

    private void CheckBounds()
    {
        if (boundsColl == null) return;

        Bounds bounds = boundsColl.bounds;

        float halfHeight = _camera.orthographicSize;
        float halfWidth = halfHeight * _camera.aspect;

        float clampedX = Mathf.Clamp(_transform.position.x,
            bounds.min.x + halfWidth, bounds.max.x - halfWidth);
        float clampedY = Mathf.Clamp(_transform.position.y,
            bounds.min.y + halfHeight, bounds.max.y - halfHeight);

        _transform.position = new Vector3(clampedX, clampedY, _transform.position.z);
    }

    public void Update() { }
}
