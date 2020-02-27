using Ara3D;
using KP3D.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KP3D.Scene
{
    class Scene
    {
        public Camera camera;
        public List<Shape> shapes;
        public Vector3 light_dir = new Vector3(0, 0, 1).Normalize();

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

            public float pitch = 0;
            public float yaw = 0;
            public void OnMouseMove(int deltaX, int deltaY)
            {
                //изменяем широту и долготу
                pitch = pitch + deltaX * 0.05f; //долгота, фи
                yaw = yaw + deltaY * 0.05f; //широта, тэта

                //if (pitch > (float)Math.PI / 2) pitch = (float)Math.PI / 2;
                //if (yaw < -(float)Math.PI / 2) yaw = -(float)Math.PI / 2;

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
                    pixels[currentLine + x] = (byte)225;
                    pixels[currentLine + x + 1] = (byte)225;
                    pixels[currentLine + x + 2] = (byte)225;
                }
            }

            float[] zbuffer = new float[width * height];
            for (int j = 0; j < width * height; j++)
                zbuffer[j] = -0xFFFFFFFF;

            bool first_time = false;
            again:
            for (int i = 0; i < shapes.Count; i++)
            {

                var RotX = MyMath.MyMatrix4x4.CreateRotationX(shapes[i].rotation_x);
                var RotY = MyMath.MyMatrix4x4.CreateRotationY(shapes[i].rotation_y);
                var RotZ = MyMath.MyMatrix4x4.CreateRotationZ(shapes[i].rotation_z);

                var Rot = RotX * RotY * RotZ;

                var Scale = MyMath.MyMatrix4x4.CreateScale(shapes[i].scale_x, shapes[i].scale_y, shapes[i].scale_z);
                var TL = MyMath.MyMatrix4x4.CreateTranslation(shapes[i].dx, shapes[i].dy, shapes[i].dz);
                var W = TL * Rot * Scale;

                //var V = MyMath.MyMatrix4x4.CreateJRALookAt(camera.eye, camera.center, new Vector3(0, 1, 0));
                //var V = MyMath.MyMatrix4x4.CreateLookAt(camera.eye, camera.center, new Vector3(0, 1, 0));

                var V = MyMath.MyMatrix4x4.CreateFPSLookAt(camera.eye, camera.pitch, camera.yaw);
                var P = MyMath.MyMatrix4x4.CreateProjectionFOV(camera.fov, width / height, 0.01f, 1000);
                var WVP = P * V * W;
                Random rnd = new Random(228);


                Object _lock = new Object();
                Parallel.For(0, shapes[i].triangles.Count, new ParallelOptions { MaxDegreeOfParallelism = 16 }, j =>
                //for (int j = 0; j < shapes[i].triangles.Count; j++)
                {
                    var p0 = MyMath.MyMatrix4x1.FromPoint4x1(shapes[i].triangles[j].a);
                    var p1 = MyMath.MyMatrix4x1.FromPoint4x1(shapes[i].triangles[j].b);
                    var p2 = MyMath.MyMatrix4x1.FromPoint4x1(shapes[i].triangles[j].c);

                    var t1 = MyMath.MyMatrix4x1.ToPoint(WVP * p0);
                    var t2 = MyMath.MyMatrix4x1.ToPoint(WVP * p1);
                    var t3 = MyMath.MyMatrix4x1.ToPoint(WVP * p2);

                    var intensivity = 0.5 + 0.5 * Math.Abs(shapes[i].triangles[j].normal.Normalize().Z);

                    //Debug.WriteLine("normal = "+shapes[i].triangles[j].normal);
                    //Debug.WriteLine("light dir = "+light_dir);
                    //Debug.WriteLine("intensivity = " + intensivity);
                    //intensivity = 1;

                    //var main_color = Render.color(255, 255, 255);
                    var main_color = Render.color(255, 153, 153);
                    var color_lines = Render.color(0, 0, 0);

                    var clr = Render.color(
                        (int)(main_color.R * intensivity),
                        (int)(main_color.G * intensivity),
                        (int)(main_color.B * intensivity)
                        );
                    lock (_lock)
                    {
                        Render.AndYetAnotherMemesSavedTheWorld(t1, t2, t3, clr, zbuffer, pixels, bitmapData, bytesPerPixel);
                        //Render.DrawTriangle(t1, t2, t3, clr, zbuffer, pixels, bitmapData, bytesPerPixel);
                    }
                    //Render.p_triangle_old(t1, t2, t3, (float)intensivity, (float)intensivity, (float)intensivity, pixels, bitmapData, bytesPerPixel, height, width, clr, zbuffer);
                    //Render.AndYetAnotherMemesSavedTheWorld(t1, t2, t3, clr, zbuffer, pixels, bitmapData, bytesPerPixel);
                    //Render.DrawLine((int)t1.X+width/2, (int)t1.Y + height/2, (int)t1.Z, (int)t2.X+width/2, (int)t2.Y + height/2, t2.Z, color_lines, pixels, bitmapData, bytesPerPixel, null);
                    //Render.DrawLine((int)t1.X+width/2, (int)t1.Y + height / 2, (int)t1.Z, (int)t3.X+width/2, (int)t3.Y + height/2, t3.Z, color_lines, pixels, bitmapData, bytesPerPixel, null);
                    //Render.DrawLine((int)t3.X+width/2, (int)t3.Y + height / 2, (int)t3.Z, (int)t2.X+width/2, (int)t2.Y + height/2, t2.Z, color_lines, pixels, bitmapData, bytesPerPixel, null);
                }
                );
                if (first_time == true) {
                    for (int y = 0; y < heightInPixels; y++)
                    {
                        int currentLine = y * bitmapData.Stride;
                        for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                        {
                            // calculate new pixel value
                            pixels[currentLine + x] = (byte)225;
                            pixels[currentLine + x + 1] = (byte)225;
                            pixels[currentLine + x + 2] = (byte)225;
                        }
                    }
                    first_time = false;
                    goto again;
                }
            }
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            bm.UnlockBits(bitmapData);
            return bm;

        }

    }
}
