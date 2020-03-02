using Ara3D;
using EarClipperLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace KP3D.Shapes
{
    public class MyTriangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Vector3 normal;

        public MyTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }


        public static Vector3[] Triangulate(List<Vector3> points)
        {
            List<Vector3m> ppp = new List<Vector3m>();

            for (int i = 0; i < points.Count; i++)
                ppp.Add(new Vector3m(points[i].X, points[i].Y, points[i].Z));

            EarClipping earClipping = new EarClipping();
            earClipping.SetPoints(ppp);
            
            earClipping.Triangulate();
            var res = earClipping.Result;
            Vector3[] vertices = new Vector3[res.Count];
            for (int i = 0; i < res.Count; i++)
                vertices[i] = new Vector3(
                    (float)res[i].X.ToDouble(),
                    (float)res[i].Y.ToDouble(),
                    (float)res[i].Z.ToDouble()
                    );

            return vertices;    
        }

    }
}
