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
            Scene.Camera camera = new Scene.Camera(new Vector3(0, 10, 200), fov);
            scene = new Scene.Scene(camera);
            Shapes.IcoSphere fig = new Shapes.IcoSphere(3);
            fig.dx = 200;
            fig.dy = -50;
            fig.dz = 1;
            fig.scale_x = 100;
            fig.scale_y = 100;
            fig.scale_z = 100;
            fig.main_clr = Render.color(153, 153, 255);
            fig.select_clr = Render.color(204, 204, 255);

            Shapes.Box fig2 = new Shapes.Box();
            fig2.dx = 50;
            fig2.dy = -50;
            fig2.dz = 1;
            fig2.scale_x = 200;
            fig2.scale_y = 100;
            fig2.scale_z = 100;
            fig2.main_clr = Render.color(153, 153, 255);
            //fig2.select_clr = Render.color(204, 204, 255);

            Shapes.Toroid fig3 = new Shapes.Toroid();
            fig3.dx = -100;
            fig3.dy = -150;
            fig3.dz = 1;
            fig3.scale_x = 100;
            fig3.scale_y = 100;
            fig3.scale_z = 100;
            fig3.main_clr = Render.color(153, 153, 255);
            //fig3.select_clr = Render.color(204, 204, 255);

            Shapes.Conus fig4 = new Shapes.Conus();
            fig4.dx = -100;
            fig4.dy = 50;
            fig4.dz = 1;
            fig4.scale_x = 100;
            fig4.scale_y = 100;
            fig4.scale_z = 100;
            fig4.main_clr = Render.color(153, 153, 255);
            //fig4.select_clr = Render.color(204, 204, 255);

            Shapes.Cylinder fig5 = new Shapes.Cylinder();
            fig5.dx = -100;
            fig5.dy = -150;
            fig5.dz = 1;
            fig5.scale_x = 100;
            fig5.scale_y = 100;
            fig5.scale_z = 100;

            fig5.main_clr = Render.color(153, 153, 255);
            //fig5.select_clr = Render.color(204, 204, 255);

            //scene.shapes.Add(fig);
            //scene.shapes.Add(fig2);
            //scene.shapes.Add(fig3);
            //scene.shapes.Add(fig4);
            //scene.shapes.Add(fig5);

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
                var shape = scene.shapes[scene.selected_id];
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

        public void RefreshComboBox()
        {
            selecter.Items.Clear();
            selecter.Items.Add("-1 - None");
            for(int i = 0; i < scene.shapes.Count; i++)
            {
                selecter.Items.Add($"{i} - "+scene.shapes[i].type);
            }
            selecter.SelectedIndex = scene.selected_id+1;
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

                scene.shapes.Add(shape);
                RefreshComboBox();
                selecter.SelectedIndex = scene.shapes.Count;
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
            if (id != -1) 
                Debug.WriteLine($"({x}, {y}): {scene.shapes[id].type}");
            scene.selected_id = id;
            selecter.SelectedIndex = scene.selected_id + 1;
            if (scene.selected_id != -1)
                GenerateShapeView(scene.shapes[scene.selected_id]);
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
            else if (e.Key == Key.Space)
                dy = 1f * coef; 
            else if (e.Key == Key.LeftShift)
                dy = -1f * coef;
            else if (e.Key == Key.Delete)
            {
                if (scene.selected_id != -1)
                {
                    scene.shapes.RemoveAt(scene.selected_id);
                    scene.selected_id = -1;
                    RefreshComboBox();
                }
            }
            scene.camera.eye = new Vector3(scene.camera.eye.X + dx, scene.camera.eye.Y + dy, scene.camera.eye.Z+dz);
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }



        private void selecter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selecter.Items.Count > 0)
            {
                var a = selecter.SelectedItem.ToString();
                scene.selected_id = int.Parse(a.Split(" ")[0]);
                if (scene.selected_id != -1)
                    GenerateShapeView(scene.shapes[scene.selected_id]);
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
                scene.shapes[scene.selected_id].main_clr = Render.color(cp.SelectedColor.Value.R, cp.SelectedColor.Value.G, cp.SelectedColor.Value.B);
        }

        private void dx_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbdx.Text = dx_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.shapes[scene.selected_id];
                shape.dx = (float)dx_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void dy_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbdy.Text = dy_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.shapes[scene.selected_id];
                shape.dy = (float)dy_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void dz_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbdz.Text = dz_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.shapes[scene.selected_id];
                shape.dz = (float)dz_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void scx_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbsx.Text = scx_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.shapes[scene.selected_id];
                shape.scale_x = (float)scx_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void scy_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbsy.Text = scy_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.shapes[scene.selected_id];
                shape.scale_y = (float)scy_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void scz_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbsz.Text = scz_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.shapes[scene.selected_id];
                shape.scale_z = (float)scz_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void rx_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbrx.Text = rx_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.shapes[scene.selected_id];
                shape.rotation_x = (float)rx_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void ry_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbry.Text = ry_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.shapes[scene.selected_id];
                shape.rotation_y = (float)ry_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void rz_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbrz.Text = rz_slider.Value.ToString();
            if (scene.selected_id != -1)
            {
                var shape = scene.shapes[scene.selected_id];
                shape.rotation_z = (float)rz_slider.Value;
            }
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void is_lines_Click(object sender, RoutedEventArgs e)
        {
            SceneView.Source = BitmapToImageSource(scene.render(width, height, is_lines.IsChecked ?? false));
        }

        private void tbdx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(tbdx.Text!="")
                dx_slider.Value = float.Parse(tbdx.Text);
        }

        private void tbdy_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbdy.Text != "")
                dy_slider.Value = float.Parse(tbdy.Text);
        }

        private void tbdz_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbdz.Text != "")
                dz_slider.Value = float.Parse(tbdz.Text);
        }

        private void tbsx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbsx.Text != "")
                scx_slider.Value = float.Parse(tbsx.Text);
        }

        private void tbsy_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbsy.Text != "")
                scy_slider.Value = float.Parse(tbsy.Text);
        }

        private void tbsz_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbsz.Text != "")
                scz_slider.Value = float.Parse(tbsz.Text);
        }

        private void tbrx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbrx.Text != "")
                rx_slider.Value = float.Parse(tbrx.Text);
        }

        private void tbry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbry.Text != "")
                ry_slider.Value = float.Parse(tbry.Text);
        }

        private void tbrz_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbrz.Text != "")
                rz_slider.Value = float.Parse(tbrz.Text);
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
                    for (int i = 0; i < scene.shapes.Count; i++)
                    {
                        fs.WriteLine(scene.shapes[i].type);

                        fs.WriteLine(scene.shapes[i].dx);
                        fs.WriteLine(scene.shapes[i].dy);
                        fs.WriteLine(scene.shapes[i].dz);

                        fs.WriteLine(scene.shapes[i].scale_x);
                        fs.WriteLine(scene.shapes[i].scale_y);
                        fs.WriteLine(scene.shapes[i].scale_z);

                        fs.WriteLine(scene.shapes[i].rotation_x);
                        fs.WriteLine(scene.shapes[i].rotation_y);
                        fs.WriteLine(scene.shapes[i].rotation_z);
                        fs.WriteLine(scene.shapes[i].main_clr.R + " " + scene.shapes[i].main_clr.G + " " + scene.shapes[i].main_clr.B);
                    }
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
                scene.shapes.Clear();
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
                            scene.shapes.Add(shape);
                        }
                    }
                    RefreshComboBox();
                }
            }
        }
    }
}
