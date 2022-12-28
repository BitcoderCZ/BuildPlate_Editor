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
    public struct Vertex
    {
        public const int Size = 6 * sizeof(float);

        public Vector3 position;
        public Vector3 uv;

        public Vertex(Vector3 _pos, Vector2 _uv, uint _id)
        {
            position = _pos;
            uv = new Vector3(_uv.X, _uv.Y, (float)_id - 1f);
        }

        public Vertex(float x, float y, float z, float u, float v, uint _id)
        {
            position = new Vector3(x, y, z);
            uv = new Vector3(u, v, (float)_id - 1f);
        }

        public Vertex(float x, float y, float z) : this(x, y, z, 0f, 0f, 1)
        { }
    }
}
