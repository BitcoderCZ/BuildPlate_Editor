using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    public static class SkyBox
    {
        private static List<Vertex> verts = new List<Vertex>();
        private static List<uint> tris = new List<uint>();

        public static Vector3 pos;

        private static int vao;
        private static int vbo;
        private static int ebo;

        public static int texId { get; private set; }

        public static void Init(string texturePath, Vector3 cameraPos, float size)
        {
            if (File.Exists(texturePath)) {
                Texture tex = new Texture(texturePath);
                texId = tex.id;
            }
            else
                texId = -1;
            pos = cameraPos;
            Util.SkyboxTex(texId, Vector3.One * size, ref verts, ref tris);
            InitMesh();
        }

        static void InitMesh()
        {
            GL.CreateVertexArrays(1, out vao);
            GL.BindVertexArray(vao);
            GL.CreateBuffers(1, out ebo);
            GL.CreateBuffers(1, out vbo);
            CreateMesh();
        }

        static void CreateMesh()
        {
            GL.NamedBufferData(ebo, tris.Count * sizeof(uint), tris.ToArray(), BufferUsageHint.DynamicDraw);
            GL.VertexArrayElementBuffer(vao, ebo);

            int vertexBindingPoint = 0;
            GL.NamedBufferData(vbo, verts.Count * Vertex.Size, verts.ToArray(), BufferUsageHint.DynamicDraw);
            GL.VertexArrayVertexBuffer(vao, vertexBindingPoint, vbo, IntPtr.Zero, Vertex.Size);

            // pos
            GL.VertexArrayAttribFormat(vao, 0, 3, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vao, 0, vertexBindingPoint);
            GL.EnableVertexArrayAttrib(vao, 0);
            // uv
            GL.VertexArrayAttribFormat(vao, 1, 3, VertexAttribType.Float, false, 3 * sizeof(float));
            GL.VertexArrayAttribBinding(vao, 1, vertexBindingPoint);
            GL.EnableVertexArrayAttrib(vao, 1);
        }

        public static void Render(Shader s)
        {
            if (texId == -1)
                return;

            Matrix4 transform = Matrix4.CreateTranslation(pos);

            s.Bind();
            s.UploadMat4("uTransform", ref transform);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texId);
            s.UploadInt("text", 0);

            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, tris.Count, DrawElementsType.UnsignedInt, 0);
        }
    }
}
