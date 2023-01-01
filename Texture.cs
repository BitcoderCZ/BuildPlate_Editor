using BuildPlate_Editor.Maths;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemPlus.Utils;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace BuildPlate_Editor
{
    public class Texture
    {
        public int id;
        public int Width;
        public int Height;

        public static (Vector2i offset, Vector2i size)[] CreateOffsets(int width, int height) // +X, -X, +Y, -Y, +Z, -Z
        {
            int w = (int)((float)width / 4f);
            int h = (int)((float)height / 3f);

            if (w > h)
                w = h;
            else if (h > w)
                h = w;

            return new (Vector2i offset, Vector2i size)[]
            {
                (new Vector2i(w * 2, h), new Vector2i(w, h)),
                (new Vector2i(0, h), new Vector2i(w, h)),
                (new Vector2i(w, 0), new Vector2i(w, h)),
                (new Vector2i(w, h * 2), new Vector2i(w, h)),
                (new Vector2i(w, h), new Vector2i(w, h)),
                (new Vector2i(w * 3, h), new Vector2i(w, h)),
            };
        }

        public static int GenerateCubeMap(DirectBitmap db, (Vector2i offset, Vector2i size)[] offsets)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.GenTextures(1, out int id);
            GL.BindTexture(TextureTarget.TextureCubeMap, id);

            int peaceWidth = (int)((float)db.Width / 4f);
            int peaceHeight = (int)((float)db.Height / 3f);

            // Texture must be 1:1
            if (peaceWidth > peaceHeight)
                peaceWidth = peaceHeight;
            else if (peaceHeight > peaceWidth)
                peaceHeight = peaceWidth;

            for (int i = 0; i < 6; i++) {

                DirectBitmap peace = new DirectBitmap(peaceWidth, peaceHeight);
                for (int j = 0; j < peaceHeight; j++)
                    peace.Write(j * peaceWidth, db.Data, (offsets[i].offset.Y + j) * db.Width + offsets[i].offset.X, peaceWidth);


                BitmapData data = peace.Bitmap.LockBits(new Rectangle(0, 0, peaceWidth, peaceHeight), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgb, peaceWidth, peaceHeight, 0,
                    PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                peace.Bitmap.UnlockBits(data);
                peace.Dispose();
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            return id;
        }
        public static int GenerateCubeMap(string[] paths)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.GenTextures(1, out int id);
            GL.BindTexture(TextureTarget.TextureCubeMap, id);

            if (paths.Length != 6)
                throw new Exception($"Skybox must be 6 images not {paths.Length}");

            for (int i = 0; i < paths.Length; i++) {

                DirectBitmap db = DirectBitmap.Load(paths[i]);

                BitmapData data = db.Bitmap.LockBits(new Rectangle(0, 0, db.Width, db.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgb, db.Width, db.Height, 0,
                    PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                db.Bitmap.UnlockBits(data);
                db.Dispose();
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            return id;
        }

        public static int CreateTextureArray(string[] texturesToLoad, TexFlip flip)
        {
            int taid;
            GL.ActiveTexture(TextureUnit.Texture1);
            taid = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, taid);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMaxLevel, 4);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, SizedInternalFormat.Rgba32f, 16, 16, texturesToLoad.Length/*count*/);

            Bitmap bm;
            BitmapData data;

            Console.WriteLine("TEXTURE:BLOCKARRAY:GENERATE:START");
            for (int i = 0; i < texturesToLoad.Length; i++) {
                string texPath = texturesToLoad[i];

                if (!File.Exists(texPath)) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    string name = Path.GetFileNameWithoutExtension(texPath);
                    if (name != "air" && !name.Contains("constraint"))
                        Console.WriteLine($"Block texture {Path.GetFileName(texPath)}, wasn't found");
                    Console.ResetColor();
                    texPath = Program.baseDir + "Data\\Textures\\Black.png";
                }

                bm = new Bitmap(texPath);

                if (flip != TexFlip.None) {
                    if ((flip & TexFlip.Vertical) == TexFlip.Vertical)
                        bm.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    if ((flip & TexFlip.Horizontal) == TexFlip.Horizontal)
                        bm.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }

                data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, bm.Width, bm.Height, 1, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                bm.UnlockBits(data);
                bm.Dispose();
            }
            Console.WriteLine($"TEXTURE:BLOCKARRAY:GENERATE:DONE count: {texturesToLoad.Length}");

            return taid;
        }

        public Texture(string path)
        {
            DirectBitmap db = DirectBitmap.Load(path, false);

            Width = db.Width;
            Height = db.Height;

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.GenTextures(1, out id);
            GL.BindTexture(TextureTarget.Texture2D, id);

            BitmapData data = db.Bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            db.Bitmap.UnlockBits(data);

            db.Dispose();
        }
    }

    public struct TextureInfo
    {
        public string name;
        public TexFlip flip;
        public Vector3 colorFilter;

        public TextureInfo(string _name, TexFlip _flip, Vector3 _colorFilter)
        {
            name = _name;
            flip = _flip;
            colorFilter = _colorFilter;
        }

        public TextureInfo(string _name, TexFlip _flip) : this(_name, _flip, Vector3.One)
        { }

        public TextureInfo(string _name, Vector3 _colorFilter) : this(_name, TexFlip.None, _colorFilter)
        { }

        public TextureInfo(string _name) : this(_name, TexFlip.None, Vector3.One)
        { }
    }

    [Flags]
    public enum TexFlip : byte
    {
        None = 0b0000_0000,
        Vertical = 0b0000_0001,
        Horizontal = 0b_0000_0010,
    }
}
