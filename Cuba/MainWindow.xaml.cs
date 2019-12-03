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
        Camera cam = new Camera();
        Timer t = new Timer();

        List<Cube> Cubes = new List<Cube>();
        private int UBOID;

        public MainWindow()
        {
            InitializeComponent();
            Cubes = new List<Cube>();
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            glControl.MakeCurrent();

            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.MouseMove += GlControl_MouseMove;

            GL.ClearColor(0f, 0.25f, 0.5f, 1.0f);

            t.Interval = 16; //ms
            t.Tick += (o, args) =>
            {
                Update();
                Render();
            };
            t.Enabled = true;

            Cubes.Add(new Cube(new Vector3(1, 0, 0), new Vector4(1.0f, 0.25f, 0.25f, 1.0f)));
            Cubes.Add(new Cube(new Vector3(-1, 0, 0), new Vector4(0.25f, 1.0f, 0.25f, 1.0f)));

            GenerateUBO();

            foreach (Cube c in Cubes)
            {
                // Get the index to the Matrices block for the shader program
                int uniformIndex1 = GL.GetUniformBlockIndex(c.ShaderProgramID, "Matrices");

                // Set the Matrices block in the shader program to bind point 0.
                GL.UniformBlockBinding(c.ShaderProgramID, uniformIndex1, 0);
            }
        }

        private int GenerateUBO()
        {
            int size = Cubes.Count * Marshal.SizeOf(typeof(Matrices));
            int test = 0;
            GL.GetInteger(GetPName.UniformBufferOffsetAlignment, out test);

            // Generate buffer for Uniform Buffer Object
            UBOID = GL.GenBuffer();

            // Bind to the generated buffer and allocate the necessary memory.
            GL.BindBuffer(BufferTarget.UniformBuffer, UBOID);
            GL.BufferData(BufferTarget.UniformBuffer, size, IntPtr.Zero, BufferUsageHint.StaticDraw);

            // Bind to bind point 0 and make it a mirror of the uniform buffer (?)
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, UBOID, IntPtr.Zero, size);

            return UBOID;
        }

        private void Update()
        {
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(cam.Fovy * ((float)Math.PI / 180.0f), 4 / 3.0f, 0.01f, 1000000.0f);

            Matrices[] mats = new Matrices[Cubes.Count];

            for (int i = 0; i < Cubes.Count; i++)
            {
                mats[i] = new Matrices()
                {
                    Projection = projection,
                    View = cam.ViewMatrix,
                    Model = Cubes[i].ModelMatrix,
                    Color = Cubes[i].Color
                };
            }

            GL.BindBuffer(BufferTarget.UniformBuffer, UBOID);
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, Cubes.Count * Marshal.SizeOf(typeof(Matrices)), mats);
        }

        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Cubes[0].Render(UBOID, 0);
            Cubes[1].Render(UBOID, Marshal.SizeOf(typeof(Matrices)));

            glControl.SwapBuffers();
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }

        private void WindowsFormsHost_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            cam.HandleInput(e.Key);
        }

        private void GlControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            cam.HandleScroll(e.Delta);
        }

        private void GlControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            cam.HandleRotate(e);
        }
    }
}
