using BuildPlate_Editor.Maths;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor.UI
{
    public class UIImage : GUIElement
    {
        public int textureID;

        private float z;

        public static UIImage CreateCenter(float width, float height, int _textureID, bool mentainAspectRatio)
            => new UIImage(-width / 2f, -height / 2f, width, height, _textureID, mentainAspectRatio);

        public static UIImage CreatePixel(int x, int y, int width, int height, int _textureID, float z = 1f)
            => new UIImage(Util.PixelToGL(x, y), Util.PixelToNormal(width, height) * 2, _textureID, false, z);
        public static UIImage CreatePixel(Vector2i pos, Vector2i size, int _textureID, float z = 1f)
            => new UIImage(Util.PixelToGL(pos), Util.PixelToNormal(size) * 2, _textureID, false, z);

        public static UIImage CreateRepeate(float x, float y, float width, float height, int _textureID, int _repX, int repY, float z = 1f)
        {
            UIImage img = new UIImage(x, y, width, height, _textureID, false, z);

            float repX = (float)_repX * Program.Window.AspectRatio;

            img.vertices[0] = new Vertex2D(0f, 0f, -z, 0f, 1f * repY);
            img.vertices[1] = new Vertex2D(0f, img.Height, -z, 0f, 0f);
            img.vertices[2] = new Vertex2D(img.Width, 0f, -z, 1f * repX, 1f * repY);
            img.vertices[3] = new Vertex2D(img.Width, img.Height, -z, 1f * repX, 0f);

            img.UploadMesh();
            return img;
        }

        public UIImage(float x, float y, float width, float height, int _textureID, bool mentainAspectRatio, float _z = 1f)
        {
            Position = new Vector3((mentainAspectRatio ? x / Program.Window.AspectRatio : x), y, 0f);
            Width = mentainAspectRatio ? width / Program.Window.AspectRatio : width;
            Height = height;
            textureID = _textureID;

            z = _z;
            vertices = new Vertex2D[4]
            {
                new Vertex2D(0f, 0f, -z, 0f, 1f),
                new Vertex2D(0f, Height, -z, 0f, 0f),
                new Vertex2D(Width, 0f, -z, 1f, 1f),
                new Vertex2D(Width, Height, -z, 1f, 0f),
            };
            triangles = new uint[6]
            {
                2, 1, 0,
                2, 3, 1,
            };

            InitMesh();
            Active = true;
        }
        public UIImage(Vector2 _pos, float width, float height, int _textureID, bool mentainAspectRatio, float z = 1f) : this(_pos.X, _pos.Y, width, height, _textureID, mentainAspectRatio, z)
        { }
        public UIImage(Vector2 _pos, Vector2 _size, int _textureID, bool mentainAspectRatio, float z = 1f) : this(_pos.X, _pos.Y, _size.X, _size.Y, _textureID, mentainAspectRatio, z)
        { }

        public void UpdateVerts()
        {
            vertices[0] = new Vertex2D(0f, 0f, -z, 0f, 1f);
            vertices[1] = new Vertex2D(0f, Height, -z, 0f, 0f);
            vertices[2] = new Vertex2D(Width, 0f, -z, 1f, 1f);
            vertices[3] = new Vertex2D(Width, Height, -z, 1f, 0f);

            UploadMesh();
        }
        public void SetMeshData(Vertex2D[] v, uint[] t)
        {
            vertices[0] = v[0];
            vertices[1] = v[1];
            vertices[2] = v[2];
            vertices[3] = v[3];

            triangles[0] = t[0];
            triangles[1] = t[1];
            triangles[2] = t[2];
            triangles[3] = t[3];
            triangles[4] = t[4];
            triangles[5] = t[5];

            UploadMesh();
        }

        public override void Render(Shader s)
        {
            if (!Active)
                return;

            s.Bind();
            Matrix4 mat = Matrix4.CreateTranslation(Position);
            s.UploadMat4("uTransform", ref mat);

            int slot = 0;
            GL.ActiveTexture(TextureUnit.Texture0 + slot);
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            s.UploadInt("uTexture", slot);

            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, triangles.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
