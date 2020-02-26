using Ara3D;
using KP3D.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace KP3D.Scene
{
    class Scene
    {
        public Camera camera;
        public List<Shape> shapes;
        public Vector3 light_dir = new Vector3(1, 1, 1).Normalize();

        public class Shape
        {

            public string type;

            public float dx = 0;
            public float dy = 0;
            public float dz = 0;

            public float rotation_x = 0;
            public float rotation_y = 0;
            public float rotation_z = 0;

            public float scale_x = 1;
            public float scale_y = 1;
            public float scale_z = 1;


            public List<MyTriangle> triangles;
        }

        public class Camera
        {
            public Vector3 eye;
            public Vector3 center;
            public float fov;

            public Camera(Vector3 eye, Vector3 center, float fov)
            {
                this.eye = eye;
                this.center = center;
                this.fov = fov;
            }

            public float longitude = 0;
            public float latitude = 0;
            public void OnMouseMove(int deltaX, int deltaY)
            {
                //изменяем широту и долготу
                longitude = longitude + deltaX * 0.05f; //долгота, фи
                latitude = latitude + deltaY * 0.05f; //широта, тэта
                                                    //ограничиваем широту(тэта)
                //if (latitude > (float)Math.PI / 2) latitude = (float)Math.PI / 2;
                //if (latitude < -(float)Math.PI / 2) latitude = -(float)Math.PI / 2;
                
            }
            public void gen_eye(int r)
            {
                float x = r * (float)(Math.Cos(latitude) * Math.Cos(longitude));
                float y = r * (float)(Math.Sin(latitude) * Math.Sin(longitude));
                float z = r * (float)Math.Cos(latitude);
                eye = new Vector3(y, x, 100);
            }
            public void gen_center(int r)
            {
                float x = r * (float)(Math.Cos(latitude) * Math.Cos(longitude));
                float y = r * (float)(Math.Sin(latitude) * Math.Sin(longitude));
                float z = r * (float)Math.Cos(latitude);
                center = new Vector3(y, x, 100);
            }
        }

        public Scene(Scene.Camera camera)
        {
            this.camera = camera;
            this.shapes = new List<Shape>();
        }

        public System.Drawing.Bitmap render(int width, int height)
        {
            int size_x = width;
            int size_y = height;
            Bitmap bm = new Bitmap(size_x, size_y);

            BitmapData bitmapData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, bm.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(bm.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * bm.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;

            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * bitmapData.Stride;
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    // calculate new pixel value
                    pixels[currentLine + x] = (byte)30;
                    pixels[currentLine + x + 1] = (byte)30;
                    pixels[currentLine + x + 2] = (byte)30;
                }
            }

            for (int i = 0; i < shapes.Count; i++)
            {

                var RotX = MyMath.MyMatrix4x4.CreateRotationX(shapes[i].rotation_x);
                var RotY = MyMath.MyMatrix4x4.CreateRotationY(shapes[i].rotation_y);
                var RotZ = MyMath.MyMatrix4x4.CreateRotationZ(shapes[i].rotation_z);

                var Rot = RotX * RotY * RotZ;

                var Scale = MyMath.MyMatrix4x4.CreateScale(shapes[i].scale_x, shapes[i].scale_y, shapes[i].scale_z);
                var TL = MyMath.MyMatrix4x4.CreateTranslation(shapes[i].dx, shapes[i].dy, shapes[i].dz);
                var W = TL * Rot * Scale;
                var V = MyMath.MyMatrix4x4.CreateLookAt(camera.eye, camera.center, new Vector3(0, 1, 0));
                var P = MyMath.MyMatrix4x4.CreateProjectionFOV(camera.fov, 0.01f, 100);
                var VP = MyMath.MyMatrix4x4.CreateViewPort(width / 8, width / 8, width, height, 1);
                var WVP = P * V * W;
                Random rnd = new Random(228);

                float[] zbuffer = new float[width * height];
                for (int j = 0; j < width * height; j++)
                    zbuffer[j] = -0xFFFFFFFF;

                for (int j = 0; j < shapes[i].triangles.Count; j++)
                {
                    var p0 = MyMath.MyMatrix4x1.FromPoint4x1(shapes[i].triangles[j].a);
                    var p1 = MyMath.MyMatrix4x1.FromPoint4x1(shapes[i].triangles[j].b);
                    var p2 = MyMath.MyMatrix4x1.FromPoint4x1(shapes[i].triangles[j].c);

                    var t1 = MyMath.MyMatrix4x1.ToPoint(WVP * p0);
                    var t2 = MyMath.MyMatrix4x1.ToPoint(WVP * p1);
                    var t3 = MyMath.MyMatrix4x1.ToPoint(WVP * p2);

                    Vector3 intensivity = shapes[i].triangles[j].normal * light_dir;
                    intensivity = new Vector3(1, 1, 1);
                    Render.p_triangle_old(t1, t2, t3, intensivity.X, intensivity.Y, intensivity.Z, pixels, bitmapData, bytesPerPixel, bm.Height, bm.Width, Render.color(rnd.Next(255, 255), 0, 0), zbuffer);
                    //Render.triangle(new Vector3[] { t1, t2, t3 }, zbuffer, bm, Render.color(rnd.Next(0, 128), 0, 0));
                    //Render.line((int)t1.X, (int)t1.Y, (int)t2.X, (int)t2.Y, bm, Render.color(255, 0, 0));
                    //Render.line((int)t2.X, (int)t2.Y, (int)t3.X, (int)t3.Y, bm, Render.color(255, 0, 0));
                    //Render.line((int)t1.X, (int)t1.Y, (int)t3.X, (int)t3.Y, bm, Render.color(255, 0, 0));
                }
            }
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            bm.UnlockBits(bitmapData);
            return bm;

        }

    }
}
