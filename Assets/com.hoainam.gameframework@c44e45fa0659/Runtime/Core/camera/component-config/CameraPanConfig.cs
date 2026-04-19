using System;

[Serializable]
public class CameraPanConfig : BaseCameraConfig
{
    public MouseButtonType panWithMouse;
    public bool usingInertia;
    public float inertiaDamping = 5;
}