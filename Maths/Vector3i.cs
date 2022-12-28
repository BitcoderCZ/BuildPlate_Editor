using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor.Maths
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3i
    {
        public static readonly Vector3i Zero = new Vector3i(0, 0, 0);
        public static readonly Vector3i One = new Vector3i(1, 1, 1);

        public int X;
        public int Y;
        public int Z;

        public Vector3i(int _X, int _Y, int _Z)
        {
            X = _X;
            Y = _Y;
            Z = _Z;
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
            => new Vector3i(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3i operator -(Vector3i a, Vector3i b)
            => new Vector3i(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3i operator *(Vector3i a, Vector3i b)
            => new Vector3i(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3i operator /(Vector3i a, Vector3i b)
            => new Vector3i(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static Vector3i operator *(Vector3i a, int b)
            => new Vector3i(a.X * b, a.Y * b, a.Z * b);
        public static Vector3i operator /(Vector3i a, int b)
            => new Vector3i(a.X / b, a.Y / b, a.Z / b);
        public static Vector3 operator *(Vector3i a, float b)
            => new Vector3(a.X * b, a.Y * b, a.Z * b);
        public static Vector3 operator /(Vector3i a, float b)
            => new Vector3(a.X / b, a.Y / b, a.Z / b);

        public static Vector3i operator +(Vector3i a)
            => a;
        public static Vector3i operator -(Vector3i a)
            => new Vector3i(-a.X, -a.Y, -a.Z);

        public static implicit operator Vector3(Vector3i a)
            => new Vector3((float)a.X, (float)a.Y, (float)a.Z);
        public static explicit operator Vector3i(Vector3 a)
            => new Vector3i((int)a.X, (int)a.Y, (int)a.Z);

        public static bool operator ==(Vector3i a, Vector3i b)
            => a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        public static bool operator !=(Vector3i a, Vector3i b)
            => a.X != b.X || a.Y != b.Y || a.Z != b.Z;

        public override string ToString() => $"({X}, {Y}, {Z})";

        public override bool Equals(object obj)
        {
            if (obj is Vector3i v)
                return this == v;
            else
                return false;
        }

        public override int GetHashCode() => X ^ Y ^ Z;
    }
}
