using BuildPlate_Editor.Maths;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemPlus.Utils;

namespace BuildPlate_Editor.UI
{
    public class UISliceImage : GUIElement
    {
        private Texture texCenter;
        private UIImage imgCenter;
        private int tex;

        public static UISliceImage CreateCenter(float width, float height, int borderWidthInPixels, int _textureID, bool mentainAspectRatio)
            => new UISliceImage(-width / 2f, -height / 2f, width, height, borderWidthInPixels, _textureID, mentainAspectRatio);

        public static UISliceImage CreatePixel(int x, int y, int width, int height, int borderWidthInPixels, int _textureID)
            => new UISliceImage(Util.PixelToGL(x, y), Util.PixelToNormal(width, height) * 2, borderWidthInPixels, _textureID, false);
        public static UISliceImage CreatePixel(Vector2i pos, Vector2i size, int borderWidthInPixels, int _textureID)
            => new UISliceImage(Util.PixelToGL(pos), Util.PixelToNormal(size) * 2, borderWidthInPixels, _textureID, false);

        public UISliceImage(float x, float y, float width, float height, int borderWidthInPixels, int texture, bool mentainAspectRatio = false)
        {
            Position = new Vector3(/*(mentainAspectRatio ? x / Program.Window.AspectRatio : */x/*)*/, y, 0f);
            Width = /*mentainAspectRatio ? width / Program.Window.AspectRatio : */width;
            Height = height;

            tex = texture;

            DirectBitmap _db = Texture.GetDB(texture);
            DirectBitmap db = new DirectBitmap(_db.Width, _db.Height);
            db.Draw(_db, 0, 0);

            float slice = (float)db.Width / (float)borderWidthInPixels;
            float sliceW = (height / slice) / (Program.Window.AspectRatio);
            float sliceH = height / slice;

            float z = 1f;
            float zero = 1f / slice;
            float one = 1f - 1f / slice;
            vertices = new Vertex2D[]
            {
                // Bottom-Left
                new Vertex2D(0, 0, -z, 0, 0), // 0
                new Vertex2D(0, sliceH, -z, 0, zero),
                new Vertex2D(sliceW, sliceH, -z, zero, zero),
                new Vertex2D(sliceW, 0, -z, zero, 0), // 3
                // Top-Left
                new Vertex2D(0, Height - sliceH, -z, 0, one), // 4
                new Vertex2D(0, Height, -z, 0, 1),
                new Vertex2D(sliceW, Height, -z, zero, 1),
                new Vertex2D(sliceW, Height - sliceH, -z, zero, one), // 7
                // Bottom-Right
                new Vertex2D(Width - sliceW, 0, -z, one, 0), // 8
                new Vertex2D(Width - sliceW, sliceH, -z, one, zero),
                new Vertex2D(Width, sliceH, -z, 1, zero),
                new Vertex2D(Width, 0, -z, 1, 0), // 11
                // Top-Right
                new Vertex2D(Width - sliceW, Height - sliceH, -z, one, one), // 12
                new Vertex2D(Width - sliceW, Height, -z, one, 1),
                new Vertex2D(Width, Height, -z, 1, 1),
                new Vertex2D(Width, Height - sliceH, -z, 1, one), // 15
            };
            triangles = new uint[]
            {
                // Bottom-Left
                2, 1, 0,
                0, 3, 2,
                // Top-Left
                6, 5, 4,
                4, 7, 6,
                // Bottom-Right
                10, 9, 8,
                8, 11, 10,
                // Top-Right
                14, 13, 12,
                12, 15, 14,
                // Top
                13, 6, 7,
                7, 12, 13,
                // Bottom
                9, 2, 3,
                3, 8, 9,
                // Left
                7, 4, 1,
                1, 2, 7,
                // Right
                14, 13, 9,
                9, 10, 14,
            };

            DirectBitmap center = new DirectBitmap(db.Width - borderWidthInPixels * 2, db.Height - borderWidthInPixels * 2);
            for (int i = 0; i < center.Height; i++)
                center.Write(i * center.Width, db.Data, (i + 2) * db.Width + 2, center.Width);

            float aspect = Width / Height;

            texCenter = new Texture(center);
            imgCenter = new UIImage(x, y, Width, Height, texCenter.id, false);
            imgCenter.SetMeshData(new Vertex2D[]
            {
                new Vertex2D(sliceW, sliceH, -z, zero, zero),
                new Vertex2D(sliceW, Height - sliceH, -z, zero, one),
                new Vertex2D(Width - sliceW, Height - sliceH, -z, one * (mentainAspectRatio ? aspect : 1f)
                * 2, one),
                new Vertex2D(Width - sliceW, sliceH, -z, one * (mentainAspectRatio ? aspect : 1f) * 2, zero),
            }, new uint[] 
            {
                2, 1, 0,
                0, 3, 2,
            });

            InitMesh();
            Active = true;
        }
        public UISliceImage(Vector2 _pos, float width, float height, int borderWidthInPixels, int _textureID, bool mentainAspectRatio) 
            : this(_pos.X, _pos.Y, width, height, borderWidthInPixels, _textureID, mentainAspectRatio)
        { }
        public UISliceImage(Vector2 _pos, Vector2 _size, int borderWidthInPixels, int _textureID, bool mentainAspectRatio) 
            : this(_pos.X, _pos.Y, _size.X, _size.Y, borderWidthInPixels, _textureID, mentainAspectRatio)
        { }

        public void UpdateVerts()
        {
            float z = 1f;
            vertices[0] = new Vertex2D(0f, 0f, -z, 0f, 1f);
            vertices[1] = new Vertex2D(0f, Height, -z, 0f, 0f);
            vertices[2] = new Vertex2D(Width, 0f, -z, 1f, 1f);
            vertices[3] = new Vertex2D(Width, Height, -z, 1f, 0f);

            UploadMesh();
        }

        public void Move()
        {
            imgCenter.Position = Position;
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
            GL.BindTexture(TextureTarget.Texture2D, tex);
            s.UploadInt("uTexture", slot);

            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, triangles.Length, DrawElementsType.UnsignedInt, 0);

            imgCenter.Render(s);
        }
    }
}
