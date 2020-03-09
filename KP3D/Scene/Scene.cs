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
        //public List<Shape> shapes;
        public List<GroupShapes> groups;

        public float[] zbuffer;
        public int[] id_object_in_pixel;
        public int[] id_group_in_pixel;

        public int width;
        public int height;

        public int selected_id = -1;
        public int selected_gid = -1;

        public Scene(Camera camera)
        {
            this.camera = camera;
            //this.shapes = new List<Shape>();
            this.groups = new List<GroupShapes>();
        }

        public System.Drawing.Bitmap render(int width, int height, bool is_lines)
        {
            this.width = width;
            this.height = height;
            Bitmap bm = new Bitmap(width, height);

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
                    pixels[currentLine + x] = (byte)225;
                    pixels[currentLine + x + 1] = (byte)225;
                    pixels[currentLine + x + 2] = (byte)225;
                }
            }

            zbuffer = new float[width * height];
            id_object_in_pixel = new int[width * height];
            id_group_in_pixel = new int[width * height];
            for (int j = 0; j < width * height; j++)
            {
                zbuffer[j] = -0xFFFFFFFF;
                id_object_in_pixel[j] = -1;
                id_group_in_pixel[j] = -1;
            }

            for (int g = 0; g < groups.Count; g++)
            {
                float rotGX = groups[g].rotation_x;
                float rotGY = groups[g].rotation_y;
                float rotGZ = groups[g].rotation_z;

                float scaleGX = groups[g].scale_x;
                float scaleGY = groups[g].scale_y;
                float scaleGZ = groups[g].scale_z;

                float Gdx = groups[g].dx;
                float Gdy = groups[g].dy;
                float Gdz = groups[g].dz;

                for (int i = 0; i < groups[g].shapes.Count; i++)
                {
                    
                    var RotX = MyMath.MyMatrix4x4.CreateRotationX(groups[g].shapes[i].rotation_x);
                    var RotY = MyMath.MyMatrix4x4.CreateRotationY(groups[g].shapes[i].rotation_y);
                    var RotZ = MyMath.MyMatrix4x4.CreateRotationZ(groups[g].shapes[i].rotation_z);

                    var RotXG = MyMath.MyMatrix4x4.CreateRotationX(rotGX);
                    var RotYG = MyMath.MyMatrix4x4.CreateRotationY(rotGY);
                    var RotZG = MyMath.MyMatrix4x4.CreateRotationZ(rotGZ);

                    var Rot = RotX * RotY * RotZ;
                    var RotG = RotXG * RotYG * RotZG;


                    var Scale = MyMath.MyMatrix4x4.CreateScale(
                        groups[g].shapes[i].scale_x * scaleGX,
                        groups[g].shapes[i].scale_y * scaleGY,
                        groups[g].shapes[i].scale_z * scaleGZ
                        );

                    var TL = MyMath.MyMatrix4x4.CreateTranslation(
                        (groups[g].shapes[i].dx + Gdx) * scaleGX,
                        (groups[g].shapes[i].dy + Gdy) * scaleGY,
                        (groups[g].shapes[i].dz + Gdz) * scaleGZ
                        );

                    var TLG = MyMath.MyMatrix4x4.CreateTranslation(
                        -(groups[g].shapes[i].dx + Gdx) * scaleGX,
                        -(groups[g].shapes[i].dy + Gdy) * scaleGY,
                        -(groups[g].shapes[i].dz + Gdz) * scaleGZ
                        );

                    var W =  TL * TLG * RotG * TL * Rot * Scale;

                    var V = MyMath.MyMatrix4x4.CreateFPSLookAt(camera.eye, camera.pitch, camera.yaw);
                    if (camera.center != Vector3.MinValue)
                        V = MyMath.MyMatrix4x4.CreateLookAt(camera.eye, camera.center, new Vector3(0, 1, 0));

                    float far = 100;
                    float near = 0.01f;

                    float A = 1 / (float)Math.Tan(camera.fov * 0.5f);
                    float B = 1 / (float)Math.Tan(camera.fov * 0.5f);
                    float C = far / (far - near);
                    float D = -1;
                    float E = near * far / (far - near);

                    //var P = MyMath.MyMatrix4x4.CreateProjectionFOV(camera.fov, (float)width / (float)height, near, far);
                    var P = MyMath.MyMatrix4x4.CreateProjectionFOV(A, B, C, D, E);
                    Object _lock = new Object();
                    Parallel.For(0, groups[g].shapes[i].triangles.Count, new ParallelOptions { MaxDegreeOfParallelism = 16 }, j =>
                    //for (int j = 0; j < shapes[i].triangles.Count; j++)
                    {
                        var p0 = MyMath.MyMatrix4x1.FromPoint4x1(groups[g].shapes[i].triangles[j].a);
                        var p1 = MyMath.MyMatrix4x1.FromPoint4x1(groups[g].shapes[i].triangles[j].b);
                        var p2 = MyMath.MyMatrix4x1.FromPoint4x1(groups[g].shapes[i].triangles[j].c);

                        var t1 = MyMath.MyMatrix4x1.ToPoint(P * V * W * p0, A, B, C, D, E, width, height);
                        var t2 = MyMath.MyMatrix4x1.ToPoint(P * V * W * p1, A, B, C, D, E, width, height);
                        var t3 = MyMath.MyMatrix4x1.ToPoint(P * V * W * p2, A, B, C, D, E, width, height);


                        var intensivity = 0.5 + 0.5 * Math.Abs(groups[g].shapes[i].triangles[j].normal.Normalize().Z);
                        if (intensivity.IsNaN())
                            intensivity = 0.5;


                        var color = Render.color(255, 255, 255);

                        if (i == selected_id && g == selected_gid)
                        {
                            color = groups[g].shapes[i].select_clr;
                        }
                        else
                        {
                            color = groups[g].shapes[i].main_clr;
                        }

                        var clr = Render.color(
                            (int)(color.B * intensivity),
                            (int)(color.G * intensivity),
                            (int)(color.R * intensivity)
                            );
                        lock (_lock)
                        {
                            if (!is_lines)
                            {
                                Render.AndYetAnotherMemesSavedTheWorld(t1, t2, t3, clr, zbuffer, pixels, bitmapData, bytesPerPixel, id_object_in_pixel, i, id_group_in_pixel, g);
                            }
                            else
                            {
                                Render.DrawLine((int)t1.X + width / 2, (int)t1.Y + height / 2, (int)t1.Z, (int)t2.X + width / 2, (int)t2.Y + height / 2, t2.Z, color, pixels, bitmapData, bytesPerPixel, null, id_object_in_pixel, i, id_group_in_pixel, g);
                                Render.DrawLine((int)t1.X + width / 2, (int)t1.Y + height / 2, (int)t1.Z, (int)t3.X + width / 2, (int)t3.Y + height / 2, t3.Z, color, pixels, bitmapData, bytesPerPixel, null, id_object_in_pixel, i, id_group_in_pixel, g);
                                Render.DrawLine((int)t3.X + width / 2, (int)t3.Y + height / 2, (int)t3.Z, (int)t2.X + width / 2, (int)t2.Y + height / 2, t2.Z, color, pixels, bitmapData, bytesPerPixel, null, id_object_in_pixel, i, id_group_in_pixel, g);
                            }
                        }

                    }
                    );
                }
            }
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            bm.UnlockBits(bitmapData);
            return bm;

        }

    }
}
