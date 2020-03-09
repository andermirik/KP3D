using Ara3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KP3D.MyMath
{
    class MyMatrix4x4
    {
        public float[,] points = new float[4, 4];

        public MyMatrix4x4()
        {
            points[0, 0] = 1;
            points[1, 1] = 1;
            points[2, 2] = 1;
            points[3, 3] = 1;
        }

        public static MyMatrix4x4 Identity()
        {
            return new MyMatrix4x4();
        }
        public static MyMatrix4x4 CreateScale(float x, float y, float z)
        {
            var temp = new MyMatrix4x4();
            temp.points[0, 0] = x;
            temp.points[1, 1] = y;
            temp.points[2, 2] = z;
            return temp;
        }

        public static MyMatrix4x4 CreateTranslation(float dx, float dy, float dz)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();
            temp.points[0, 3] = dx;
            temp.points[1, 3] = dy;
            temp.points[2, 3] = dz;
            return temp;
        }

        public static MyMatrix4x4 CreateRotationX(float A)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();
            temp.points[1, 1] = (float)Math.Cos(A);
            temp.points[1, 2] = (float)Math.Sin(A);
            temp.points[2, 2] = (float)Math.Cos(A);
            temp.points[2, 1] = -(float)Math.Sin(A);
            return temp;
        }

        public static MyMatrix4x4 CreateRotationY(float A)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();
            temp.points[0, 0] = (float)Math.Cos(A);
            temp.points[2, 0] = -(float)Math.Sin(A);
            temp.points[0, 2] = (float)Math.Sin(A);
            temp.points[2, 2] = (float)Math.Cos(A);
            return temp;
        }

        public static MyMatrix4x4 CreateRotationZ(float A)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();
            temp.points[0, 0] = (float)Math.Cos(A);
            temp.points[1, 0] = -(float)Math.Sin(A);
            temp.points[0, 1] = (float)Math.Sin(A);
            temp.points[1, 1] = (float)Math.Cos(A);
            return temp;
        }

        public static MyMatrix4x4 CreateFPSLookAt(Vector3 eye, float pitch, float yaw)
        {
            float cosPitch = (float)Math.Cos(pitch);
            float sinPitch = (float)Math.Sin(pitch);
            float cosYaw = (float)Math.Cos(yaw);
            float sinYaw = (float)Math.Sin(yaw);

            Vector3 xaxis = new Vector3(cosYaw, 0, -sinYaw);
            Vector3 yaxis = new Vector3(sinYaw * sinPitch, cosPitch, cosYaw * sinPitch);
            Vector3 zaxis = new Vector3(sinYaw * cosPitch, -sinPitch, cosPitch * cosYaw);

            MyMatrix4x4 view = new MyMatrix4x4();
            view.points[0, 0] = xaxis.X;
            view.points[1, 0] = yaxis.X;
            view.points[2, 0] = zaxis.X;

            view.points[0, 1] = xaxis.Y;
            view.points[1, 1] = yaxis.Y;
            view.points[2, 1] = zaxis.Y;

            view.points[0, 2] = xaxis.Z;
            view.points[1, 2] = yaxis.Z;
            view.points[2, 2] = zaxis.Z;

            view.points[0, 3] = -xaxis.Dot(eye);
            view.points[1, 3] = -yaxis.Dot(eye);
            view.points[2, 3] = -zaxis.Dot(eye);
            return view;// * MyMath.MyMatrix4x4.CreateTranslation(eye.X, eye.Y, eye.Z);
        }

        public static MyMatrix4x4 CreateLookAt(Vector3 eye, Vector3 center, Vector3 up)
        {
            Vector3 zaxis = (eye - center).Normalize();    // The "forward" vector.
            Vector3 xaxis = up.Cross(zaxis).Normalize();   // The "right" vector.
            Vector3 yaxis = zaxis.Cross(xaxis);            // The "up" vector.
            MyMatrix4x4 view = new MyMatrix4x4();
            view.points[0, 0] = xaxis.X;
            view.points[0, 1] = xaxis.Y;
            view.points[0, 2] = xaxis.Z;
            view.points[0, 3] = xaxis.Dot(eye);

            view.points[1, 0] = yaxis.X;
            view.points[1, 1] = yaxis.Y;
            view.points[1, 2] = yaxis.Z;
            view.points[1, 3] = yaxis.Dot(eye);

            view.points[2, 0] = zaxis.X;
            view.points[2, 1] = zaxis.Y;
            view.points[2, 2] = zaxis.Z;
            view.points[2, 3] = zaxis.Dot(eye);
            return view;
        }

        public static MyMatrix4x4 CreateProjection(float left, float right, float bottom, float top, float near, float far)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();
            temp.points[0,0] = 2 * near / (right - left);
            temp.points[1,1] = 2 * near / (top - bottom);
            temp.points[2,2] = -(far + near) / (far - near);
            temp.points[2,3] = -1;
            temp.points[3,2] = -2 * far * near / (far - near);
            temp.points[2,0] = (right + left) / (right - left);
            temp.points[2,1] = (top + bottom) / (top - bottom);
            temp.points[3,3] = 0;
            
            return temp;
        }

        public static MyMatrix4x4 CreateProjectionFOV(float A, float B, float C, float D, float E)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();

            temp.points[0, 0] = A;
            temp.points[1, 1] = B;

            temp.points[2, 2] = C;
            temp.points[3, 2] = D;
            temp.points[2, 3] = E;
            temp.points[3, 3] = 0;

            return temp;
        }

        public static MyMatrix4x4 CreateProjectionFOV(float fov, float aspect, float near, float far)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();

            temp.points[0, 0] = 1 / (float)Math.Tan(fov * 0.5f) / aspect;
            temp.points[1, 1] = 1 / (float)Math.Tan(fov * 0.5f);

            temp.points[2, 2] = (far) / (far - near);
            temp.points[3, 2] = -1f;
            temp.points[2, 3] = -near * far / (far - near);
            temp.points[3, 3] = 0;



            //var a = Matrix4x4.CreatePerspectiveFieldOfView(fov, aspect, near, far);
            //temp.points = new float[,] { { a.M11, a.M12, a.M13, a.M14 }, { a.M21, a.M22, a.M23, a.M24 }, { a.M31, a.M32, a.M33, a.M34 }, { a.M41, a.M42, a.M43, a.M44 } };
            //temp.points = new float[,] { { a.M11, a.M21, a.M31, a.M41 }, { a.M12, a.M22, a.M32, a.M42 }, { a.M13, a.M23, a.M33, a.M43 }, { a.M14, a.M24, a.M34, a.M44 } };

            return temp;
        }

        public static MyMatrix4x4 operator *(MyMatrix4x4 A, MyMatrix4x4 B)
        {
            MyMatrix4x4 C = new MyMatrix4x4();

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    C.points[i, j] = 0;

                    for (var k = 0; k < 4; k++)
                    {
                        C.points[i, j] += A.points[i, k] * B.points[k, j];
                    }
                }
            }

            return C;
        }

        public static MyMatrix4x4 FromPoint(Vector3 vector)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();
            temp.points[0, 0] = vector.X;
            temp.points[1, 1] = vector.Y;
            temp.points[2, 2] = vector.Z;
            return temp;
        }

        public static Vector3 ToPoint(MyMatrix4x4 m)
        {
            return new Vector3(m.points[0, 0], m.points[1, 1], m.points[2, 2]);
        }

        public static MyMatrix4x4 inverse(MyMatrix4x4 matrix)
        {
            return null;
        }
    }
    class MyMatrix4x1
    {
        public float[,] points = new float[4, 1];

        public MyMatrix4x1()
        {
            points[0, 0] = 1;
            points[1, 0] = 1;
            points[2, 0] = 1;
            points[3, 0] = 1;
        }

        public static MyMatrix4x1 operator *(MyMatrix4x4 A, MyMatrix4x1 B)
        {
            MyMatrix4x1 C = new MyMatrix4x1();

            for (var i = 0; i < 4; i++)
            {

                C.points[i, 0] = 0;

                for (var k = 0; k < 4; k++)
                {
                    C.points[i, 0] += A.points[i, k] * B.points[k, 0];
                }
            }

            return C;
        }

        public static MyMatrix4x1 FromPoint4x1(Vector3 vector)
        {
            MyMatrix4x1 temp = new MyMatrix4x1();
            temp.points[0, 0] = vector.X;
            temp.points[1, 0] = vector.Y;
            temp.points[2, 0] = vector.Z;
            return temp;
        }
        public static Vector3 ToPoint(MyMatrix4x1 m)
        {
            float x = m.points[0, 0];
            float y = m.points[1, 0];
            float z = m.points[2, 0];

            return new Vector3(x, y, z);
        }

        public static Vector3 ToPoint(MyMatrix4x1 m, float A, float B, float C, float D, float E, int width, int height)
        {
            float x = m.points[0, 0];
            float y = m.points[1, 0];
            float z = m.points[2, 0];
            float w = m.points[3, 0];

            //x = (A * x) / (D * z);
            //y = (B * y) / (D * z);


            //x = (x) * width;
            //y = (y) * height;


            //z = (C * z + E * w * E) / (D * z);
            //z = MathF.Log2(MathF.Max(1e-6f, 1.0f + w)) * 2.0f / MathF.Log2(100f + 1.0f) - 1.0f;
            //z = MathF.Log2(1 * z + 1) / MathF.Log2(1 * 100f + 1) * w;
            //Debug.WriteLine(z);

            return new Vector3(x, y, z);
        }

        public static Vector3 ToPointZisW(MyMatrix4x1 m)
        {
            float x = m.points[0, 0];
            float y = m.points[1, 0];
            float z = m.points[2, 0];

            return new Vector3(x, y, z / m.points[3, 0]);
        }
    }

}
