using BuildPlate_Editor.Graphics;
using BuildPlate_Editor.Maths;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    public class BlockOutline
    {
        public Vector3 Position { get; set; }

        public ColVertex[] vertices;
        public uint[] triangles;
        public bool Active { get; set; }


        protected int vao;
        protected int vbo;
        protected int ebo;

        public float lineWidth;
        public float blocksFromCam;

        public bool Raycast = false;
        public RaycastResult raycastResult;

        public BlockOutline(float size, float _lineWidth, float _blocksFromCam, Color _c)
        {
            Vector4 c = new Vector4(
                    (float)_c.R / 255f,
                    (float)_c.G/ 255f,
                    (float)_c.B / 255f,
                    (float)_c.A / 255f
                );
            Vector3 half = Vector3.One / 2f;

            vertices = new ColVertex[VoxelData.voxelVerts.Length];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = new ColVertex((VoxelData.voxelVerts[i] - half) * size, c);

            triangles = VoxelData.voxelLines;

            lineWidth = _lineWidth;
            blocksFromCam = _blocksFromCam;

            InitMesh();

            Active = true;
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
            GL.NamedBufferData(ebo, triangles.Length * sizeof(uint), triangles, BufferUsageHint.StaticDraw);
            GL.VertexArrayElementBuffer(vao, ebo);

            int vertexBindingPoint = 0;
            GL.NamedBufferData(vbo, vertices.Length * ColVertex.Size, vertices, BufferUsageHint.StaticDraw);
            GL.VertexArrayVertexBuffer(vao, vertexBindingPoint, vbo, IntPtr.Zero, ColVertex.Size);

            // pos
            GL.VertexArrayAttribFormat(vao, 0, 3, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vao, 0, vertexBindingPoint);
            GL.EnableVertexArrayAttrib(vao, 0);
            // color
            GL.VertexArrayAttribFormat(vao, 1, 4, VertexAttribType.Float, false, 3 * sizeof(float));
            GL.VertexArrayAttribBinding(vao, 1, vertexBindingPoint);
            GL.EnableVertexArrayAttrib(vao, 1);
        }

        public void Update()
        {
            Vector3 dir = Camera.target - Camera.position; // is normalized
            if (Raycast)
                raycastResult = World.Raycast(Camera.position, dir, 0.01f, 20f);
            else
                raycastResult = new RaycastResult((Vector3i)((Camera.position + dir * blocksFromCam) + Vector3.One / 2f));
            Position = raycastResult.HitPos;
        }

        public void Render(Shader s)
        {
            if (!Active) //|| Raycast)
                return;

            Matrix4 transform = Matrix4.CreateTranslation(Position);
            s.UploadMat4("uTransform", ref transform);

            GL.BindVertexArray(vao);
            GL.LineWidth(lineWidth);
            GL.DrawElements(BeginMode.Lines, triangles.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
