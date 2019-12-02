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

            c = new Cube(new Vector3(1, 0, 0));
            c2 = new Cube(new Vector3(-1, 0, 0));

            GenerateUBO();
        }

        private void GenerateUBO()
        {
            // Get the indices to the Matrices block for each shader program.
            int uniformIndex1 = GL.GetUniformBlockIndex(c.ShaderProgramID, "Matrices");
            int uniformIndex2 = GL.GetUniformBlockIndex(c2.ShaderProgramID, "Matrices");

            // Set the Matrices block in both shader programs to bind point 0.
            GL.UniformBlockBinding(c.ShaderProgramID, uniformIndex1, 0);
            GL.UniformBlockBinding(c2.ShaderProgramID, uniformIndex2, 0);

            // Generate buffer for Uniform Buffer Object
            int uboMatrices = GL.GenBuffer();

            // Bind to the generated buffer and allocate the necessary memory.
            GL.BindBuffer(BufferTarget.UniformBuffer, uboMatrices);
            GL.BufferData(BufferTarget.UniformBuffer, 128, IntPtr.Zero, BufferUsageHint.StaticDraw);

            // Bind to bind point 0 and make it a mirror of the uniform buffer (?)
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, uboMatrices, IntPtr.Zero, 128);

            // Create projection matrix
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(45.0f * ((float)Math.PI / 180.0f), 4 / 3.0f, 0.01f, 1000000.0f);

            // Create view matrix
            Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, -10), Vector3.UnitZ, Vector3.UnitY);

            // Bind to the uniform buffer again and upload the matrices
            GL.BindBuffer(BufferTarget.UniformBuffer, uboMatrices);
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, 64, ref projection);
            GL.BufferSubData(BufferTarget.UniformBuffer, (IntPtr)64, 64, ref view);

            // ???
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        private void Render()
        {
            GL.ClearColor(0.0f, 0.25f, 0.5f, 1.0f);

            c.Render();
            c2.Render();

            glControl.SwapBuffers();
        }

        private void WindowsFormsHost_Initialized(object sender, EventArgs e)
        {

        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }
    }
}
