using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Runtime.InteropServices;

namespace Cuba
{
    public struct Light
    {
        public Vector4 Position;
        public Vector4 Color;
        public float Attenuation;
        public float AmbientCoefficient;
    }
}
