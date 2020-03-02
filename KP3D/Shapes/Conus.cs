using Ara3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace KP3D.Shapes
{
    class Conus : Scene.Shape
    {
        public Conus(int subdivisions=60, float radius=1, float height=2)
        {
            type = "Conus";
            triangles = new List<MyTriangle>();
            ToTriangles(subdivisions, radius, height);
        }
        public void ToTriangles(int subdivisions, float radius, float height)
        {
            Vector3[] vertices = new Vector3[subdivisions + 2];

			int[] itriangles = new int[(subdivisions * 2) * 3];

			vertices[0] = Vector3.Zero;

			for (int i = 0, n = subdivisions - 1; i < subdivisions; i++)
			{
				float ratio = (float)i / n;
				float r = ratio * ((float)Math.PI * 2f);
				float x = (float)Math.Cos(r) * radius;
				float z = (float)Math.Sin(r) * radius;
				vertices[i + 1] = new Vector3(x, 0f, z);

			}
			vertices[subdivisions + 1] = new Vector3(0f, height, 0f);

			// construct bottom

			for (int i = 0, n = subdivisions - 1; i < n; i++)
			{
				int offset = i * 3;
				itriangles[offset] = 0;
				itriangles[offset + 1] = i + 1;
				itriangles[offset + 2] = i + 2;
			}

			// construct sides

			int bottomOffset = subdivisions * 3;
			for (int i = 0, n = subdivisions - 1; i < n; i++)
			{
				int offset = i * 3 + bottomOffset;
				itriangles[offset] = i + 1;
				itriangles[offset + 1] = subdivisions + 1;
				itriangles[offset + 2] = i + 2;
			}

			for (int i = 0; i < itriangles.Length; i+=3)
			{
				triangles.Add(new Shapes.MyTriangle(vertices[itriangles[i]], vertices[itriangles[i+1]], vertices[itriangles[i+2]]));
			}

			for (int i = 0; i < triangles.Count; i++)
			{
				float _a = triangles[i].a.X + triangles[i].b.X + triangles[i].c.X;
				float _b = triangles[i].a.Y + triangles[i].b.Y + triangles[i].c.Y;
				float _c = triangles[i].a.Z + triangles[i].b.Z + triangles[i].c.Z;
				Vector3 v = new Vector3(_a / 3, _b / 3, _c / 3);

				triangles[i].normal = (triangles[i].c - v).Cross(triangles[i].a - v);

				if (triangles[i].normal.Y < 0) //or whatever direction up is
					triangles[i].normal = -triangles[i].normal;
			}

		}
    }
}
