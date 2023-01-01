using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemPlus.Utils;

namespace BuildPlate_Editor
{
    public static class SkyBox
    {
        private static Vector3[] verts = new Vector3[0];
        private static uint[] tris = new uint[0];

        public static Vector3 pos;

        private static float size;

        private static int vao;
        private static int vbo;
        private static int ebo;

        public static int texId { get; private set; }

        private static string[] MultiImageLookUp = new string[]
        {
            "+X", "-X", "+Y", "-Y", "+Z", "-Z"
        };

        public static void Init(string skyboxName, Vector3 cameraPos, float _size)
        {
            string fileName = "Skybox_" + skyboxName;
            string baseTexPath = Program.baseDir + "Data/Textures/";
            if (File.Exists(baseTexPath + fileName + ".png")) {
                DirectBitmap db = DirectBitmap.Load(baseTexPath + fileName + ".png");
                texId = Texture.GenerateCubeMap(db, Texture.CreateOffsets(db.Width, db.Height));
            }
            else if (Directory.Exists(baseTexPath + fileName)) {
                string dir = baseTexPath + fileName + "/";
                string[] paths = new string[MultiImageLookUp.Length];
                for (int i = 0; i < MultiImageLookUp.Length; i++) {
                    string path = dir + skyboxName + MultiImageLookUp[i] + ".png";
                    if (!File.Exists(path)) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Couldn't load part of skybox \"{skyboxName}\": {path}");
                        Console.ResetColor();
                        texId = -1;
                        goto cont;
                    }
                    else
                        paths[i] = path;
                }
                texId = Texture.GenerateCubeMap(paths);
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Couldn't load skybox \"{skyboxName}\": {baseTexPath + fileName + ".png"}");
                Console.ResetColor();
                texId = -1;
            }

            cont: // continue
            pos = cameraPos;
            size = _size;

            verts = VoxelData.SkyBox2.verts;
            tris = VoxelData.SkyBox2.tris;
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
            GL.NamedBufferData(ebo, tris.Length * sizeof(uint), tris, BufferUsageHint.DynamicDraw);
            GL.VertexArrayElementBuffer(vao, ebo);

            int vertexBindingPoint = 0;
            GL.NamedBufferData(vbo, verts.Length * sizeof(float) * 3, verts, BufferUsageHint.DynamicDraw);
            GL.VertexArrayVertexBuffer(vao, vertexBindingPoint, vbo, IntPtr.Zero, sizeof(float) * 3);

            // pos
            GL.VertexArrayAttribFormat(vao, 0, 3, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vao, 0, vertexBindingPoint);
            GL.EnableVertexArrayAttrib(vao, 0);
        }

        public static void Render(Shader s)
        {
            if (texId == -1)
                return;

            Matrix4 transform = Matrix4.CreateScale(size);
            transform *= Matrix4.CreateTranslation(pos);

            s.Bind();
            s.UploadMat4("uTransform", ref transform);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, texId);
            s.UploadInt("tex", 0);

            GL.DepthMask(false);

            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, tris.Length, DrawElementsType.UnsignedInt, 0);

            GL.DepthMask(true);
        }
    }

    /*public static class SkyBox
    {
        private static Vertex2[] verts = new Vertex2[0];
        private static uint[] tris = new uint[0];

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
            //Util.SkyboxTex(Vector3.One * size, ref verts, ref tris);
            SphereMesh.Create(5, size, ref tris, ref verts);
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
            GL.NamedBufferData(ebo, tris.Length * sizeof(uint), tris, BufferUsageHint.DynamicDraw);
            GL.VertexArrayElementBuffer(vao, ebo);

            int vertexBindingPoint = 0;
            GL.NamedBufferData(vbo, verts.Length * Vertex2.Size, verts, BufferUsageHint.DynamicDraw);
            GL.VertexArrayVertexBuffer(vao, vertexBindingPoint, vbo, IntPtr.Zero, Vertex2.Size);

            // pos
            GL.VertexArrayAttribFormat(vao, 0, 3, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vao, 0, vertexBindingPoint);
            GL.EnableVertexArrayAttrib(vao, 0);
            // uv
            GL.VertexArrayAttribFormat(vao, 1, 2, VertexAttribType.Float, false, 3 * sizeof(float));
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
            s.UploadInt("tex", 0);

            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, tris.Length, DrawElementsType.UnsignedInt, 0);
        }
    }*/
}
