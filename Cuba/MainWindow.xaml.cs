using System;
using System.Collections.Generic;
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
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Cuba
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer t = new Timer();
        Cube c;
        Cube c2;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            glControl.MakeCurrent();

            t.Interval = 16; //ms
            t.Tick += (o, args) =>
            {
                Render();
            };
            t.Enabled = true;

            c = new Cube(new Vector3(1, 0, 0), new Vector4(1.0f, 0.25f, 0.25f, 1.0f));
            c2 = new Cube(new Vector3(-1, 0, 0), new Vector4(0.25f, 1.0f, 0.25f, 1.0f));

            UBOManager.LinkUBO(c);
            UBOManager.LinkUBO(c2);

            InitMatrices();
        }

        private void InitMatrices()
        {
            // Create projection and view matrices
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(45.0f * ((float)Math.PI / 180.0f), 4 / 3.0f, 0.01f, 1000000.0f);
            Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, -10), Vector3.UnitZ, Vector3.UnitY);

            UBOManager.UpdateProjectionMatrix(projection);
            UBOManager.UpdateViewMatrix(view);
        }

        private void Render()
        {
            GL.ClearColor(0.0f, 0.25f, 0.5f, 1.0f);

            c.Render();
            c2.Render();

            glControl.SwapBuffers();
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }
    }
}
