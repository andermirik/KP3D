using Ara3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace KP3D.Shapes
{
    class Toroid : Scene.Shape
    {
        public Toroid(int nbrRingSegments=64, int nbrTubeSegments = 32, float ringRad = 1f, float tubeRadX = 0.35f, float tubeRadY = 0.35f, int tubeZeroPos = 90)
        {
            type = "Toroid";
            triangles = new List<MyTriangle>();
            ToTriangles(nbrRingSegments, nbrTubeSegments, ringRad, tubeRadX, tubeRadY, tubeZeroPos);
        }

        public void ToTriangles(int nbrRingSegments = 64, int nbrTubeSegments = 32, float ringRad = 1f, float tubeRadX = 0.35f, float tubeRadY = 0.35f, int tubeZeroPos = 90)
        {
            var vertices = new List<Vector3>();
            var indices = new List<int>();

            //int nbrRingSegments=64;
            //int nbrTubeSegments=32;
            //int tubeZeroPos = 90;


            //float ringRad = 1f;
            //float tubeRadX = 0.35f;
            //float tubeRadY = 0.35f;

            int nbrRingSteps = nbrRingSegments+1;
            int nbrTubeSteps = nbrTubeSegments+1;

            Vector3[,] coord = new Vector3[nbrRingSteps, nbrTubeSteps];
            // Calculate segment size in radians
            float ringDeltaAng = (float)(2 * Math.PI / nbrRingSegments);
            float tubeDeltaAng = (float)(2 * Math.PI / nbrTubeSegments);

            // Calculate the XY coordinates of the tube in the Z=0 plane
            for (int t = 0; t < nbrTubeSteps; t++)
            {
                float angle = tubeZeroPos + t * tubeDeltaAng;
                coord[0,t] = new Vector3(
                        ringRad + tubeRadX * (float)Math.Cos(angle),
                        tubeRadY * (float)Math.Sin(angle),
                        0
                );
            }
            // Calculate all the points for all the other ring segments
            for (int r = 1; r < nbrRingSteps; r++)
            {
                float angle = r * ringDeltaAng;
                float sinA = (float)Math.Sin(angle);
                float cosA = (float)Math.Cos(angle);
                for (int t = 0; t < nbrTubeSteps; t++)
                {
                    Vector3 point0 = coord[0, t];
                    coord[r, t] = new Vector3(
                            point0.X * cosA,
                            point0.Y,
                            point0.X * sinA
                    );
                }
            }
            // Transfer to float array
            Vector3[] points = new Vector3[nbrRingSteps * nbrTubeSteps];
            int id = 0;
            for (int t = 0; t < nbrTubeSteps; t++)
            {
                for (int r = 0; r < nbrRingSteps; r++)
                {
                    points[id++] = new Vector3((float)coord[r, t].X, (float)coord[r, t].Y, (float)coord[r, t].Z);
                }
            }

            int idx = 0;

            for (int t = 0; t < nbrTubeSegments; t++)
            {
                for (int r = 0; r < nbrRingSegments; r++)
                {
                    int idxTL = r + t * nbrRingSteps;
                    int idxBL = r + (t + 1) * nbrRingSteps;
                    int idxBR = r + 1 + (t + 1) * nbrRingSteps;
                    int idxTR = r + 1 + t * nbrRingSteps;
                    triangles.Add(new MyTriangle(
                        points[idxTL],
                        points[idxBL],
                        points[idxTR]
                        ));
                    triangles.Add(new MyTriangle(
                        points[idxBL],
                        points[idxBR],
                        points[idxTR]
                        ));


                }
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
