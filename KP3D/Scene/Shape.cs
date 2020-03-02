using KP3D.Shapes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KP3D.Scene
{
    public class Shape
    {
        public string type;

        public float dx = 0;
        public float dy = 0;
        public float dz = 0;

        public float rotation_x = 0;
        public float rotation_y = 0;
        public float rotation_z = 0;

        public float scale_x = 10;
        public float scale_y = 10;
        public float scale_z = 10;

        public System.Drawing.Color main_clr;
        public System.Drawing.Color select_clr = Render.color(204, 204, 255);

        public List<MyTriangle> triangles;
    }
}
