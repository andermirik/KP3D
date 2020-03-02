using Ara3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KP3D.Scene
{
    class Camera
    {
        public Vector3 eye;
        public float fov;

        public Camera(Vector3 eye, float fov)
        {
            this.eye = eye;
            this.fov = fov;
        }

        public float pitch = 0.01f;
        public float yaw = 0.01f;
        public void OnMouseMove(int deltaX, int deltaY)
        {
            pitch = pitch + deltaX * 0.02f;
            yaw = yaw + deltaY * 0.02f;

            if (Math.Abs(pitch - 0) < 1e-4)
                pitch = 0.001f * Math.Sign(deltaX);
            if (Math.Abs(yaw - 0) < 1e-4)
                yaw = 0.001f * Math.Sign(deltaY);
        }
    }
}
