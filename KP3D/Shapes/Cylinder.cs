using Ara3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace KP3D.Shapes
{
	class Cylinder : Scene.Shape
	{
		public Cylinder()
		{
			type = "Cylinder";
			triangles = new List<MyTriangle>();
			ToTriangles();
		}
		public void ToTriangles()
		{
			float radius = 1.0f;
			float length = 2.0f;

			int radialSegments = 60;
			int heightSegments = 6;

			int numVertexColumns = radialSegments + 1;  //+1 for welding
			int numVertexRows = heightSegments + 1;

			int numVertices = numVertexColumns * numVertexRows;

			int numSideTris = radialSegments * heightSegments * 2;      //for one cap
			int numCapTris = radialSegments - 2;                        //fact

			int trisArrayLength = (numSideTris + numCapTris * 2) * 3;   //3 places in the array for each tri

			//initialize arrays
			Vector3[] Vertices = new Vector3[numVertices];

			int[] Tris = new int[trisArrayLength];

			//precalculate increments to improve performance
			float heightStep = length / heightSegments;
			float angleStep = 2 * (float)Math.PI / radialSegments;

			for (int j = 0; j < numVertexRows; j++)
			{
				for (int i = 0; i < numVertexColumns; i++)
				{
					//calculate angle for that vertex on the unit circle
					float angle = i * angleStep;

					//"fold" the sheet around as a cylinder by placing the first and last vertex of each row at the same spot
					if (i == numVertexColumns - 1)
					{
						angle = 0;
					}

					//position current vertex
					Vertices[j * numVertexColumns + i] = new Vector3(radius * (float)Math.Cos(angle), j * heightStep, radius * (float)Math.Sin(angle));

					//create the tris				
					if (j == 0 || i >= numVertexColumns - 1)
					{
						//nothing to do on the first and last "floor" on the tris, capping is done below
						//also nothing to do on the last column of vertices
						continue;
					}
					else
					{
						//create 2 tris below each vertex
						//6 seems like a magic number. For every vertex we draw 2 tris in this for-loop, therefore we need 2*3=6 indices in the Tris array
						//offset the base by the number of slots we need for the bottom cap tris. Those will be populated once we draw the cap
						int baseIndex = numCapTris * 3 + (j - 1) * radialSegments * 6 + i * 6;

						//1st tri - below and in front
						Tris[baseIndex + 0] = j * numVertexColumns + i;
						Tris[baseIndex + 1] = j * numVertexColumns + i + 1;
						Tris[baseIndex + 2] = (j - 1) * numVertexColumns + i;

						//2nd tri - the one it doesn't touch
						Tris[baseIndex + 3] = (j - 1) * numVertexColumns + i;
						Tris[baseIndex + 4] = j * numVertexColumns + i + 1;
						Tris[baseIndex + 5] = (j - 1) * numVertexColumns + i + 1;
					}
				}
			}

			//draw caps
			bool leftSided = true;
			int leftIndex = 0;
			int rightIndex = 0;
			int middleIndex = 0;
			int topCapVertexOffset = numVertices - numVertexColumns;
			for (int i = 0; i < numCapTris; i++)
			{
				int bottomCapBaseIndex = i * 3;
				int topCapBaseIndex = (numCapTris + numSideTris) * 3 + i * 3;

				if (i == 0)
				{
					middleIndex = 0;
					leftIndex = 1;
					rightIndex = numVertexColumns - 2;
					leftSided = true;
				}
				else if (leftSided)
				{
					middleIndex = rightIndex;
					rightIndex--;
				}
				else
				{
					middleIndex = leftIndex;
					leftIndex++;
				}
				leftSided = !leftSided;

				//assign bottom tris
				Tris[bottomCapBaseIndex + 0] = rightIndex;
				Tris[bottomCapBaseIndex + 1] = middleIndex;
				Tris[bottomCapBaseIndex + 2] = leftIndex;

				//assign top tris
				Tris[topCapBaseIndex + 0] = topCapVertexOffset + leftIndex;
				Tris[topCapBaseIndex + 1] = topCapVertexOffset + middleIndex;
				Tris[topCapBaseIndex + 2] = topCapVertexOffset + rightIndex;
			}

			for (int i = 0; i < Tris.Length; i += 3)
			{
				triangles.Add(new Shapes.MyTriangle(Vertices[Tris[i]], Vertices[Tris[i + 1]], Vertices[Tris[i + 2]]));
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
