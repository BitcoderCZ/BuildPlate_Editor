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
    public class SubChunk
    {
        List<Vertex> vertices = new List<Vertex>();
        List<uint> triangles = new List<uint>();

        private ChunkOutline outline;

        public readonly uint[] blocks;
        public readonly int[] renderers;
        public Palette[] palette;

        public Vector3i pos;

        private int vao;
        private int vbo;
        private int ebo;

        public int texId;

        public SubChunk(Vector3i _position, uint[] _blocks, int[] _renderers, Palette[] _palette, int _texId)
        {
            pos = _position;
            blocks = _blocks;
            renderers = _renderers;
            palette = _palette;
            texId = _texId;
            outline = new ChunkOutline(pos, VoxelData.ChunkWidth, 4f, Color.Yellow);
        }

        public static Vector3i[] possitionLookUp;

        static SubChunk()
        {
            possitionLookUp = new Vector3i[VoxelData.ChunkLayerLength * VoxelData.ChunkHeight];
            int x = 0;
            int y = 0;
            int z = 0;
            int origx;
            int origy;
            int origz;

            for (int currentBlock = 0; currentBlock < possitionLookUp.Length; currentBlock++) {
                z++;
                if (z == 16) { z = 0; y += 1; }
                if (y == 16) { y = 0; x += 1; }

                origz = z;
                origy = y;
                origx = x;

                if (z == 0) {
                    z = 16;
                    y -= 1;
                }

                if (y == -1)
                    y = 16;

                if (Math.Abs(z) % 16 == 0 && y == 16) {
                    y--;
                    x--;
                }

                possitionLookUp[currentBlock] = new Vector3i(x, y, z);

                z = origz;
                y = origy;
                x = origx;
            }
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

            for (int currentBlock = 0; currentBlock < blocks.Length; currentBlock++) {

                int renderer = renderers[currentBlock];
                if (renderer > -1 && blocks[currentBlock] < palette.Length) { // don't render air
#if DEBUG
                    uint blockId = blocks[currentBlock];
                    Palette pal = palette[blockId];
                    World.blockRenderers[renderer](possitionLookUp[currentBlock], pos * 16, pal.textures, pal.data,
                        ref vertices, ref triangles);
#else
                try {
                    Palette pal = palette[blocks[currentBlock]];
                    World.blockRenderers[renderer](possitionLookUp[currentBlock], pos * 16, pal.textures, pal.data,
                        ref vertices, ref triangles);
                } catch (Exception ex) {
                    uint b = blocks[currentBlock];
                    string name = "Couldn't get";
                    string data = "Couldn't get";
                    if (b >= 0 && b < palette.Length) {
                        name = palette[b].name;
                        data = palette[b].data.ToString();
                    }
                    Util.Exit(EXITCODE.World_Render_Block, ex, $"Block ID: {blocks[currentBlock]}, SubChunk pos: {pos}, Renderer ID: {renderer}, " +
                        $"Block Name: {name}, Block Data: {data}");
                }
#endif
                }
            }
        }

        public void InitMesh()
        {
            GL.CreateVertexArrays(1, out vao);
            GL.BindVertexArray(vao);
            GL.CreateBuffers(1, out ebo);
            GL.CreateBuffers(1, out vbo);
            CreateMesh();
        }

        public void CreateMesh()
        {
            GL.NamedBufferData(ebo, triangles.Count * sizeof(uint), triangles.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexArrayElementBuffer(vao, ebo);

            int vertexBindingPoint = 0;
            GL.NamedBufferData(vbo, vertices.Count * Vertex.Size, vertices.ToArray(), BufferUsageHint.StaticDraw);
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

        public void Render(Shader s, Shader outlineShader)
        {
            Matrix4 transform = Matrix4.CreateTranslation(new Vector3(pos.X * VoxelData.ChunkWidth, pos.Y * VoxelData.ChunkHeight, pos.Z * VoxelData.ChunkWidth));
            
            s.Bind();
            s.UploadMat4("uTransform", ref transform);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2DArray, texId);
            s.UploadInt("textures", 1);

            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, triangles.Count, DrawElementsType.UnsignedInt, 0);

            if (World.ShowChunkOutlines)
                outline.Render(outlineShader);
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

        public void GetBlockIndex(int bx, int by, int bz, out int blockIndex)
        {
            Vector3i offset = new Vector3i(pos.X * VoxelData.ChunkWidth, pos.Y * VoxelData.ChunkHeight, pos.Z * VoxelData.ChunkWidth);

            Vector3i block = new Vector3i(bx, by, bz);

            block -= offset;

            for (int currentBlock = 0; currentBlock < blocks.Length; currentBlock++) {
                if (possitionLookUp[currentBlock] == block) {
                    blockIndex = currentBlock;
                    return;
                }
            }

            blockIndex = -1;
        }

        public int GetPaletteIndex(string name, int data, bool compareData)
        {
            if (!name.Contains(":"))
                name = "minecraft:" + name;

            for (int i = 0; i < palette.Length; i++)
                if (palette[i].name == name && (!compareData || palette[i].data == data))
                    return i;

            return -1;
        }
        public Palette GetPaletteByName(string name)
        {
            if (!name.Contains(":"))
                name = "minecraft:" + name;

            for (int i = 0; i < palette.Length; i++)
                if (palette[i].name == name)
                    return palette[i];

            return Palette.NULL;
        }

        public void SetBlock(int blockIndex, int palletteIndex, int renderer, int data, bool update)
        {
            if (blockIndex < 0 || blockIndex >= blocks.Length)
                return;
            blocks[blockIndex] = (uint)palletteIndex;
            renderers[blockIndex] = renderer;
            Update();

            Vector3i p = possitionLookUp[blockIndex];

            if (p.X < 2)
                World.UpdateChunk(pos - Vector3i.UnitX);
            else if (p.X > 13)
                World.UpdateChunk(pos + Vector3i.UnitX);

            if (p.Y < 2)
                World.UpdateChunk(pos - Vector3i.UnitY);
            else if (p.Y > 13)
                World.UpdateChunk(pos + Vector3i.UnitY);

            if (p.Z < 2)
                World.UpdateChunk(pos - Vector3i.UnitZ);
            else if (p.Z > 13)
                World.UpdateChunk(pos + Vector3i.UnitZ);
        }

        public int AddNewPalette(string name, int data)
        {
            GL.DeleteTexture(texId);

            for (int i = 0; i < World.chunks.Length; i++)
                if (World.chunks[i].pos == pos) {
                    string mcn = name.Contains(':') ? name : "minecraft:" + name;
                    Array.Resize(ref palette, palette.Length + 1);
                    palette[palette.Length - 1] = new Palette(mcn, data, -1);
                    World.ReloadPaletteTextures(i);
                    return palette.Length - 1;
                }

            return -1;
        }

        public int NextPaletteIndex()
        {
            if (palette.Length > 0 && palette[palette.Length - 1].textures.Length > 0) {
                Palette p = palette[palette.Length - 1];
                return p.textures[p.textures.Length - 1];
            }
            else
                return -1;
        }

        public static bool WouldBeBlockInChunk(Vector3i _chunkPos, Vector3i blockPos)
        {
            Vector3i chunkPos = _chunkPos * 16;
            for (int i = 0; i < possitionLookUp.Length; i++)
                if (possitionLookUp[i] + chunkPos == blockPos)
                    return true;
            return false;
        }

        public static Vector3i GetToWhereCreateChunk(Vector3i shouldBeThere, Vector3i blockPos)
        {
            if (WouldBeBlockInChunk(shouldBeThere, blockPos))
                return shouldBeThere;

            for (int i = 0; i < VoxelData.faceChecks.Length; i++) {
                Vector3i offset = VoxelData.faceChecks[i];
                if (WouldBeBlockInChunk(shouldBeThere + offset, blockPos))
                    return shouldBeThere + offset;
            }
            return shouldBeThere;
        }

        class ChunkOutline
        {
            public Vector3 Position { get; set; }

            public ColVertex[] vertices;
            public uint[] triangles;
            public bool Active { get; set; }


            protected int vao;
            protected int vbo;
            protected int ebo;

            public float lineWidth;

            public ChunkOutline(Vector3i pos, float size, float _lineWidth, Color _c)
            {
                Vector4 c = new Vector4(
                        (float)_c.R / 255f,
                        (float)_c.G / 255f,
                        (float)_c.B / 255f,
                        (float)_c.A / 255f
                    );
                Vector3 half = Vector3.One / 2f;

                vertices = new ColVertex[VoxelData.voxelVerts.Length];
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = new ColVertex(VoxelData.voxelVerts[i] * size, c);

                triangles = VoxelData.voxelLines;

                lineWidth = _lineWidth;

                Position = pos * size + new Vector3(0.5f, 0.5f, 0.5f);

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

            public void Render(Shader s)
            {
                if (!Active)
                    return;

                s.Bind();
                Matrix4 transform = Matrix4.CreateTranslation(Position);
                s.UploadMat4("uTransform", ref transform);

                GL.BindVertexArray(vao);
                GL.LineWidth(lineWidth);
                GL.DrawElements(BeginMode.Lines, triangles.Length, DrawElementsType.UnsignedInt, 0);
            }
        }
    }
}
