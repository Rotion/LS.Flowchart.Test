using System;

namespace LS.Flowchart.Cameras
{
    public class CameraClassAttribute : Attribute
    {
        public Type CameraType { get; private set; }
        public CameraClassAttribute(Type cameraType)
        {
            if (!typeof(ICamera).IsAssignableFrom(cameraType))
            {
                CameraType = null;
            }
            else
            {
                CameraType = cameraType;
            }
        }
    }
}
