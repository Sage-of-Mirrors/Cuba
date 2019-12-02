using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace Cuba
{
    public class Cube
    {
        public int ShaderProgramID { get; private set; }

        private int m_VBO;
        private int m_IBO;

        private float[] m_Vertices = new float[24]
        {
            0.5f, 0.5f, 0.5f,
           -0.5f, 0.5f, 0.5f,
            -0.5f, 0.5f, -0.5f,
            0.5f, 0.5f, -0.5f,
            0.5f, -0.5f, 0.5f,
            -0.5f, -0.5f, 0.5f,
            -0.5f, -0.5f, -0.5f,
            0.5f, -0.5f, -0.5f
        };

        private uint[] m_Indices = new uint[36]
        {
            0, 1, 2,
            2, 3, 0,

            0, 1, 4,
            1, 5, 4,

            1, 2, 5,
            2, 6, 5,

            2, 3, 6,
            3, 7, 6,

            3, 7, 4,
            3, 4, 0,

            4, 5, 6,
            6, 7, 4
        };

        private Matrix4 m_ModelMatrix = Matrix4.Identity;
        private Vector4 m_Color;

        public Cube(Vector3 translation, Vector4 color)
        {
            // Generate VBO and IBO buffers
            m_VBO = GL.GenBuffer();
            m_IBO = GL.GenBuffer();

            // Init shader program
            ShaderProgramID = InitShadersAndGetProgram();

            // Apply translation to model matrix
            m_ModelMatrix.M41 = translation.X;
            m_ModelMatrix.M42 = translation.Y;
            m_ModelMatrix.M43 = translation.Z;

            // Set color
            m_Color = color;

            // Bind VBO as an array buffer and upload vertex data
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(12 * m_Vertices.Length), m_Vertices, BufferUsageHint.StaticDraw);

            // Bind IBO as an element array buffer and upload index data
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(4 * m_Indices.Length), m_Indices, BufferUsageHint.StaticDraw);
        }

        public void Render()
        {
            // Ensure the proper shader program is bound
            GL.UseProgram(ShaderProgramID);

            // Update UBO with instance-specific data
            UBOManager.UpdateModelMatrix(m_ModelMatrix);
            UBOManager.UpdateColor(m_Color);

            // Bind VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_VBO);
            // Enable position attribute
            GL.EnableVertexAttribArray(0);
            // Tell the GPU about the position data.
            // Index 0, there are 3 elements, the elements are made of floats, they're not normalized, a single element is 12 bytes long,
            // and they start at offset 0.
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 12, 0);

            // Bind IBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_IBO);

            // Draw
            GL.DrawElements(BeginMode.Triangles, 12 * 3, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(0);
        }

        private int InitShadersAndGetProgram()
        {
            // Vertex shader
            int vShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vShader, File.ReadAllText("res/shader/vertshader.glsl"));
            GL.CompileShader(vShader);

            // Throw an error if vertex compilation failed
            string vShaderInfo = GL.GetShaderInfoLog(vShader);
            if (vShaderInfo != "")
            {
                Console.WriteLine($"Vertex shader compile failed: { vShaderInfo }");
                return -1;
            }

            // Fragment shader
            int fShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fShader, File.ReadAllText("res/shader/fragshader.glsl"));
            GL.CompileShader(fShader);

            // Throw an error if fragment compilation failed
            string fShaderInfo = GL.GetShaderInfoLog(fShader);
            if (fShaderInfo != "")
            {
                Console.WriteLine($"Fragment shader compile failed: { fShaderInfo }");
                return -1;
            }

            // Generate program
            int program = GL.CreateProgram();

            // Attach shaders to program
            GL.AttachShader(program, vShader);
            GL.AttachShader(program, fShader);

            // Link program
            GL.LinkProgram(program);

            // Make program active
            GL.UseProgram(program);

            return program;
        }
    }
}
