using BuildPlate_Editor.Maths;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemPlus;
using SystemPlus.Utils;

namespace BuildPlate_Editor.UI
{
    public static class GUI
    {
        public static Vector2i slotSize;
        public static Vector2i iteminslotSize;
        public static Vector2i iteminslotOffset;
        public static Vector2i backSize;
        public static Vector2i backPos;


        public static int Scene { get; private set; }

        public static List<IGUIElement> elements;

        private static readonly string texPath = Program.baseDir + "/Data/Textures/UI/";
        private static readonly TextureInfo[] texturesToLoad = new TextureInfo[]
        {
            new TextureInfo("Crosshair"),
            new TextureInfo("options_background"),
            new TextureInfo("Button", TexFlip.Horizontal),
            new TextureInfo("Button_Selected", TexFlip.Horizontal),
            new TextureInfo("InputField", TexFlip.Horizontal),
            new TextureInfo("TextBox"),
        };
        public static readonly Dictionary<string, int> Textures = new Dictionary<string, int>();

        public static List<IGUIElement>[] Scenes;

        public static void Init(Font font)
        {
            elements = new List<IGUIElement>();

            Bitmap bm;
            for (int i = 0; i < texturesToLoad.Length; i++) {
                if (!File.Exists(texPath + texturesToLoad[i].name + ".png")) {
                    Console.WriteLine($"Block texture {texturesToLoad[i].name}, wasn't found. skipped.");
                    continue;
                }

                bm = (Bitmap)Image.FromFile(texPath + texturesToLoad[i].name + ".png");//DirectBitmap.Load(texPath + texturesToLoad[i].name + ".png", false);

                if (texturesToLoad[i].flip != TexFlip.None) {
                    if ((texturesToLoad[i].flip & TexFlip.Vertical) == TexFlip.Vertical)
                        bm.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    if ((texturesToLoad[i].flip & TexFlip.Horizontal) == TexFlip.Horizontal)
                        bm.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }

                if (texturesToLoad[i].colorFilter != Vector3.One) {
                    Vector3 filter = texturesToLoad[i].colorFilter;
                    for (int x = 0; x < bm.Width; x++)
                        for (int y = 0; y < bm.Height; y++) {
                            Color c = bm.GetPixel(x, y);
                            float r = (float)(c.R / 255f) * filter.X;
                            float g = (float)(c.G / 255f) * filter.Y;
                            float b = (float)(c.B / 255f) * filter.Z;
                            bm.SetPixel(x, y, Color.FromArgb(255, (byte)(r * 255f), (byte)(g * 255f), (byte)(b * 255f)));
                        }
                }

                BitmapData data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, 
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Textures.Add(texturesToLoad[i].name, new Texture(data, TextureWrapMode.Repeat).id);

                bm.UnlockBits(data);
                bm.Dispose();
            }

            LoadColorTextures();

            Scenes = new List<IGUIElement>[]
            {
                new List<IGUIElement>() { 
                    UIImage.CreateRepeate(-1f, -1f, 2f, 2f, Textures["options_background"], 12, 12),
                    UIImage.CreatePixel(new Vector2i(230, 250), new Vector2i(820, 60), Textures["Black"]), // loading bar
                    UIImage.CreatePixel(new Vector2i(240, 260), new Vector2i(0, 40), Textures["Green"]), // loading bar
                    UItext.CreateCenter("Loading...", 0, 50, 5f, font),
                },
                new List<IGUIElement>() {
                    UIImage.CreateCenter(0.06f, 0.06f, Textures["Crosshair"], true),
                }, // in game
                new List<IGUIElement>() { }, // save
                new List<IGUIElement>() {
                    new UIImage(-1f, -1f, 2f, 2f, Textures["BlackTransparent"], false),
                    UItext.CreateCenter("Input to Console", 0, 350, 3f, font),
                }, // select block in console
            };
            AddToScenes(font);
        }
        private static void AddToScenes(Font font)
        {
           
        }

        public static List<IGUIElement> GetUnderPoint(Vector2i point)
        {
            List<IGUIElement> list = new List<IGUIElement>();
            float x = ((float)point.X / Program.Window.Width * 2f) - 1f;
            float y = (((float)point.Y / Program.Window.Height * 2f) - 1f) * -1f;
            
            for (int i = 0; i < elements.Count; i++) {
                if (x >= elements[i].Position.X && y >= elements[i].Position.Y
                    && x < elements[i].Position.X + elements[i].Width && y < elements[i].Position.Y + elements[i].Height)
                    list.Add(elements[i]);
            }

            return list;
        }

        private static void LoadColorTextures()
        {
            DirectBitmap db = new DirectBitmap(1, 1);
            db.Clear(Color.FromArgb(255 / 3, 0, 0, 0));
            Textures.Add("BlackTransparent", new Texture(db.Data, db.Width, db.Height, TextureWrapMode.Repeat).id);
            db.Dispose();

            db = new DirectBitmap(1, 1);
            db.Clear(Color.FromArgb(255, 0, 0, 0));
            Textures.Add("Black", new Texture(db.Data, db.Width, db.Height, TextureWrapMode.Repeat).id);
            db.Dispose();

            db = new DirectBitmap(1, 1);
            db.Clear(Color.FromArgb(0, 0, 0, 0));
            Textures.Add("Transparent", new Texture(db.Data, db.Width, db.Height, TextureWrapMode.Repeat).id);
            db.Dispose();

            db = new DirectBitmap(1, 1);
            db.Clear(Color.FromArgb(255, 0, 255, 0));
            Textures.Add("Green", new Texture(db.Data, db.Width, db.Height, TextureWrapMode.Repeat).id);
            db.Dispose();

            db = new DirectBitmap(1, 1);
            db.Clear(Color.FromArgb(255, 255, 255, 255));
            Textures.Add("White", new Texture(db.Data, db.Width, db.Height, TextureWrapMode.Repeat).id);
            db.Dispose();
        }

        public static void SetScene(int id)
        {
            elements.Clear();
            for (int i = 0; i < Scenes[id].Count; i++)
                elements.Add(Scenes[id][i]);
            Scene = id;
        }

        public static void Update(float delta)
        {
            if (Scene == 0) { // fake load :)
                UIImage bar = elements[2] as UIImage;
                bar.Width += delta;
                if (bar.PixWidth > 800) {
                    bar.PixWidth = 800;
                    SetScene(1);
                }
                bar.UpdateVerts();
            }

            for (int i = 0; i < elements.Count; i++) {
                elements[i].Update(delta);
            }
        }

        public static void Render(Shader uiShader)
        {
            uiShader.Bind();
            for (int i = 0; i < elements.Count; i++) {
                elements[i].Render(uiShader);
            }
        }

        public static void OnKeyPress(char keyChar)
        {
            for (int i = 0; i < elements.Count; i++) {
                elements[i].OnKeyPress(keyChar);
            }
        }
        public static void OnKeyDown(Key key, KeyModifiers modifiers)
        {
            for (int i = 0; i < elements.Count; i++) {
                elements[i].OnKeyDown(key, modifiers);
            }
        }
        public static void OnKeyUp(Key key, KeyModifiers modifiers)
        {
            for (int i = 0; i < elements.Count; i++) {
                elements[i].OnKeyUp(key, modifiers);
            }
        }
        public static void OnMouseDown(MouseButton button, Point pos)
        {
            float x = (float)pos.X / Program.Window.Width * 2f - 1f;
            float y = ((float)pos.Y / Program.Window.Height * 2f - 1) * -1f;
            int _scene = Scene;
            for (int i = 0; i < elements.Count; i++) {
                elements[i].OnMouseDown(button,
                    x >= elements[i].Position.X && x < elements[i].Width + elements[i].Position.X &&
                    y >= elements[i].Position.Y && y < elements[i].Height + elements[i].Position.Y);
                if (_scene != Scene)
                    break;
            }
        }
        public static void OnMouseUp(MouseButton button, Point pos)
        {
            float x = (float)pos.X / Program.Window.Width;
            float y = (float)pos.Y / Program.Window.Height;
            for (int i = 0; i < elements.Count; i++) {
                elements[i].OnMouseUp(button,
                    x >= elements[i].Position.X && x < elements[i].Width + elements[i].Position.X &&
                    y >= elements[i].Position.Y && y < elements[i].Height + elements[i].Position.Y);
            }
        }
    }
}
