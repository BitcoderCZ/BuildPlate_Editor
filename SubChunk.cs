using BuildPlate_Editor.Maths;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    public class SubChunk
    {
        List<Vertex> vertices = new List<Vertex>();
        List<uint> triangles = new List<uint>();

        public readonly uint[] blocks;
        public readonly int[] renderers;
        public readonly Palette[] palette;

        public Vector3i pos;

        private int vao;
        private int vbo;
        private int ebo;

        public readonly int texId;

        public SubChunk(Vector3i _position, uint[] _blocks, int[] _renderers, Palette[] _palette, int _texId)
        {
            pos = _position;
            blocks = _blocks;
            renderers = _renderers;
            palette = _palette;
            texId = _texId;
        }

        public void Init()
        {
            CreateMeshData();
            InitMesh();
        }

        public void CreateMeshData()
        {
            vertices.Clear();
            triangles.Clear();

            int x = 0;
            int y = 0;
            int z = 0;
            int origx;
            int origy;
            int origz;

            for (int currentBlock = 0; currentBlock < blocks.Length; currentBlock++) {
                z++;
                if (z == 16) { z = 0; y += 1; }
                if (y == 16) { y = 0; x += 1; }

                origz = z;
                origy = y;

                if (z == 0) {
                    z = 16;
                    y -= 1;
                }

                if (y == -1)
                    y = 16;

                int renderer = renderers[currentBlock];
                if (renderer > -1 && blocks[currentBlock] < palette.Length) // don't render air
                    World.blockRenderers[renderer](new Vector3(x, y, z), pos * 16, palette[blocks[currentBlock]].textures, palette[blocks[currentBlock]].data,
                        ref vertices, ref triangles);

                z = origz;
                y = origy;
            }
        }

        void AddVoxelDataToChunk(Vector3i pos, uint blockId)
        {
            int renderer = GetRenderer(pos);
            if (renderer > -1)
                World.blockRenderers[renderer](pos, this.pos * 16, palette[blockId].textures, palette[blockId].data, ref vertices, ref triangles);
        }

        void InitMesh()
        {
            GL.CreateVertexArrays(1, out vao);
            GL.BindVertexArray(vao);
            GL.CreateBuffers(1, out ebo);
            GL.CreateBuffers(1, out vbo);
            CreateMesh();
        }

        public void CreateMesh()
        {
            GL.NamedBufferData(ebo, triangles.Count * sizeof(uint), triangles.ToArray(), BufferUsageHint.DynamicDraw);
            GL.VertexArrayElementBuffer(vao, ebo);

            int vertexBindingPoint = 0;
            GL.NamedBufferData(vbo, vertices.Count * Vertex.Size, vertices.ToArray(), BufferUsageHint.DynamicDraw);
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

        public void Update()
        {
            CreateMeshData();
            CreateMesh();
        }

        public void Render(Shader s)
        {
            Matrix4 transform = Matrix4.CreateTranslation(new Vector3(pos.X * VoxelData.ChunkWidth, pos.Y * VoxelData.ChunkHeight, pos.Z * VoxelData.ChunkWidth));
            
            s.Bind();
            s.UploadMat4("uTransform", ref transform);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2DArray, texId);
            s.UploadInt("textures", 1);

            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, triangles.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void SetBlock(int X, int Y, int Z, uint id)
        {
            if (X < 0 || X >= VoxelData.ChunkWidth || Y < 0 || Y >= VoxelData.ChunkHeight || Z < 0 || Z >= VoxelData.ChunkWidth)
                return;
            blocks[Z + (X * VoxelData.ChunkWidth) + (Y * VoxelData.ChunkLayerLength)] = id;
        }
        public void SetBlock(Vector3i pos, uint id)
            => SetBlock(pos.X, pos.Y, pos.Z, id);

        public uint GetBlock(int X, int Y, int Z)
        {
            if (X < 0 || X >= VoxelData.ChunkWidth || Y < 0 || Y >= VoxelData.ChunkHeight || Z < 0 || Z >= VoxelData.ChunkWidth)
                return 0;

            return blocks[Z + (X * VoxelData.ChunkWidth) + (Y * VoxelData.ChunkLayerLength)];
        }
        public uint GetBlock(Vector3i pos)
            => GetBlock(pos.X, pos.Y, pos.Z);

        public int GetRenderer(int X, int Y, int Z)
        {
            if (X < 0 || X >= VoxelData.ChunkWidth || Y < 0 || Y >= VoxelData.ChunkHeight || Z < 0 || Z >= VoxelData.ChunkWidth)
                return 0;

            return renderers[Z + (X * VoxelData.ChunkWidth) + (Y * VoxelData.ChunkLayerLength)];
        }

        public int GetRenderer(Vector3i pos)
            => GetRenderer(pos.X, pos.Y, pos.Z);
    }
}
