using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor.Maths
{
    public struct Vector2i
    {
        public static readonly Vector2i Zero = new Vector2i(0, 0);
        public static readonly Vector2i One = new Vector2i(1, 1);

        public int X;
        public int Y;

        public Vector2i(int _X, int _Y)
        {
            X = _X;
            Y = _Y;
        }

        public static Vector2i operator +(Vector2i a, Vector2i b)
            => new Vector2i(a.X + b.X, a.Y + b.Y);
        public static Vector2i operator -(Vector2i a, Vector2i b)
            => new Vector2i(a.X - b.X, a.Y - b.Y);
        public static Vector2i operator *(Vector2i a, Vector2i b)
            => new Vector2i(a.X * b.X, a.Y * b.Y);
        public static Vector2i operator /(Vector2i a, Vector2i b)
            => new Vector2i(a.X / b.X, a.Y / b.Y);

        public static Vector2i operator *(Vector2i a, int b)
            => new Vector2i(a.X * b, a.Y * b);
        public static Vector2i operator /(Vector2i a, int b)
            => new Vector2i(a.X / b, a.Y / b);
        public static Vector2 operator *(Vector2i a, float b)
            => new Vector2(a.X * b, a.Y * b);
        public static Vector2 operator /(Vector2i a, float b)
            => new Vector2(a.X / b, a.Y / b);

        public static Vector2i operator +(Vector2i a)
            => a;
        public static Vector2i operator -(Vector2i a)
            => new Vector2i(-a.X, -a.Y);

        public static implicit operator Vector2(Vector2i a)
            => new Vector2((float) a.X, (float) a.Y);
        public static explicit operator Vector2i(Vector2 a)
            => new Vector2i((int)a.X, (int)a.Y);

        public static bool operator ==(Vector2i a, Vector2i b)
            => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vector2i a, Vector2i b)
            => a.X != b.X || a.Y != b.Y;

        public override string ToString() => $"({X}, {Y})";

        public override bool Equals(object obj)
        {
            if (obj is Vector2i v)
                return this == v;
            else
                return false;
        }

        public override int GetHashCode() => X ^ Y;
    }
}
