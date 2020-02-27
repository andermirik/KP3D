using Ara3D;
using System;
using System.Collections.Generic;
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
            temp.points[2, 1] = (float)Math.Sin(A);
            temp.points[2, 2] = (float)Math.Cos(A);
            temp.points[1, 2] = -(float)Math.Sin(A);
            return temp;
        }

        public static MyMatrix4x4 CreateRotationY(float A)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();
            temp.points[0, 0] = (float)Math.Cos(A);
            temp.points[0, 2] = -(float)Math.Sin(A);
            temp.points[2, 0] = (float)Math.Sin(A);
            temp.points[2, 2] = (float)Math.Cos(A);
            return temp;
        }

        public static MyMatrix4x4 CreateRotationZ(float A)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();
            temp.points[0, 0] = (float)Math.Cos(A);
            temp.points[0, 1] = -(float)Math.Sin(A);
            temp.points[1, 0] = (float)Math.Sin(A);
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
            view.points[1, 0] = xaxis.Y;
            view.points[2, 0] = xaxis.Z;
            view.points[3, 0] = xaxis.Dot(-eye);

            view.points[0, 1] = yaxis.X;
            view.points[1, 1] = yaxis.Y;
            view.points[2, 1] = yaxis.Z;
            view.points[3, 1] = yaxis.Dot(-eye);

            view.points[0, 2] = zaxis.X;
            view.points[1, 2] = zaxis.Y;
            view.points[2, 2] = zaxis.Z;
            view.points[3, 2] = zaxis.Dot(-eye);
            return view;
        }

        public static MyMatrix4x4 CreateJRALookAt(Vector3 eye, Vector3 center, Vector3 up)
        {
            //MyMatrix4x4 matrixInverted = MyMatrix4x4.CreateTranslation(-eye.X, -eye.Y, -eye.Z);

            //var w = new Vector3(center.X - eye.X, center.Y - eye.Y, center.Z - center.Z);

            //var u = up.Cross(w);
            //up = u.Cross(w);

            //u = u.Normalize();
            //up = up.Normalize();
            //w = w.Normalize();

            var camMatrix = new MyMatrix4x4();

            //camMatrix.points[0, 0] = u.X;
            //camMatrix.points[0, 1] = u.Y;
            //camMatrix.points[0, 2] = u.Z;

            //camMatrix.points[1, 0] = up.X;
            //camMatrix.points[1, 1] = up.Y;
            //camMatrix.points[1, 2] = up.Z;

            //camMatrix.points[2, 0] = w.X;
            //camMatrix.points[2, 1] = w.Y;
            //camMatrix.points[2, 2] = w.Z;

            //camMatrix = camMatrix * matrixInverted;

            //return camMatrix;
            var a = Matrix4x4.CreateLookAt(eye, center, up);
            camMatrix.points = new float[,] { { a.M11, a.M12, a.M13, a.M14 }, { a.M21, a.M22, a.M23, a.M24 }, { a.M31, a.M32, a.M33, a.M44 }, { a.M41, a.M42, a.M43, a.M44 } };
            return camMatrix;
        }

        public static MyMatrix4x4 CreateLookAt(Vector3 eye, Vector3 center, Vector3 up)
        {
            Vector3 zaxis = (eye - center).Normalize();    // The "forward" vector.
            Vector3 xaxis = up.Cross(zaxis).Normalize();   // The "right" vector.
            Vector3 yaxis = zaxis.Cross(xaxis);            // The "up" vector.
            MyMatrix4x4 view = new MyMatrix4x4();
            view.points[0, 0] = xaxis.X;
            view.points[1, 0] = xaxis.Y;
            view.points[2, 0] = xaxis.Z;
            view.points[3, 0] = -xaxis.Dot(eye);

            view.points[0, 1] = yaxis.X;
            view.points[1, 1] = yaxis.Y;
            view.points[2, 1] = yaxis.Z;
            view.points[3, 1] = -yaxis.Dot(eye);

            view.points[0, 2] = zaxis.X;
            view.points[1, 2] = zaxis.Y;
            view.points[2, 2] = zaxis.Z;
            view.points[3, 2] = -zaxis.Dot(eye);
            return view;
        }

        // t0 is a vector which represents the distance to move the camera away
        //    from the object to ensure the object is in view.
        // r  is a rotation quaternion which rotates the camera 
        //    around the object being observed.
        // t1 is an optional vector which represents the position of the object in the world.

        MyMatrix4x4 CreateArcballView(Vector3 t0, float anglex, float angley, float anglez, Vector3 t1)
        {
            MyMatrix4x4 T0 = CreateTranslation(t0.X, t0.Y, t0.Z); // Translation away from object.
            MyMatrix4x4 R = CreateRotationX(anglex) * CreateRotationY(angley) * CreateRotationZ(anglez);     // Rotate around object.
            MyMatrix4x4 T1 = CreateTranslation(t1.X, t1.Y, t1.Z); // Translate to center of object.

            //MyMatrix4x4 viewMatrix = (T1 * R * T0).;

            // return viewMatrix;
            return null;
        }

        public static MyMatrix4x4 CreateProjectionFOV(float angleOfView, float aspect, float near, float far)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();

            float frustumDepth = far - near;
            float oneOverDepth = 1 / frustumDepth;

            temp.points[1, 1] = 1 / (float)Math.Tan(0.5f * angleOfView);
            temp.points[0, 0] = temp.points[1, 1] / aspect;
            temp.points[2, 2] = far * oneOverDepth;
            temp.points[3, 2] = (-far * near) * oneOverDepth;
            temp.points[2, 3] = 1;
            temp.points[3, 3] = 0;

            return temp;
        }

        public static MyMatrix4x4 CreateViewPort(int x, int y, int w, int h, int depth)
        {
            MyMatrix4x4 m = MyMatrix4x4.Identity();
            m.points[0, 3] = x + w / 2.0f;
            m.points[1, 3] = y + h / 2.0f;
            m.points[2, 3] = depth / 2.0f;

            m.points[0, 0] = w / 2.0f;
            m.points[1, 1] = h / 2.0f;
            m.points[2, 2] = depth / 2.0f;
            return m;
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
            //var a = new Matrix4x4();
            //a = Matrix4x4.FromRows();


            //var b = new Matrix4x4();
            //Matrix4x4.Invert(a, out b);

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
                for (var j = 0; j < 1; j++)
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
            return new Vector3(m.points[0, 0], m.points[1, 0], m.points[2, 0]);
        }
    }

}
