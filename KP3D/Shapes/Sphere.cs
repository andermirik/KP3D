using Ara3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace KP3D.Shapes
{
    class Sphere
    {
        public Sphere()
        {

        }

        public Sphere(int lines)
        {
            if (lines < 3)
                lines = 10;
            horizontalLines = lines;
            verticalLines = lines;
        }

        public Vector3[] ToTriangles()
        {

            List <Vector3> points = new List<Vector3>();
            
            for (int m = 0; m < horizontalLines; m++)
            {
                for (int n = 0; n < verticalLines - 1; n++)
                {
                    double x = Math.Sin(Math.PI * m / horizontalLines) * Math.Cos(2 * Math.PI * n / verticalLines);
                    double y = Math.Sin(Math.PI * m / horizontalLines) * Math.Sin(2 * Math.PI * n / verticalLines);
                    double z = Math.Cos(Math.PI * m / horizontalLines);
                    points.Add(new Vector3((float)x, (float)y, (float)z)); 
                }
            }
            
           


            return MyTriangle.Triangulate(points);
        }



        public float radius;
        public Vector3 center;
        int horizontalLines = 60;
        int verticalLines = 60;
    }
}
