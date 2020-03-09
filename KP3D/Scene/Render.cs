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

        public static void DrawLine(int x0, int y0, double z0, int x1, int y1, double z1, System.Drawing.Color clr, byte[] pixels, BitmapData bd, int BytesPerPixel, float[] zBuffer, int[] id_object_in_pixel, int id, int[] id_group_in_pixel, int gid)
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
                                if(id_object_in_pixel != null)
                                    id_object_in_pixel[index] = id;
                                if(id_group_in_pixel != null)
                                    id_group_in_pixel[index] = gid;
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

        public static void AndYetAnotherMemesSavedTheWorld(Vector3 v1, Vector3 v2, Vector3 v3, System.Drawing.Color clr, float[] zBuffer, byte[] pixels, BitmapData bd, int BytesPerPixel, int[] id_object_in_pixel, int id, int[] id_group_in_pixel, int gid)
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

                    DrawLine((int)pStart.X, (int)pStart.Y, pStart.Z, (int)pEnd.X, (int)pEnd.Y, pEnd.Z, clr, pixels, bd, BytesPerPixel, zBuffer, id_object_in_pixel, id, id_group_in_pixel, gid);
                }

            }
        }
    }
}
