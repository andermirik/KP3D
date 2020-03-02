using Ara3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace KP3D.Shapes
{
    class IcoSphere : Scene.Shape
    {

        public IcoSphere(int level = 3)
        {
            type = "IcoSphere";
            triangles = new List<MyTriangle>();
            Create(level);
        }

        private struct TriangleIndices
        {
            public int v1;
            public int v2;
            public int v3;

            public TriangleIndices(int v1, int v2, int v3)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
            }
        }

        // return index of point in the middle of p1 and p2
        public static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
        {
            // first check if we have it already
            bool firstIsSmaller = p1 < p2;
            long smallerIndex = firstIsSmaller ? p1 : p2;
            long greaterIndex = firstIsSmaller ? p2 : p1;
            long key = (smallerIndex << 32) + greaterIndex;

            int ret;
            if (cache.TryGetValue(key, out ret))
            {
                return ret;
            }

            // not in cache, calculate it
            Vector3 point1 = vertices[p1];
            Vector3 point2 = vertices[p2];
            Vector3 middle = new Vector3
            (
                (point1.X + point2.X) / 2f,
                (point1.Y + point2.Y) / 2f,
                (point1.Z + point2.Z) / 2f
            );

            // add vertex makes sure point is on unit sphere
            int i = vertices.Count;
            vertices.Add(middle.Normalize() * radius);

            // store it, return index
            cache.Add(key, i);

            return i;
        }

        public void Create(int recursionLevell=3)
        {

            List<Vector3> vertList = new List<Vector3>();
            Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
            int index = 0;

            int recursionLevel = recursionLevell;
            float radius = 1f;

            // create 12 vertices of a icosahedron
            float t = (1f + (float)Math.Sqrt(5f)) / 2f;

            vertList.Add(new Vector3(-1f, t, 0f).Normalize() * radius);
            vertList.Add(new Vector3(1f, t, 0f).Normalize() * radius);
            vertList.Add(new Vector3(-1f, -t, 0f).Normalize() * radius);
            vertList.Add(new Vector3(1f, -t, 0f).Normalize() * radius);

            vertList.Add(new Vector3(0f, -1f, t).Normalize() * radius);
            vertList.Add(new Vector3(0f, 1f, t).Normalize() * radius);
            vertList.Add(new Vector3(0f, -1f, -t).Normalize() * radius);
            vertList.Add(new Vector3(0f, 1f, -t).Normalize() * radius);

            vertList.Add(new Vector3(t, 0f, -1f).Normalize() * radius);
            vertList.Add(new Vector3(t, 0f, 1f).Normalize() * radius);
            vertList.Add(new Vector3(-t, 0f, -1f).Normalize() * radius);
            vertList.Add(new Vector3(-t, 0f, 1f).Normalize() * radius);


            // create 20 triangles of the icosahedron
            List<TriangleIndices> faces = new List<TriangleIndices>();

            // 5 faces around point 0
            faces.Add(new TriangleIndices(0, 11, 5));
            faces.Add(new TriangleIndices(0, 5, 1));
            faces.Add(new TriangleIndices(0, 1, 7));
            faces.Add(new TriangleIndices(0, 7, 10));
            faces.Add(new TriangleIndices(0, 10, 11));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(1, 5, 9));
            faces.Add(new TriangleIndices(5, 11, 4));
            faces.Add(new TriangleIndices(11, 10, 2));
            faces.Add(new TriangleIndices(10, 7, 6));
            faces.Add(new TriangleIndices(7, 1, 8));

            // 5 faces around point 3
            faces.Add(new TriangleIndices(3, 9, 4));
            faces.Add(new TriangleIndices(3, 4, 2));
            faces.Add(new TriangleIndices(3, 2, 6));
            faces.Add(new TriangleIndices(3, 6, 8));
            faces.Add(new TriangleIndices(3, 8, 9));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(4, 9, 5));
            faces.Add(new TriangleIndices(2, 4, 11));
            faces.Add(new TriangleIndices(6, 2, 10));
            faces.Add(new TriangleIndices(8, 6, 7));
            faces.Add(new TriangleIndices(9, 8, 1));


            // refine triangles
            for (int i = 0; i < recursionLevel; i++)
            {
                List<TriangleIndices> faces2 = new List<TriangleIndices>();
                foreach (var tri in faces)
                {
                    // replace triangle by 4 triangles
                    int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
                    int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
                    int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

                    faces2.Add(new TriangleIndices(tri.v1, a, c));
                    faces2.Add(new TriangleIndices(tri.v2, b, a));
                    faces2.Add(new TriangleIndices(tri.v3, c, b));
                    faces2.Add(new TriangleIndices(a, b, c));
                }
                faces = faces2;
            }

            var vertices = vertList.ToArray();

            //List<int> triList = new List<int>();
            for (int i = 0; i < faces.Count; i++)
            {
                //triList.Add(faces[i].v1);
                //triList.Add(faces[i].v2);
                //triList.Add(faces[i].v3);

                triangles.Add(new Shapes.MyTriangle(vertices[faces[i].v1], vertices[faces[i].v2], vertices[faces[i].v3]));
                
            }
            for (int i = 0; i < triangles.Count; i++)
            {
                float _a = triangles[i].a.X + triangles[i].b.X + triangles[i].c.X;
                float _b = triangles[i].a.Y + triangles[i].b.Y + triangles[i].c.Y;
                float _c = triangles[i].a.Z + triangles[i].b.Z + triangles[i].c.Z;
                Vector3 v = new Vector3(_a / 3, _b / 3, _c / 3);

                triangles[i].normal = (triangles[i].c - v).Cross(triangles[i].a - v);

                //одна вершина 
                //triangles[i].normal = (triangles[i].c - triangles[i].b).Cross(triangles[i].a - triangles[i].b).Normalize();
                if (triangles[i].normal.Y < 0) //or whatever direction up is
                    triangles[i].normal = -triangles[i].normal;
            }


            //var temp_triangles = triList.ToArray();
            //var uv = new Vector2[vertices.Length];

            // Vector3[] normales = new Vector3[vertList.Count];
            //for (int i = 0; i < vertList.Count; i++)
            //triangles[i].normal = vertList[i].Normalize();


            //var normals = normales;

            //var new_vertices = new Vector3[triangles.Length*3];


            //return new_vertices;
        }
    }
}
