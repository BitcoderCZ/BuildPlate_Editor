using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor.Graphics
{
    public class RenderObject : IRenderObject
    {
        public Vector3 Position { get; set; }

        public TexVertex[] vertices;
        public uint[] triangles;
        public bool Active { get; set; }

        protected int textureID;

        protected int vao;
        protected int vbo;
        protected int ebo;

        public RenderObject(TexVertex[] _verts, uint[] _tris, int _textureID, bool initMesh = true)
        {
            vertices = _verts;
            triangles = _tris;
            textureID = _textureID;
            if (initMesh)
                InitMesh();
        }

        private void InitMesh()
        {
            GL.CreateVertexArrays(1, out vao);
            GL.BindVertexArray(vao);
            GL.CreateBuffers(1, out ebo);
            GL.CreateBuffers(1, out vbo);
            UploadMesh();
        }

        public void UploadMesh()
        {
            GL.NamedBufferData(ebo, triangles.Length * sizeof(uint), triangles, BufferUsageHint.DynamicDraw);
            GL.VertexArrayElementBuffer(vao, ebo);

            int vertexBindingPoint = 0;
            GL.NamedBufferData(vbo, vertices.Length * TexVertex.Size, vertices, BufferUsageHint.DynamicDraw);
            GL.VertexArrayVertexBuffer(vao, vertexBindingPoint, vbo, IntPtr.Zero, TexVertex.Size);

            // pos
            GL.VertexArrayAttribFormat(vao, 0, 3, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vao, 0, vertexBindingPoint);
            GL.EnableVertexArrayAttrib(vao, 0);
            // uv
            GL.VertexArrayAttribFormat(vao, 1, 2, VertexAttribType.Float, false, 3 * sizeof(float));
            GL.VertexArrayAttribBinding(vao, 1, vertexBindingPoint);
            GL.EnableVertexArrayAttrib(vao, 1);
        }

        public virtual void Render(Shader s)
        {
            if (!Active)
                return;

            Matrix4 transform = Matrix4.CreateTranslation(Position);
            s.UploadMat4("uTransform", ref transform);

            int slot = 0;
            GL.ActiveTexture(TextureUnit.Texture0 + slot);
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            s.UploadInt("uTexture", slot);

            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, triangles.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
