using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor.UI
{
    public abstract class GUIElement : IGUIElement
    {
        public Vector3 Position { get; set; }

        public int PixX
        {
            get => (int)(((Position.X + 1f) / 2f) * (float)Util.Width); 
            set => Position = new Vector3((((float)value / (float)Util.Width) * 2f) - 1f, Position.Y, Position.Z);
        }
        public int PixY
        {
            get => (int)(((Position.Y + 1f) / 2f) * (float)Util.Height);
            set => Position = new Vector3(Position.X, (((float)value / (float)Util.Height) * 2f) - 1f, Position.Z);
        }

        public int PixWidth
        {
            get => (int)((Width / 2f) * (float)Util.Width); // Width is 0 - 2
            set => Width = ((float)value / (float)Util.Width) * 2f;
        }
        public int PixHeight
        {
            get => (int)((Height / 2f) * (float)Util.Height); // Height is 0 - 2
            set => Height = ((float)value / (float)Util.Height) * 2f;
        }

        public float Width { get; set; }
        public float Height { get; set; }

        public bool Active { get; set; }

        public Vertex2D[] vertices;
        public uint[] triangles;

        protected int vao;
        protected int vbo;
        protected int ebo;

        public abstract void Render(Shader s);
        public virtual void Update(float delta) { }
        public virtual void OnMouseDown(MouseButton button, bool onElement) { }
        public virtual void OnMouseUp(MouseButton button, bool onElement) { }
        public virtual void OnKeyPress(char keyChar) { }
        public virtual void OnKeyDown(Key key, KeyModifiers modifiers) { }
        public virtual void OnKeyUp(Key key, KeyModifiers modifiers) { }

        protected virtual void InitMesh()
        {
            GL.CreateVertexArrays(1, out vao);
            GL.BindVertexArray(vao);
            GL.CreateBuffers(1, out ebo);
            GL.CreateBuffers(1, out vbo);
            UploadMesh();
        }

        protected virtual void UploadMesh()
        {
            GL.NamedBufferData(ebo, triangles.Length * sizeof(uint), triangles, BufferUsageHint.StaticDraw);
            GL.VertexArrayElementBuffer(vao, ebo);

            int vertexBindingPoint = 0;
            GL.NamedBufferData(vbo, vertices.Length * Vertex2D.Size, vertices, BufferUsageHint.StaticDraw);
            GL.VertexArrayVertexBuffer(vao, vertexBindingPoint, vbo, IntPtr.Zero, Vertex2D.Size);

            // pos
            GL.VertexArrayAttribFormat(vao, 0, 3, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vao, 0, vertexBindingPoint);
            GL.EnableVertexArrayAttrib(vao, 0);
            // uv
            GL.VertexArrayAttribFormat(vao, 1, 2, VertexAttribType.Float, false, 3 * sizeof(float));
            GL.VertexArrayAttribBinding(vao, 1, vertexBindingPoint);
            GL.EnableVertexArrayAttrib(vao, 1);
        }

        ~GUIElement()
        {
            /*GL.DeleteBuffer(ebo);
            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);*/
            vertices = null;
            triangles = null;
        }
    }
}
