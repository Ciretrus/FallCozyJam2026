using System;
using UnityEngine;

[Serializable]
public class CameraDrag : ICamera
{
    [SerializeField]
    float cameraSpeed = 1;

    private Vector3 _dragOrigin;
    private Camera _camera;
    private Transform _transform;

    public void Init(Camera cam, Transform cameraTransform)
    {
        _camera = cam;
        _transform = cameraTransform;
    }

    public void Update()
    {
        DragCamera();
    }

    private void DragCamera()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _dragOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 currentPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 diff = _dragOrigin - currentPosition;

            _transform.position += new Vector3(diff.x, diff.y, 0) * cameraSpeed;
        }
    }
    public void LateUpdate() { }
}
