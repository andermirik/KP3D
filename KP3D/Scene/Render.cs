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

        public static void line(int x0, int y0, int x1, int y1, byte[] pixels, int BytesPerPixel, BitmapData bd, float[] zbuffer, System.Drawing.Color clr)
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
                if (y >= 0 && x >= 0 && x < bd.Width && y < bd.Height)
                {
                    int tx = y;
                    int ty = x;
                    if (steep)
                    {
                        int currentLine = (int)ty * bd.Stride;

                        pixels[currentLine + (int)tx * BytesPerPixel] = (byte)(clr.R);
                        pixels[currentLine + (int)tx * BytesPerPixel + 1] = (byte)(clr.G);
                        pixels[currentLine + (int)tx * BytesPerPixel + 2] = (byte)(clr.B);
                    }
                    else
                    {
                        int currentLine = (int)tx * bd.Stride;

                        pixels[currentLine + (int)ty * BytesPerPixel] = (byte)(clr.R);
                        pixels[currentLine + (int)ty * BytesPerPixel + 1] = (byte)(clr.G);
                        pixels[currentLine + (int)ty * BytesPerPixel + 2] = (byte)(clr.B);
                    }
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

                        int currentLine = (int)P.Y * bd.Stride;

                        pixels[currentLine + (int)P.X * BytesPerPixel] = (byte)(clr.R * ityP);
                        pixels[currentLine + (int)P.X * BytesPerPixel + 1] = (byte)(clr.G * ityP);
                        pixels[currentLine + (int)P.X * BytesPerPixel + 2] = (byte)(clr.B * ityP);
                    }
                }
            }
        }


        static void fillBottomFlatTriangle(Vector3 v1, Vector3 v2, Vector3 v3, byte[] pixels, BitmapData bd, int BytesPerPixel, System.Drawing.Color clr, float[] zbuffer)
        {
            var invslope1 = (v2.X - v1.X) / (v2.Y - v1.Y);
            var invslope2 = (v3.X - v1.X) / (v3.Y - v1.Y);

            var curx1 = v1.X;
            var curx2 = v1.X;

            for (var scanlineY = v1.Y; scanlineY <= v2.Y; scanlineY++)
            {
                
                line((int)curx1+bd.Width/2, (int)scanlineY + bd.Height / 2, (int)curx2 + bd.Width / 2, (int)scanlineY+ bd.Height/2, pixels, BytesPerPixel, bd, zbuffer, clr);
                curx1 += invslope1;
                curx2 += invslope2;
            }
        }


        public static void easy_triangle(Vector3 v1, Vector3 v2, Vector3 v3, System.Drawing.Color clr, byte[] pixels, BitmapData bd, int BytesPerPixel)
        {
            int x0 = (int)v1.X;
            int x1 = (int)v2.X;
            int x2 = (int)v3.X;

            int y0 = (int)v1.Y;
            int y1 = (int)v2.Y;
            int y2 = (int)v3.Y;
            if (y1 > y2)
            {
                Swap(ref x1, ref x2);
                Swap(ref y1, ref y2);
            }
            if (y0 > y1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            if (y1 > y2)
            {
                Swap(ref x1, ref x2);
                Swap(ref y1, ref y2);
            }

            y0 += bd.Height / 2;
            y1 += bd.Height / 2;
            y2 += bd.Height / 2;
            x0 += bd.Width / 2;
            x1 += bd.Width / 2;
            x2 += bd.Width / 2;

            double dx_far = Convert.ToDouble(x2 - x0) / (y2 - y0 + 1);
            double dx_upper = Convert.ToDouble(x1 - x0) / (y1 - y0 + 1);
            double dx_low = Convert.ToDouble(x2 - x1) / (y2 - y1 + 1);
            double xf = x0;
            double xt = x0 + dx_upper; // if y0 == y1, special case


            for (int y = y0; y <= (y2 > bd.Height - 1 ? bd.Height - 1 : y2); y++)
            {
                if (y >= 0)
                {
                    for (int x = (xf > 0 ? Convert.ToInt32(xf) : 0); x <= (xt < bd.Width ? xt : bd.Width - 1); x++)
                    {
                        int currentLine = (int)y * bd.Stride;

                        pixels[currentLine + (int)x * BytesPerPixel] = (byte)(clr.R);
                        pixels[currentLine + (int)x * BytesPerPixel + 1] = (byte)(clr.G);
                        pixels[currentLine + (int)x * BytesPerPixel + 2] = (byte)(clr.B);
                      
                    }
                    for (int x = (xf < bd.Width ? Convert.ToInt32(xf) : bd.Width - 1); x >= (xt > 0 ? xt : 0); x--)
                    {    
                        int currentLine = (int)y * bd.Stride;

                        pixels[currentLine + (int)x * BytesPerPixel] = (byte)(clr.R);
                        pixels[currentLine + (int)x * BytesPerPixel + 1] = (byte)(clr.G);
                        pixels[currentLine + (int)x * BytesPerPixel + 2] = (byte)(clr.B);
                    }
                }
                xf += dx_far;
                if (y < y1)
                    xt += dx_upper;
                else
                    xt += dx_low;
            }

        }

        private static void FillBottomFlatTriangle(Vector3 a, Vector3 b, Vector3 c, float[] zBuffer, System.Drawing.Color clr, byte[] pixels, BitmapData bd, int BytesPerPixel)
        {
            double invslope1 = 1.0 * (b.X - a.X) / (b.Y - a.Y);
            double invslope2 = 1.0 * (c.X - a.X) / (c.Y - a.Y);

            double slopez1 = 1.0 * (b.Z - a.Z) / (b.Y - a.Y);
            double slopez2 = 1.0 * (c.Z - a.Z) / (c.Y - a.Y);

            double curx1 = a.X;
            double curx2 = a.X;

            double zLeft = a.Z;
            double zRight = a.Z;

            for (int scanlineY = (int)a.Y; scanlineY <= b.Y; scanlineY++)
            {
                DrawLine((int)curx1 + bd.Width / 2, scanlineY + bd.Height / 2, zLeft, (int)curx2 + bd.Width / 2, scanlineY + bd.Height / 2, zRight, clr, pixels, bd, BytesPerPixel, zBuffer);

                curx1 += invslope1;
                curx2 += invslope2;

                zLeft += slopez1;
                zRight += slopez2;
            }
        }

        private static void FillTopFlatTriangle(Vector3 a, Vector3 b, Vector3 c, float[] zBuffer, System.Drawing.Color clr, byte[] pixels, BitmapData bd, int BytesPerPixel)
        {
            double slope1 = 1.0 * (c.X - a.X) / (c.Y - a.Y);
            double slope2 = 1.0 * (c.X - b.X) / (c.Y - b.Y);

            double slopez1 = 1.0 * (c.Z - a.Z) / (c.Y - a.Y);
            double slopez2 = 1.0 * (c.Z - b.Z) / (c.Y - b.Y);

            double currentX1 = c.X;
            double currentX2 = c.X;

            double leftZ = c.Z;
            double rightZ = c.Z;

            for (int scanlineY = (int)c.Y; scanlineY > a.Y; scanlineY--)
            {
                DrawLine((int)currentX1+bd.Width/2, scanlineY + bd.Height / 2, leftZ, (int)currentX2 + bd.Width / 2, scanlineY + bd.Height / 2, rightZ, clr, pixels, bd, BytesPerPixel, zBuffer);
                currentX1 -= slope1;
                currentX2 -= slope2;

                leftZ -= slopez1;
                rightZ -= slopez2;
            }
        }

        public static void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, System.Drawing.Color clr, float[] zBuffer, byte[] pixels, BitmapData bd, int BytesPerPixel)
        {
            if (a.Y > c.Y)
            {
                Swap(ref a, ref c);
            }

            if (a.Y > b.Y)
            {
                Swap(ref a, ref b);
            }

            if (b.Y > c.Y)
            {
                Swap(ref b, ref c);
            }


            if ((b.Y == c.Y))
            {
                FillBottomFlatTriangle(a, b, c, zBuffer, clr, pixels, bd, BytesPerPixel);
            }
            else if (a.Y == b.Y)
            {
                FillTopFlatTriangle(a, b, c, zBuffer, clr, pixels, bd, BytesPerPixel);
            }
            else
            {
                double vz = a.Z + (1.0 * (b.Y - a.Y) / (c.Y - a.Y)) * (c.Z - a.Z);
                Vector3 v = new Vector3((int)(a.X + (1.0 * (b.Y - a.Y) / (c.Y - a.Y)) * (c.X - a.X)), b.Y, (float)vz);

                FillBottomFlatTriangle(a, b, v, zBuffer, clr, pixels, bd, BytesPerPixel);
                FillTopFlatTriangle(b, v, c, zBuffer, clr, pixels, bd, BytesPerPixel);
            }
        }

        public static void DrawLine(int x0, int y0, double z0, int x1, int y1, double z1, System.Drawing.Color clr, byte[] pixels, BitmapData bd, int BytesPerPixel, float[] zBuffer = null)
        {
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0;
                x0 = y0;
                y0 = t;
                t = x1;
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0;
                x0 = x1;
                x1 = t;
                t = y0;
                y0 = y1;
                y1 = t;
                Swap(ref z0, ref z1);
            }
            var dx = x1 - x0;
            var dy = Math.Abs(y1 - y0);
            var error = dx / 2;
            var ystep = y0 < y1 ? 1 : -1;
            var y = y0;

            double deltaZ = (z1 - z0) / (x1 - x0);
            double z = z0;

            for (var x = x0; x <= x1; x++)
            {
                if (x >= bd.Width)
                    return;

                if (x > 0)
                {
                    bool shouldBeDrawn = false;

                    if (zBuffer == null)
                    {
                        //don't use Z buffer
                        shouldBeDrawn = true;
                    }
                    else
                    {
                        //check Z buffer
                        int index = x + y * bd.Width;
                        if (index < zBuffer.Length && index > 0)
                        {
                            if (z > zBuffer[index])
                            {
                                zBuffer[index] = (float)z;
                                shouldBeDrawn = true;
                            }
                        }
                    }

                    if (shouldBeDrawn)
                        SetPixel(steep ? y : x, steep ? x : y, clr, pixels, bd, BytesPerPixel);
                }


                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
                z += deltaZ;
            }
        }

        public static void SetPixel(int x, int y, System.Drawing.Color clr, byte[] pixels, BitmapData bd, int BytesPerPixel)
        {
            if (x >= 0 && y >= 0 && x < bd.Width && y < bd.Height)
            {
                int currentLine = (int)y * bd.Stride;

                pixels[currentLine + (int)x * BytesPerPixel] = (byte)(clr.R);
                pixels[currentLine + (int)x * BytesPerPixel + 1] = (byte)(clr.G);
                pixels[currentLine + (int)x * BytesPerPixel + 2] = (byte)(clr.B);
            }
        }

        public static void AndYetAnotherMemesSavedTheWorld(Vector3 v1, Vector3 v2, Vector3 v3, System.Drawing.Color clr, float[] zBuffer, byte[] pixels, BitmapData bd, int BytesPerPixel)
        {
            Vector2 min = new Vector2(Math.Min(v1.X, Math.Min(v2.X, v3.X)), Math.Min(v1.Y, Math.Min(v2.Y, v3.Y)));
            Vector2 max = new Vector2(Math.Max(v1.X, Math.Max(v2.X, v3.X)), Math.Max(v1.Y, Math.Max(v2.Y, v3.Y)));

            //min = min.Clamp(Vector2.Zero, new Vector2(bd.Width, bd.Height));
            //max = max.Clamp(Vector2.Zero, new Vector2(bd.Width, bd.Height));

            Vector3[] verts = new Vector3[3] { v1, v2, v3 };

            for (int yp = (int)min.Y; yp <= (int)max.Y; yp++)
            {
                List<Vector3> intersections = new List<Vector3>();

                //voor alle lijnen van de triangle, vind alle intersecties (als het goed is altijd 2)
                for (int l = 0; l < 3; l++)
                {
                    Vector3 vert1, vert2;
                    vert1 = verts[l];
                    vert2 = verts[(l + 1) % 3];

                    float ymin, ymax;
                    ymin = Math.Min(vert1.Y, vert2.Y);
                    ymax = Math.Max(vert1.Y, vert2.Y);
                    if (yp > ymin && yp < ymax)//check of yp tussen de y-en van de vertices ligt, zo ja, intersectie
                    {
                        float xSlope = (vert2.X - vert1.X) / (vert2.Y - vert1.Y);
                        float xIntersect = vert1.X + xSlope * (yp - vert1.Y);

                        float zSlope = (vert2.Z - vert1.Z) / (vert2.Y - vert1.Y);
                        float zIntersect = vert1.Z + zSlope * (yp - vert1.Y);
                        intersections.Add(new Vector3((int)xIntersect+bd.Width/2, yp+bd.Height/2, zIntersect));
                    }
                }

                if (intersections.Count > 1)
                {
                    Vector3 pStart, pEnd;
                    if (intersections[0].X < intersections[1].X) { pStart = intersections[0]; pEnd = intersections[1]; }
                    else
                    { pStart = intersections[1]; pEnd = intersections[0]; }

                    DrawLine((int)pStart.X, (int)pStart.Y, pStart.Z, (int)pEnd.X, (int)pEnd.Y, pEnd.Z, clr, pixels, bd, BytesPerPixel, zBuffer);
                }

            }
        }
    }
}
