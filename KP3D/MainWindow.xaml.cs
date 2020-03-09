using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Ara3D;
using System.Diagnostics;
using System.Windows.Threading;
using KP3D.Scene;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;

namespace KP3D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        float fov = (float)Math.PI/1.6f;
        Scene.Scene scene;

        int width = 550;
        int height = 480;

        public MainWindow()
        {
            InitializeComponent();
            Scene.Camera camera = new Scene.Camera(new Vector3(0, 0, 0), fov);
            scene = new Scene.Scene(camera);

            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));

            var timer = new DispatcherTimer();

            timer.Tick += new EventHandler(timer_Tick);

            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);

            timer.Start();
            RefreshComboBox();
        }

        public void ApplyChanges(bool render = true) {
            
            if (scene.selected_id != -1)
            {
                var shape = scene.groups[scene.selected_gid].shapes[scene.selected_id];
                shape.dx = (float)dx_slider.Value;
                shape.dy = (float)dy_slider.Value;
                shape.dz = (float)dz_slider.Value;

                shape.scale_x = (float)scx_slider.Value;
                shape.scale_y = (float)scy_slider.Value;
                shape.scale_z = (float)scz_slider.Value;

                shape.rotation_x = (float)rx_slider.Value;
                shape.rotation_y = (float)ry_slider.Value;
                shape.rotation_z = (float)rz_slider.Value;
            }

            if(render)
                SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));

        }

        public void GenerateShapeView(Scene.Shape shape)
        {
            tbdx.Text = shape.dx.ToString();
            tbdy.Text = shape.dy.ToString();
            tbdz.Text = shape.dz.ToString();

            dx_slider.Value = shape.dx;
            dy_slider.Value = shape.dy;
            dz_slider.Value = shape.dz;

            tbsx.Text = shape.scale_x.ToString();
            tbsy.Text = shape.scale_y.ToString();
            tbsz.Text = shape.scale_z.ToString();

            scx_slider.Value = shape.scale_x;
            scy_slider.Value = shape.scale_y;
            scz_slider.Value = shape.scale_z;

            tbrx.Text = shape.rotation_x.ToString();
            tbry.Text = shape.rotation_y.ToString();
            tbrz.Text = shape.rotation_z.ToString();

            rx_slider.Value = shape.rotation_x;
            ry_slider.Value = shape.rotation_y;
            rz_slider.Value = shape.rotation_z;


            cp.SelectedColor = System.Windows.Media.Color.FromRgb(shape.main_clr.R, shape.main_clr.G, shape.main_clr.B);
        }

        public void RefreshGroupsCB()
        {
            group_selecter.Items.Clear();
            group_selecter.Items.Add("-1 - undefined");
            for (int i = 0; i < scene.groups.Count; i++)
            {
                group_selecter.Items.Add($"{i} - " + scene.groups[i].name);
            }
            group_selecter.SelectedIndex = scene.selected_gid + 1;
        }

        public void RefreshComboBox()
        {
            if (scene.selected_gid != -1)
            {
                selecter.Items.Clear();
                selecter.Items.Add("-1 - None");
                for (int i = 0; i < scene.groups[scene.selected_gid].shapes.Count; i++)
                {
                    selecter.Items.Add($"{i} - " + scene.groups[scene.selected_gid].shapes[i].type);
                }
                selecter.SelectedIndex = scene.selected_id + 1;
            }
        }

        public void AddShape(string ShapeName)
        {
            Scene.Shape shape = null;
            if(ShapeName == "Box")
            {
                shape = new Shapes.Box();
            }
            else if(ShapeName== "IcoSphere")
            {
                shape = new Shapes.IcoSphere(3);
            }
            else if (ShapeName == "Conus")
            {
               shape = new Shapes.Conus();
            }
            else if (ShapeName == "Cylinder")
            {
                shape = new Shapes.Cylinder();
            }
            else if(ShapeName == "Toroid")
            {
                shape = new Shapes.Toroid();
            }
            if (shape != null)
            {
                shape.dx = 0;
                shape.dy = 0;
                shape.dz = 0;

                shape.scale_x = 100;
                shape.scale_y = 100;
                shape.scale_z = 100;

                shape.rotation_x = 0;
                shape.rotation_y = 0;
                shape.rotation_z = 0;

                shape.main_clr = Render.color(255, 255, 255);
                if (scene.selected_gid != -1)
                {
                    scene.groups[scene.selected_gid].shapes.Add(shape);
                    RefreshComboBox();
                    selecter.SelectedIndex = scene.groups[scene.selected_gid].shapes.Count;
                }
            }

        }


        private void timer_Tick(object sender, EventArgs e)
        {
            if(is_rotating.IsChecked ?? true)
                scene.camera.yaw += 0.01f;
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void SceneView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
            int x = (int)e.GetPosition(SceneView).X;
            int y = (int)e.GetPosition(SceneView).Y;
            int id = scene.id_object_in_pixel[x + y * scene.width];
            int gid = scene.id_group_in_pixel[x + y * scene.width];
            if (id != -1 && gid != -1) 
                Debug.WriteLine($"({x}, {y}): {scene.groups[gid].shapes[id].type}");
            scene.selected_id = id;
            scene.selected_gid = gid;
            selecter.SelectedIndex = scene.selected_id + 1;
            group_selecter.SelectedIndex = scene.selected_gid + 1;
            if (scene.selected_id != -1)
                GenerateShapeView(scene.groups[scene.selected_gid].shapes[scene.selected_id]);
            if (scene.selected_gid != -1)
                LoadGroupInfo(scene.groups[scene.selected_gid]);

            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void SceneView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                scene.camera.fov += 0.1f;

            else if (e.Delta < 0)
                scene.camera.fov -= 0.1f;

            if (scene.camera.fov <= 0) 
                scene.camera.fov = 0.1f;

            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));

            

        }

        int px = 0;
        int py = 0;
        private void SceneView_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                int x = (int)e.GetPosition(this).X;
                int y = (int)e.GetPosition(this).Y;
                if(px == 0 && py == 0)
                {
                    px = x;
                    py = y;
                }
                int dx = x - px;
                int dy = y - py;

                //dy = 0;

                if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1) {
                    scene.camera.OnMouseMove(-dy, -dx);
                    scene.camera.center = Vector3.MinValue;
                    SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
                    //Debug.WriteLine(scene.camera.yaw);
                    //Debug.WriteLine(scene.camera.pitch);
                    //Debug.WriteLine(scene.camera.eye);
                    px = x;
                    py = y;

                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                
            }
            else
            {
                px = 0;
                py = 0;
            }
        }

        private void SceneView_KeyDown(object sender, KeyEventArgs e)
        {
            float dz = 0;
            float dy = 0;
            float dx = 0;
            float coef = 10;
            if (e.Key == Key.W)
                dz = -1f*coef;
            else if (e.Key == Key.S)
                dz = 1f * coef; 
            if (e.Key == Key.A)
                dx = -1f * coef; 
            else if (e.Key == Key.D)
                dx = 1f * coef; 
            else if (e.Key == Key.U)
                dy = 1f * coef; 
            else if (e.Key == Key.J)
                dy = -1f * coef;
            else if (e.Key == Key.Delete)
            {
                if (scene.selected_id != -1)
                {
                    scene.groups[scene.selected_gid].shapes.RemoveAt(scene.selected_id);
                    scene.selected_id = -1;
                    RefreshComboBox();
                }
            }
            scene.camera.eye = new Vector3(scene.camera.eye.X + dx, scene.camera.eye.Y + dy, scene.camera.eye.Z+dz);
            Debug.WriteLine(scene.camera.eye);
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void selecter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selecter.Items.Count > 0)
            {
                var a = selecter.SelectedItem.ToString();
                scene.selected_id = int.Parse(a.Split(" ")[0]);
                if (scene.selected_id != -1)
                    GenerateShapeView(scene.groups[scene.selected_gid].shapes[scene.selected_id]);
                else
                {
                    tbdx.Text = "0";
                    tbdy.Text = "0";
                    tbdz.Text = "0";

                    dx_slider.Value = 0;
                    dy_slider.Value = 0;
                    dz_slider.Value = 0;

                    tbsx.Text = "0";
                    tbsy.Text = "0";
                    tbsz.Text = "0";

                    scx_slider.Value = 0;
                    scy_slider.Value = 0;
                    scz_slider.Value = 0;

                    tbrx.Text = "0";
                    tbry.Text = "0";
                    tbrz.Text = "0";

                    rx_slider.Value = 0;
                    ry_slider.Value = 0;
                    rz_slider.Value = 0;
                }
                SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
            }
        }
        private void cp_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (scene.selected_id != -1)
                scene.groups[scene.selected_gid].shapes[scene.selected_id].main_clr = Render.color(cp.SelectedColor.Value.R, cp.SelectedColor.Value.G, cp.SelectedColor.Value.B);
        }
        private void is_lines_Click(object sender, RoutedEventArgs e)
        {
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void dx_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbdx.Text = dx_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.groups[scene.selected_gid].shapes[scene.selected_id];
                shape.dx = (float)dx_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void dy_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbdy.Text = dy_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.groups[scene.selected_gid].shapes[scene.selected_id];
                shape.dy = (float)dy_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void dz_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbdz.Text = dz_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.groups[scene.selected_gid].shapes[scene.selected_id];
                shape.dz = (float)dz_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void scx_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbsx.Text = scx_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.groups[scene.selected_gid].shapes[scene.selected_id];
                shape.scale_x = (float)scx_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void scy_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbsy.Text = scy_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.groups[scene.selected_gid].shapes[scene.selected_id];
                shape.scale_y = (float)scy_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void scz_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbsz.Text = scz_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.groups[scene.selected_gid].shapes[scene.selected_id];
                shape.scale_z = (float)scz_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void rx_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbrx.Text = rx_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.groups[scene.selected_gid].shapes[scene.selected_id];
                shape.rotation_x = (float)rx_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void ry_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbry.Text = ry_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.groups[scene.selected_gid].shapes[scene.selected_id];
                shape.rotation_y = (float)ry_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void rz_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbrz.Text = rz_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.groups[scene.selected_gid].shapes[scene.selected_id];
                shape.rotation_z = (float)rz_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }


        private void tbdx_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbdx.Text != "")
                    dx_slider.Value = float.Parse(tbdx.Text);
            }
            catch (Exception)
            {

            };
}
        private void tbdy_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { 
            if (tbdy.Text != "")
                dy_slider.Value = float.Parse(tbdy.Text);
            }
            catch (Exception)
            {

            };
        }
        private void tbdz_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbdz.Text != "")
                    dz_slider.Value = float.Parse(tbdz.Text);
            }
            catch (Exception) { 
            
            };
        }

        private void tbsx_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { 
            if (tbsx.Text != "")
                scx_slider.Value = float.Parse(tbsx.Text);
            }
            catch (Exception)
            {

            };
        }
        private void tbsy_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbsy.Text != "")
                    scy_slider.Value = float.Parse(tbsy.Text);
            }
            catch (Exception)
            {

            };
}
        private void tbsz_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { 
            if (tbsz.Text != "")
                scz_slider.Value = float.Parse(tbsz.Text);
            }
            catch (Exception)
            {

            };
        }

        private void tbrx_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { 
            if (tbrx.Text != "")
                rx_slider.Value = float.Parse(tbrx.Text);
            }
            catch (Exception)
            {

            };
        }
        private void tbry_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbry.Text != "")
                    ry_slider.Value = float.Parse(tbry.Text);
            }
            catch (Exception)
            {

            };
        }
        private void tbrz_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbrz.Text != "")
                    rz_slider.Value = float.Parse(tbrz.Text);
            }
            catch (Exception)
            {

            };
}


        private void dx_slider_ValueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbdx2.Text = dx_slider2.Value.ToString();
            if (scene.selected_gid != -1)
            {
                var shape = scene.groups[scene.selected_gid];
                shape.dx = (float)dx_slider2.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void dy_slider_ValueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbdy2.Text = dy_slider2.Value.ToString();
            if (scene.selected_gid != -1)
            {
                var shape = scene.groups[scene.selected_gid];
                shape.dy = (float)dy_slider2.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void dz_slider_ValueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbdz2.Text = dz_slider2.Value.ToString();
            if (scene.selected_gid != -1)
            {
                var shape = scene.groups[scene.selected_gid];
                shape.dz = (float)dz_slider2.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void scx_slider_ValueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbsx2.Text = scx_slider2.Value.ToString();
            if (scene.selected_gid != -1)
            {
                var shape = scene.groups[scene.selected_gid];
                shape.scale_x = (float)scx_slider2.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void scy_slider_ValueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbsy2.Text = scy_slider2.Value.ToString();
            if (scene.selected_gid != -1)
            {
                var shape = scene.groups[scene.selected_gid];
                shape.scale_y = (float)scy_slider2.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void scz_slider_ValueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbsz2.Text = scz_slider2.Value.ToString();
            if (scene.selected_gid != -1)
            {
                var shape = scene.groups[scene.selected_gid];
                shape.scale_z = (float)scz_slider2.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void rx_slider_ValueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbrx2.Text = rx_slider2.Value.ToString();
            if (scene.selected_gid != -1)
            {
                var shape = scene.groups[scene.selected_gid];
                shape.rotation_x = (float)rx_slider2.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void ry_slider_ValueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbry2.Text = ry_slider2.Value.ToString();
            if (scene.selected_gid != -1)
            {
                var shape = scene.groups[scene.selected_gid];
                shape.rotation_y = (float)ry_slider2.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        private void rz_slider_ValueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbrz2.Text = rz_slider2.Value.ToString();
            if (scene.selected_gid != -1)
            {
                var shape = scene.groups[scene.selected_gid];
                shape.rotation_z = (float)rz_slider2.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }


        private void tbdx_TextChanged2(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbdx2.Text != "")
                    dx_slider2.Value = float.Parse(tbdx2.Text);
            }
            catch (Exception)
            {

            };
        }
        private void tbdy_TextChanged2(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbdy2.Text != "")
                    dy_slider2.Value = float.Parse(tbdy2.Text);
            }
            catch (Exception)
            {

            };
        }
        private void tbdz_TextChanged2(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbdz2.Text != "")
                    dz_slider2.Value = float.Parse(tbdz2.Text);
            }
            catch (Exception)
            {

            };
        }

        private void tbsx_TextChanged2(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbsx2.Text != "")
                    scx_slider2.Value = float.Parse(tbsx2.Text);
            }
            catch (Exception)
            {

            };
        }
        private void tbsy_TextChanged2(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbsy2.Text != "")
                    scy_slider2.Value = float.Parse(tbsy2.Text);
            }
            catch (Exception)
            {

            };
        }
        private void tbsz_TextChanged2(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbsz2.Text != "")
                    scz_slider2.Value = float.Parse(tbsz2.Text);
            }
            catch (Exception)
            {

            };
        }

        private void tbrx_TextChanged2(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbrx2.Text != "")
                    rx_slider2.Value = float.Parse(tbrx2.Text);
            }
            catch (Exception)
            {

            };
        }
        private void tbry_TextChanged2(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbry2.Text != "")
                    ry_slider2.Value = float.Parse(tbry2.Text);
            }
            catch (Exception)
            {

            };
        }
        private void tbrz_TextChanged2(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbrz2.Text != "")
                    rz_slider2.Value = float.Parse(tbrz2.Text);
            }
            catch (Exception)
            {

            };
        }

        private void gname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (scene != null && scene.selected_gid != -1)
            {
                scene.groups[scene.selected_gid].name = gname.Text;
                RefreshGroupsCB();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddShape("IcoSphere");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AddShape("Box");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AddShape("Toroid");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AddShape("Cylinder");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            AddShape("Conus");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "text|*.txt";
            sfd.Title = "Save an scene";
            sfd.ShowDialog();
            if (sfd.FileName != "")
            {
                using (StreamWriter fs = new StreamWriter(sfd.FileName))
                {
                    fs.WriteLine(scene.camera.eye.X + " " + scene.camera.eye.Y + " " + scene.camera.eye.Z);
                    fs.WriteLine(scene.camera.fov);
                    fs.WriteLine(scene.camera.pitch);
                    fs.WriteLine(scene.camera.yaw);
                    //for (int i = 0; i < scene.shapes.Count; i++)
                    //{
                    //    fs.WriteLine(scene.shapes[i].type);

                    //    fs.WriteLine(scene.shapes[i].dx);
                    //    fs.WriteLine(scene.shapes[i].dy);
                    //    fs.WriteLine(scene.shapes[i].dz);

                    //    fs.WriteLine(scene.shapes[i].scale_x);
                    //    fs.WriteLine(scene.shapes[i].scale_y);
                    //    fs.WriteLine(scene.shapes[i].scale_z);

                    //    fs.WriteLine(scene.shapes[i].rotation_x);
                    //    fs.WriteLine(scene.shapes[i].rotation_y);
                    //    fs.WriteLine(scene.shapes[i].rotation_z);
                    //    fs.WriteLine(scene.shapes[i].main_clr.R + " " + scene.shapes[i].main_clr.G + " " + scene.shapes[i].main_clr.B);
                    //}
                }
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var lfd = new OpenFileDialog();
            lfd.Filter = "text|*.txt";
            lfd.Title = "Load an scene";
            lfd.ShowDialog();
            if (lfd.FileName != "")
            {
                //scene.shapes.Clear();
                using (StreamReader fs = new StreamReader(lfd.FileName))
                {
                    string line;

                    var eye = fs.ReadLine().Split(" ");
                    scene.camera.eye = new Vector3(float.Parse(eye[0]), float.Parse(eye[1]), float.Parse(eye[2]));
                    scene.camera.fov = float.Parse(fs.ReadLine());
                    scene.camera.pitch = float.Parse(fs.ReadLine());
                    scene.camera.yaw = float.Parse(fs.ReadLine());

                    while ((line = fs.ReadLine()) != null)
                    {
                        if (line != "")
                        {
                            Scene.Shape shape = new Scene.Shape();
                            if (line == "Box")
                            {
                                shape = new Shapes.Box();
                            }
                            else if (line == "IcoSphere")
                            {
                                shape = new Shapes.IcoSphere();
                            }
                            else if (line == "Toroid")
                            {
                                shape = new Shapes.Toroid();
                            }
                            else if (line == "Conus")
                            {
                                shape = new Shapes.Conus();
                            }
                            else if (line == "Cylinder")
                            {
                                shape = new Shapes.Cylinder();
                            }

                            shape.dx = float.Parse(fs.ReadLine());
                            shape.dy = float.Parse(fs.ReadLine());
                            shape.dz = float.Parse(fs.ReadLine());

                            shape.scale_x = float.Parse(fs.ReadLine());
                            shape.scale_y = float.Parse(fs.ReadLine());
                            shape.scale_z = float.Parse(fs.ReadLine());

                            shape.rotation_x = float.Parse(fs.ReadLine());
                            shape.rotation_y = float.Parse(fs.ReadLine());
                            shape.rotation_z = float.Parse(fs.ReadLine());

                            var clr = fs.ReadLine().Split(" ");
                            shape.main_clr = Render.color(int.Parse(clr[0]), int.Parse(clr[1]), int.Parse(clr[2]));
                            //scene.shapes.Add(shape);
                        }
                    }
                    RefreshComboBox();
                }
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            float feye_x = float.Parse(eye_x.Text);
            float feye_y = float.Parse(eye_y.Text);
            float feye_z = float.Parse(eye_z.Text);
            scene.camera.eye = new Vector3(feye_x, feye_y, feye_z);

            float fdir_x = float.Parse(dir_x.Text);
            float fdir_y = float.Parse(dir_y.Text);
            float fdir_z = float.Parse(dir_z.Text);

            scene.camera.center = new Vector3(fdir_x, fdir_y, fdir_z);
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }
        
        //add group
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            scene.groups.Add(new GroupShapes());
            RefreshGroupsCB();
            group_selecter.SelectedIndex = scene.groups.Count;
            scene.selected_gid = scene.groups.Count - 1;
        }

        void LoadGroupInfo(GroupShapes shape)
        {
            gname.Text = shape.name.ToString();
            tbdx2.Text = shape.dx.ToString();
            tbdy2.Text = shape.dy.ToString();
            tbdz2.Text = shape.dz.ToString();

            dx_slider2.Value = shape.dx;
            dy_slider2.Value = shape.dy;
            dz_slider2.Value = shape.dz;

            tbsx2.Text = shape.scale_x.ToString();
            tbsy2.Text = shape.scale_y.ToString();
            tbsz2.Text = shape.scale_z.ToString();

            scx_slider2.Value = shape.scale_x;
            scy_slider2.Value = shape.scale_y;
            scz_slider2.Value = shape.scale_z;

            tbrx2.Text = shape.rotation_x.ToString();
            tbry2.Text = shape.rotation_y.ToString();
            tbrz2.Text = shape.rotation_z.ToString();

            rx_slider2.Value = shape.rotation_x;
            ry_slider2.Value = shape.rotation_y;
            rz_slider2.Value = shape.rotation_z;
        }

        private void group_selecter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (group_selecter.Items.Count > 0)
            {
                var a = group_selecter.SelectedItem.ToString();
                scene.selected_gid = int.Parse(a.Split(" ")[0]);

                if (scene.selected_gid != -1)
                {
                    LoadGroupInfo(scene.groups[scene.selected_gid]);
                    RefreshComboBox();
                }
                else
                {
                    gname.Text = "undefined";
                    tbdx2.Text = "0";
                    tbdy2.Text = "0";
                    tbdz2.Text = "0";

                    dx_slider2.Value = 0;
                    dy_slider2.Value = 0;
                    dz_slider2.Value = 0;

                    tbsx2.Text = "0";
                    tbsy2.Text = "0";
                    tbsz2.Text = "0";

                    scx_slider2.Value = 0;
                    scy_slider2.Value = 0;
                    scz_slider2.Value = 0;

                    tbrx2.Text = "0";
                    tbry2.Text = "0";
                    tbrz2.Text = "0";

                    rx_slider2.Value = 0;
                    ry_slider2.Value = 0;
                    rz_slider2.Value = 0;
                }
                SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (scene.selected_gid != -1)
            {
                scene.groups.RemoveAt(scene.selected_gid);
                scene.selected_gid = -1;
                scene.selected_id = -1;
                RefreshComboBox();
                RefreshGroupsCB();
            }
        }

        private void SaveGroup_Click(object sender, RoutedEventArgs e)
        {
            if (scene.selected_gid != -1)
            {
                var sfd = new SaveFileDialog();
                sfd.Filter = "text|*.txt";
                sfd.Title = "Save an object";
                sfd.ShowDialog();
                if (sfd.FileName != "")
                {
                    using (StreamWriter fs = new StreamWriter(sfd.FileName))
                    {
                        fs.WriteLine(scene.groups[scene.selected_gid].name);
                        
                        for (int i = 0; i < scene.groups[scene.selected_gid].shapes.Count; i++)
                        {
                            fs.WriteLine(scene.groups[scene.selected_gid].shapes[i].type);

                            fs.WriteLine(scene.groups[scene.selected_gid].shapes[i].dx);
                            fs.WriteLine(scene.groups[scene.selected_gid].shapes[i].dy);
                            fs.WriteLine(scene.groups[scene.selected_gid].shapes[i].dz);

                            fs.WriteLine(scene.groups[scene.selected_gid].shapes[i].scale_x);
                            fs.WriteLine(scene.groups[scene.selected_gid].shapes[i].scale_y);
                            fs.WriteLine(scene.groups[scene.selected_gid].shapes[i].scale_z);

                            fs.WriteLine(scene.groups[scene.selected_gid].shapes[i].rotation_x);
                            fs.WriteLine(scene.groups[scene.selected_gid].shapes[i].rotation_y);
                            fs.WriteLine(scene.groups[scene.selected_gid].shapes[i].rotation_z);
                            fs.WriteLine(
                                scene.groups[scene.selected_gid].shapes[i].main_clr.R
                                + " " 
                                + scene.groups[scene.selected_gid].shapes[i].main_clr.G 
                                + " " 
                                + scene.groups[scene.selected_gid].shapes[i].main_clr.B
                                );
                        }
                    }
                }
            }
        }

        private void LoadGroup_Click(object sender, RoutedEventArgs e)
        {
            
            var lfd = new OpenFileDialog();
            lfd.Filter = "text|*.txt";
            lfd.Title = "Load an scene";
            lfd.ShowDialog();
            if (lfd.FileName != "")
            {
                scene.groups.Add(new GroupShapes());
                scene.selected_gid = scene.groups.Count - 1;
                using (StreamReader fs = new StreamReader(lfd.FileName))
                {
                    string line;
                    scene.groups[scene.selected_gid].name = fs.ReadLine();
                    
                    while ((line = fs.ReadLine()) != null)
                    {
                        if (line != "")
                        {
                            Scene.Shape shape = new Scene.Shape();
                            if (line == "Box")
                            {
                                shape = new Shapes.Box();
                            }
                            else if (line == "IcoSphere")
                            {
                                shape = new Shapes.IcoSphere();
                            }
                            else if (line == "Toroid")
                            {
                                shape = new Shapes.Toroid();
                            }
                            else if (line == "Conus")
                            {
                                shape = new Shapes.Conus();
                            }
                            else if (line == "Cylinder")
                            {
                                shape = new Shapes.Cylinder();
                            }

                            shape.dx = float.Parse(fs.ReadLine());
                            shape.dy = float.Parse(fs.ReadLine());
                            shape.dz = float.Parse(fs.ReadLine());

                            shape.scale_x = float.Parse(fs.ReadLine());
                            shape.scale_y = float.Parse(fs.ReadLine());
                            shape.scale_z = float.Parse(fs.ReadLine());

                            shape.rotation_x = float.Parse(fs.ReadLine());
                            shape.rotation_y = float.Parse(fs.ReadLine());
                            shape.rotation_z = float.Parse(fs.ReadLine());

                            var clr = fs.ReadLine().Split(" ");
                            shape.main_clr = Render.color(int.Parse(clr[0]), int.Parse(clr[1]), int.Parse(clr[2]));
                            scene.groups[scene.selected_gid].shapes.Add(shape);
                        }
                    }
                }
                RefreshGroupsCB();
                group_selecter.SelectedIndex = scene.groups.Count;        
            }
        }

        
    }
}
