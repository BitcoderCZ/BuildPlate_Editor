using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ColVertex
    {
        public const int Size = 7 * sizeof(float);

        public Vector3 position;
        public Vector4 color;

        public ColVertex(Vector3 _pos, Vector4 _color)
        {
            position = _pos;
            color = _color;
        }
    }
}
