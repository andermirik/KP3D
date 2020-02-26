using Ara3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace KP3D.Scene
{
    class Render
    { 
        public static System.Drawing.Color color(int r, int g, int b)
        {
            return System.Drawing.Color.FromArgb(255, r, g, b);
        }
        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static void line(int x0, int y0, int x1, int y1, Bitmap bm, System.Drawing.Color clr)
        {
            bool steep = false;
            if (Math.Abs(x0 - x1) < Math.Abs(y0 - y1))
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
                steep = true;
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            int dx = x1 - x0;
            int dy = y1 - y0;
            int derror2 = Math.Abs(dy) * 2;
            int error2 = 0;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                if (steep)
                {
                    if (y >= 0 && x >= 0 && x <= bm.Width && y <= bm.Height)
                        bm.SetPixel(y, x, clr);
                }
                else
                {
                    if (y >= 0 && x >= 0 && x <= bm.Width && y <= bm.Height)
                        bm.SetPixel(x, y, clr);
                }
                error2 += derror2;
                if (error2 > dx)
                {
                    y += (y1 > y0 ? 1 : -1);
                    error2 -= dx * 2;
                }
            }
        }

        public static Vector3 barycentric(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
        {
            Vector3[] s = new Vector3[2];
            s[1] = new Vector3(C.Y - A.Y, B.Y - A.Y, A.Y - P.Y);
            s[0] = new Vector3(C.X - A.X, B.X - A.X, A.X - P.X);

            Vector3 u = s[0].Cross(s[1]);
            if (Math.Abs(u.Z) > 1e-2) // dont forget that u[2] is integer. If it is zero then triangle ABC is degenerate
                return new Vector3(1.0f - (u.X + u.Y) / u.Z, u.Y / u.Z, u.X / u.Z);
            return new Vector3(-1, 1, 1); // in this case generate negative coordinates, it will be thrown away by the rasterizator
        }

        public static void triangle(Vector3[] pts, float[] zbuffer, Bitmap image, System.Drawing.Color clr)
        {
            Vector2 bboxmin = new Vector2(0xFFFFFFFF, 0xFFFFFFFF);
            Vector2 bboxmax = new Vector2(-0xFFFFFFFF, -0xFFFFFFFF);
            Vector2 clamp = new Vector2(image.Width - 1, image.Height - 1);
            for (int i = 0; i < 3; i++)
            {
                bboxmin = new Vector2(Math.Max(0.0f, Math.Min(bboxmin.X, pts[i].X)), Math.Max(0.0f, Math.Min(bboxmin.Y, pts[i].Y)));
                bboxmax = new Vector2(Math.Min(clamp.X, Math.Max(bboxmax.X, pts[i].X)), Math.Min(clamp.Y, Math.Max(bboxmax.Y, pts[i].Y)));
            }
            float PX = 0;
            float PY = 0;
            float PZ = 0;
            for (PX = bboxmin.X; PX <= bboxmax.X; PX++)
            {
                for (PY = bboxmin.Y; PY <= bboxmax.Y; PY++)
                {
                    Vector3 bc_screen = barycentric(pts[0], pts[1], pts[2], new Vector3(PX, PY, PZ));
                    if (bc_screen.X < 0 || bc_screen.Y < 0 || bc_screen.Z < 0) continue;
                    PZ = 0;
                    PZ += pts[0].Z * bc_screen.X;
                    PZ += pts[1].Z * bc_screen.Y;
                    PZ += pts[2].Z * bc_screen.Z;
                    if (zbuffer[(int)(PX + PY * image.Width)] < PZ)
                    {
                        zbuffer[(int)(PX + PY * image.Width)] = PZ;
                        image.SetPixel((int)PX, (int)PY, clr);
                    }
                }
            }
        }


        public static void triangle_old(Vector3 t0, Vector3 t1, Vector3 t2, float ity0, float ity1, float ity2, Bitmap bm, System.Drawing.Color clr, float[] zbuffer)
        {
            if (t0.Y == t1.Y && t0.Y == t2.Y) return; // I dont care about degenerate triangles 
                                                      // sort the vertices, t0, t1, t2 lower−to−upper (bubblesort yay!) 
            if (t0.Y > t1.Y) Swap(ref t0, ref t1);
            if (t0.Y > t2.Y) Swap(ref t0, ref t2);
            if (t1.Y > t2.Y) Swap(ref t1, ref t2);
            int total_height = (int)t2.Y - (int)t0.Y;
            for (int i = 0; i < total_height; i++)
            {
                bool second_half = i > (int)t1.Y - (int)t0.Y || (int)t1.Y == (int)t0.Y;
                int segment_height = second_half ? (int)t2.Y - (int)t1.Y : (int)t1.Y - (int)t0.Y;
                
                float alpha = (float)i / total_height;
                float beta = (float)(i - (second_half ? (int)t1.Y - (int)t0.Y : 0)) / segment_height; // be careful: with above conditions no division by zero here 
               
                Vector3 A = t0 + (t2 - t0) * alpha;
                Vector3 B = second_half ? t1 + (t2 - t1) * beta : t0 + (t1 - t0) * beta;

                float ityA = ity0 + (ity2 - ity0) * alpha;
                float ityB = second_half ? ity1 + (ity2 - ity1) * beta : ity0 + (ity1 - ity0) * beta;

                if (A.X > B.X) Swap(ref A, ref B);
                for (int j = (int)A.X; j <= (int)B.X; j++)
                {
                    float phi = B.X == A.X ? 1.0f : (float)(j - A.X) / (B.X - A.X);
                    Vector3 P = A + (B - A) * phi;
                    float ityP = ityA + (ityB - ityA) * phi + 0.1f;
                    int idx = (int)P.X + (int)P.Y * bm.Width;

                    if (P.X >= bm.Width || P.Y >= bm.Height || P.X < 0 || P.Y < 0) continue;
                    if (zbuffer[idx] < P.Z)
                    {
                        zbuffer[idx] = P.Z;

                        ityP = (ityP > 1.0f ? 1.0f : (ityP < 0.0f ? 0.0f : ityP));

                        bm.SetPixel(
                            (int)P.X, (int)P.Y, 
                            Render.color(
                                (int)(clr.R * ityP),
                                (int)(clr.G * ityP),
                                (int)(clr.B * ityP)
                                )
                            );
                    }
                }
            }
        }

        public static void p_triangle_old(Vector3 t0, Vector3 t1, Vector3 t2, float ity0, float ity1, float ity2, byte[] pixels, BitmapData bd, int BytesPerPixel, int Height, int Width, System.Drawing.Color clr, float[] zbuffer)
        {
            if (t0.Y == t1.Y && t0.Y == t2.Y) return; // I dont care about degenerate triangles 
                                                      // sort the vertices, t0, t1, t2 lower−to−upper (bubblesort yay!) 
            if (t0.Y > t1.Y) Swap(ref t0, ref t1);
            if (t0.Y > t2.Y) Swap(ref t0, ref t2);
            if (t1.Y > t2.Y) Swap(ref t1, ref t2);
            int total_height = (int)t2.Y - (int)t0.Y;
            for (int i = 0; i < total_height; i++)
            {
                bool second_half = i > (int)t1.Y - (int)t0.Y || (int)t1.Y == (int)t0.Y;
                int segment_height = second_half ? (int)t2.Y - (int)t1.Y : (int)t1.Y - (int)t0.Y;

                float alpha = (float)i / total_height;
                float beta = (float)(i - (second_half ? (int)t1.Y - (int)t0.Y : 0)) / segment_height; // be careful: with above conditions no division by zero here 

                Vector3 A = t0 + (t2 - t0) * alpha;
                Vector3 B = second_half ? t1 + (t2 - t1) * beta : t0 + (t1 - t0) * beta;

                float ityA = ity0 + (ity2 - ity0) * alpha;
                float ityB = second_half ? ity1 + (ity2 - ity1) * beta : ity0 + (ity1 - ity0) * beta;

                if (A.X > B.X) Swap(ref A, ref B);
                for (int j = (int)A.X; j <= (int)B.X; j++)
                {
                    float phi = B.X == A.X ? 1.0f : (float)(j - A.X) / (B.X - A.X);
                    Vector3 P = A + (B - A) * phi;
                    float ityP = ityA + (ityB - ityA) * phi + 0.1f;
                    int idx = (int)P.X + (int)P.Y * Width;

                    if (P.X >= Width || P.Y >= Height || P.X < 0 || P.Y < 0) continue;
                    if (zbuffer[idx] < P.Z)
                    {
                        zbuffer[idx] = P.Z;

                        ityP = (ityP > 1.0f ? 1.0f : (ityP < 0.0f ? 0.0f : ityP));

                        int widthInBytes = bd.Width * BytesPerPixel;
                        int currentLine = (int)P.Y * bd.Stride;

                        pixels[currentLine + (int)P.X * BytesPerPixel] = (byte)(clr.R * ityP);
                        pixels[currentLine + (int)P.X * BytesPerPixel + 1] = (byte)(clr.G * ityP);
                        pixels[currentLine + (int)P.X * BytesPerPixel + 2] = (byte)(clr.B * ityP);
                    }
                }
            }
        }
    }
}
