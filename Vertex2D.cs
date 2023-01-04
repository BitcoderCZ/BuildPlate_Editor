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
    public struct Vertex2D
    {
        public const int Size = 5 * sizeof(float);

        public Vector3 position;
        public Vector2 uv;

        public Vertex2D(Vector3 _pos, Vector2 _uv)
        {
            position = _pos;
            uv = _uv;
        }

        public Vertex2D(float x, float y, float z, float u, float v)
        {
            position = new Vector3(x, y, z);
            uv = new Vector2(u, v);
        }

        public Vertex2D(float x, float y, float z) : this(x, y, z, 0f, 0f)
        { }
    }
}
