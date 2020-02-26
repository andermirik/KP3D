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

namespace KP3D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        float fov = 180/2;
        Scene.Scene scene;
        Random rnd = new Random();
        public MainWindow()
        {
            InitializeComponent();
            Scene.Scene.Camera camera = new Scene.Scene.Camera(new Vector3(0, 0, 100), new Vector3(0, 0, 0), fov);
            scene = new Scene.Scene(camera);
            Shapes.IcoSphere fig = new Shapes.IcoSphere(2);
            fig.dx = 300;
            fig.dy = 200;
            fig.dz = 50;
            fig.scale_x = 100;
            fig.scale_y = 100;
            fig.scale_z = 100;

            Shapes.Box fig2 = new Shapes.Box();
            fig2.dx = 100;
            fig2.dy = 100;
            fig2.dz = 0;
            fig2.scale_x = 100;
            fig2.scale_y = 100;
            fig2.scale_z = 100;
            //fig2.rotation_x = 3.14f / 3;
            //scene.shapes.Add(fig);
            scene.shapes.Add(fig2);

        }

        
        public void render(int width, int height, System.Windows.Controls.Image image)
        {
            //image.Source = BitmapToImageSource(bm);
            
           
            SceneView.Source = BitmapToImageSource(scene.render(580, 440));
        }


        private void SceneView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SceneView.Source = BitmapToImageSource(scene.render(580, 440));
        }

        private void FOV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //render(400, 400, SceneView);
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
                scene.camera.fov += 1f;

            else if (e.Delta < 0)
                scene.camera.fov -= 1f;
            SceneView.Source = BitmapToImageSource(scene.render(580, 440));
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

                dy = 0;

                if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1) {
                    scene.camera.OnMouseMove(-dx, dy);
                    scene.camera.gen_eye(100);
                    SceneView.Source = BitmapToImageSource(scene.render(580, 440));
                    Debug.WriteLine(scene.camera.latitude);
                    Debug.WriteLine(scene.camera.longitude);
                    Debug.WriteLine(scene.camera.center);
                    px = x;
                    py = y;
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                int x = (int)e.GetPosition(this).X;
                int y = (int)e.GetPosition(this).Y;
                if (px == 0 && py == 0)
                {
                    px = x;
                    py = y;
                }
                int dx = x - px;
                int dy = y - py;

                dx = 0;

                if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
                {
                    scene.camera.OnMouseMove(-dx, -dy);
                    scene.camera.gen_eye(100);
                    SceneView.Source = BitmapToImageSource(scene.render(580, 440));
                    Debug.WriteLine(scene.camera.latitude);
                    Debug.WriteLine(scene.camera.longitude);
                    Debug.WriteLine(scene.camera.center);
                    px = x;
                    py = y;
                }
            }
            else
            {
                px = 0;
                py = 0;
            }
        }

        private void latitude_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scene.camera.latitude = (float)latitude.Value;
            scene.camera.gen_eye(100);
            //scene.camera.gen_center(100);
            SceneView.Source = BitmapToImageSource(scene.render(580, 440));
        }

        private void longitude_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scene.camera.longitude = (float)longitude.Value;
            scene.camera.gen_eye(100);
            //scene.camera.gen_center(100);
            SceneView.Source = BitmapToImageSource(scene.render(580, 440));
        }
    }
}
