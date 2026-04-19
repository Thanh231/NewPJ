using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonMonoBehaviour<CameraController>
{
    public Camera mainCamera;
    public CameraPanConfig panConfig;
    public CameraZoomConfig zoomConfig;

    public bool Enable { get; set; } = true;

    private List<BaseCameraComponent> lComponents = new List<BaseCameraComponent>();

    protected override void Awake()
    {
        base.Awake();

        if (panConfig.enable)
        {
            lComponents.Add(new CameraComponent_pan(panConfig));
        }

        if (zoomConfig.enable)
        {
            lComponents.Add(new CameraComponent_zoom(zoomConfig));
        }
    }

    private void Update()
    {
        if (Enable)
        {
            foreach (var i in lComponents)
            {
                i.Update();
            }
        }
    }

    public T GetCameraComponent<T>() where T : BaseCameraComponent
    {
        return (T)lComponents.Find(x => x.GetType() == typeof(T));
    }
}