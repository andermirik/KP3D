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

        public static MyMatrix4x4 CreateLookAt(Vector3 eye, Vector3 center, Vector3 up)
        {
            Vector3 z = (eye - center).Normalize();
            Vector3 x = up.Cross(z).Normalize();
            Vector3 y = z.Cross(x).Normalize();
            MyMatrix4x4 Minv = MyMatrix4x4.Identity();
            MyMatrix4x4 Tr = MyMatrix4x4.Identity();

            Minv.points[0, 0] = x.X;
            Minv.points[1, 0] = y.X;
            Minv.points[2, 0] = z.X;
            Tr.points[0, 3] = -center.X;

            Minv.points[0, 1] = x.Y;
            Minv.points[1, 1] = y.Y;
            Minv.points[2, 1] = z.Y;
            Tr.points[1, 3] = -center.Y;

            Minv.points[0, 2] = x.Z;
            Minv.points[1, 2] = y.Z;
            Minv.points[2, 2] = z.Z;
            Tr.points[2, 3] = -center.Z;

            return Minv * Tr;

            //MyMatrix4x4 V = new MyMatrix4x4();
            //Vector3 tmp = new Vector3(0, 1, 0);

            //Vector3 forward = (eye - center).Normalize();
            //Vector3 right = tmp.Normalize().Cross(forward);
            //up = forward.Cross(right);

            //V.points[0, 0] = right.X;
            //V.points[0, 1] = right.Y;
            //V.points[0, 2] = right.Z;
            //V.points[1, 0] = up.X;
            //V.points[1, 1] = up.Y;
            //V.points[1, 2] = up.Z;
            //V.points[2, 0] = forward.X;
            //V.points[2, 1] = forward.Y;
            //V.points[2, 2] = forward.Z;

            //V.points[3, 0] = eye.X;
            //V.points[3, 1] = eye.Y;
            //V.points[3, 2] = eye.Z;

            //return V;
        }

        public static MyMatrix4x4 CreateProjectionFOV(float angleOfView, float near, float far)
        {
            MyMatrix4x4 temp = new MyMatrix4x4();
            float scale = 1 / (float)Math.Tan(angleOfView * 0.5 * Math.PI / 180);
            temp.points[0, 0] = scale; // scale the x coordinates of the projected point 
            temp.points[1, 1] = scale; // scale the y coordinates of the projected point 
            temp.points[2, 2] = -far / (far - near); // used to remap z to [0,1] 
            temp.points[3, 2] = -far * near / (far - near); // used to remap z [0,1] 
            temp.points[2, 3] = -1; // set w = -z 
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

        public static MyMatrix4x4 operator*(MyMatrix4x4 A, MyMatrix4x4 B)
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
