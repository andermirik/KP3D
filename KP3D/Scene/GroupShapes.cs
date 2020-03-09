using System;
using System.Collections.Generic;
using System.Text;

namespace KP3D.Scene
{
    class GroupShapes
    {
        public GroupShapes()
        {
            shapes = new List<Shape>();
        }

        public string name = "undefined";

        public float dx = 0;
        public float dy = 0;
        public float dz = 0;

        public float rotation_x = 0;
        public float rotation_y = 0;
        public float rotation_z = 0;

        public float scale_x = 1;
        public float scale_y = 1;
        public float scale_z = 1;

        public List<Shape> shapes;
    }
}
