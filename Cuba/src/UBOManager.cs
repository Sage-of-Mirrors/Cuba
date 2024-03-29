﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Cuba
{
    [StructLayout(LayoutKind.Explicit, Size = 256)]
    public struct Matrices
    {
        [FieldOffset(0)] public Matrix4 Projection;
        [FieldOffset(64)] public Matrix4 View;
        [FieldOffset(128)] public Matrix4 Model;
        [FieldOffset(192)] public Vector4 Color;
        [FieldOffset(208)] public Light Light;
    }

    public static class UBOManager
    {
        public static int UBOID = -1;

        public static Matrices MatrixStruct;

        public static void LinkUBO(Cube c)
        {
            // Get the index to the Matrices block for the shader program
            int uniformIndex1 = GL.GetUniformBlockIndex(c.ShaderProgramID, "Matrices");

            // Set the Matrices block in the shader program to bind point 0.
            GL.UniformBlockBinding(c.ShaderProgramID, uniformIndex1, 0);
        }

        public static void UploadUBO()
        {
            if (UBOID < 0)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.UniformBuffer, UBOID);
            GL.BufferData(BufferTarget.UniformBuffer, 208, ref MatrixStruct, BufferUsageHint.StaticDraw);
        }

        static UBOManager()
        {
            // Generate buffer for Uniform Buffer Object
            UBOID = GL.GenBuffer();

            // Bind to the generated buffer and allocate the necessary memory.
            GL.BindBuffer(BufferTarget.UniformBuffer, UBOID);
            GL.BufferData(BufferTarget.UniformBuffer, 208, IntPtr.Zero, BufferUsageHint.StaticDraw);

            // Bind to bind point 0 and make it a mirror of the uniform buffer (?)
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, UBOID, IntPtr.Zero, 208);
        }
    }
}
