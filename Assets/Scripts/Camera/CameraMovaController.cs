using System.Collections.Generic;
using UnityEngine;

public class CameraMovaController : MonoBehaviour
{
    [SerializeField]
    private CameraDrag CameraDrag;
    [SerializeField]
    private CameraZoom CameraZoom;
    [SerializeField]
    private CameraBounds CameraBounds;

    List<ICamera> modules = new List<ICamera>();

    private void Start()
    {
        modules.Add(CameraDrag);
        modules.Add(CameraZoom);
        modules.Add(CameraBounds);

        foreach (var module in modules)
            module.Init(Camera.main, transform);
    }

    private void Update()
    {
        foreach (var module in modules)
            module.Update();
    }

    private void LateUpdate()
    {
        foreach (var module in modules)
            module.LateUpdate();
    }
}
