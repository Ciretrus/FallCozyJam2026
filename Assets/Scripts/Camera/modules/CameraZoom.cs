using System;
using UnityEngine;

[Serializable]
public class CameraZoom : ICamera
{
    [SerializeField]
    private float zoomSpeed = 1;
    [SerializeField]
    private float minSize = 3;
    [SerializeField]
    private float maxSize = 10;

    private Camera _camera;

    public void Init(Camera cam, Transform cameraTransform)
    {
        _camera = cam;
    }

    public void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0) return;

        float newSize = _camera.orthographicSize - scroll * zoomSpeed;
        _camera.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);
    }
    public void LateUpdate() { }
}
