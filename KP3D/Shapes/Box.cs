using Ara3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace KP3D.Shapes
{
    class Box : Scene.Scene.Shape
    {
        public Box()
        {
            type = "Box";
            triangles = new List<MyTriangle>();
            ToTriangles();
        }
        public void ToTriangles()
        {

            //нижняя грань
            triangles.Add(new MyTriangle(
                new Vector3(0, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0)
                )
            );
            triangles.Add(new MyTriangle(
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0)
                )
            );

            //верхняя грань
            triangles.Add(new MyTriangle(
                new Vector3(0, 0, 1),
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1)
                )
            );
            triangles.Add(new MyTriangle(
               new Vector3(0, 0, 1),
               new Vector3(1, 0, 1),
               new Vector3(1, 1, 1)
               )
            );

            //левая грань
            triangles.Add(new MyTriangle(
               new Vector3(1, 0, 0),
               new Vector3(0, 0, 0),
               new Vector3(0, 0, 1)
               )
            );
            triangles.Add(new MyTriangle(
               new Vector3(1, 0, 0),
               new Vector3(1, 0, 1),
               new Vector3(0, 0, 1)
               )
           );
            //правая грань
            triangles.Add(new MyTriangle(
               new Vector3(1, 1, 0),
               new Vector3(0, 1, 0),
               new Vector3(0, 1, 1)
               )
           );
            triangles.Add(new MyTriangle(
               new Vector3(1, 1, 0),
               new Vector3(1, 1, 1),
               new Vector3(0, 1, 1)
               )
           );

            //передняя грань
            triangles.Add(new MyTriangle(
               new Vector3(1, 0, 0),
               new Vector3(1, 0, 1),
               new Vector3(1, 1, 1)
               )
            );
            triangles.Add(new MyTriangle(
               new Vector3(1, 0, 0),
               new Vector3(1, 1, 0),
               new Vector3(1, 1, 1)
               )
            );
            //задняя грань
            triangles.Add(new MyTriangle(
               new Vector3(0, 0, 0),
               new Vector3(0, 0, 1),
               new Vector3(0, 1, 1)
               )
            );

            triangles.Add(new MyTriangle(
               new Vector3(0, 0, 0),
               new Vector3(0, 1, 0),
               new Vector3(0, 1, 1)
               )
            );
            for(int i = 0; i < triangles.Count; i++)
            {
                float _a = triangles[i].a.X + triangles[i].b.X + triangles[i].c.X;
                float _b = triangles[i].a.Y + triangles[i].b.Y + triangles[i].c.Y;
                float _c = triangles[i].a.Z + triangles[i].b.Z + triangles[i].c.Z;
                Vector3 v = new Vector3(_a/3, _b/3, _c/3);

                triangles[i].normal = (triangles[i].c - v).Cross(triangles[i].a - v);
               
                //одна вершина 
                //triangles[i].normal = (triangles[i].c - triangles[i].b).Cross(triangles[i].a - triangles[i].b).Normalize();
                if (triangles[i].normal.Y < 0) //or whatever direction up is
                    triangles[i].normal = -triangles[i].normal;
            }
        }
    }
}
