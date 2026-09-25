using UnityEngine;

interface ICamera
{
    void Init(Camera cam, Transform cameraTransform);
    void Update();
    void LateUpdate();
}