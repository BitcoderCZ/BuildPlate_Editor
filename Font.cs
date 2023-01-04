using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemPlus.Utils;

namespace BuildPlate_Editor
{
    public class Font
    {
        public readonly Dictionary<char, int> Chars;
        public readonly Dictionary<char, Size> Sizes;
        public float RenderedSize;

        private static readonly string fontsPath = Program.baseDir + "/Data/Fonts/";

        public Font(string name, int renderedSize = 32)
        {
            Chars = new Dictionary<char, int>();
            Sizes = new Dictionary<char, Size>();
            RenderedSize = renderedSize;

            string infoPath = fontsPath + name + ".ifh";
            string dataName = name + ".ifh";
            Dictionary<char, CharInfo> chars = new Dictionary<char, CharInfo>();

            string[] lines = File.ReadAllLines(infoPath);
            string[] split;
            string[] charSplit;

            for (int i = 0; i < lines.Length; i++) {
                split = lines[i].Split(',');
                if (split.Length < 2)
                    continue;

                if (split[0] == "info") 
                { }
                else if (split[0] == "data_name") {
                    for (int j = 1; j < split.Length; j++) {
                        charSplit = split[j].Split('=');
                        if (charSplit.Length != 2)
                            continue;

                        if (charSplit[0] == "data")
                            dataName = charSplit[1];
                    }
                } else if (split[0] == "char_info" && split.Length >= 6) {
                    char ch = '\0';
                    CharInfo info = CharInfo.InitInvalid();
                    for (int j = 1; j < split.Length; j++) {
                        charSplit = split[j].Split('=');
                        if (charSplit.Length != 2)
                            continue;

                        try {
                            if (charSplit[0] == "char")
                                ch = charSplit[1][0];
                            else if (charSplit[0] == "x")
                                info.x = int.Parse(charSplit[1]);
                            else if (charSplit[0] == "y")
                                info.y = int.Parse(charSplit[1]);
                            else if (charSplit[0] == "width")
                                info.width = int.Parse(charSplit[1]);
                            else if (charSplit[0] == "height")
                                info.height = int.Parse(charSplit[1]);
                        } catch { }
                    }

                    if (ch != '\0' && CharInfo.Valid(info)) {
                        chars.Add(ch, info);
                        Sizes.Add(ch, new Size(info.width, info.height));
                    }
                }
            }

            string dataPath = fontsPath + dataName;

            CreateAndLoadTextures(chars, dataPath);
        }

        private void CreateAndLoadTextures(Dictionary<char, CharInfo> _chars, string dataPath)
        {
            DirectBitmap tileMap = DirectBitmap.Load(dataPath, false);

            DirectBitmap db;

            KeyValuePair<char, CharInfo>[] chars = _chars.ToArray();

            for (int i = 0; i < chars.Length; i++) {
                CharInfo info = chars[i].Value;
                db = new DirectBitmap(info.width, info.height);

                for (int y = 0; y < info.height; y++) {
                    db.Write(y * info.width, tileMap.Data, info.x + (info.y + y) * tileMap.Width, info.width);
                }

                for (int j = 0; j < db.Data.Length; j++) {
                    Color c = Color.FromArgb(db.Data[j]);
                    if (c.A > 0) {
                        c = Color.FromArgb(255, 255, 255, 255);
                    }
                    db.Data[j] = c.ToArgb();
                }

                Chars.Add(chars[i].Key, new Texture(db.Data, db.Width, db.Height, TextureWrapMode.Repeat).id);

                db.Dispose();
            }

            tileMap.Dispose();
        }
    }

    public struct CharInfo
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public static CharInfo InitInvalid()
            => new CharInfo() { x = -1, y = -1, width = -1, height = -1 };

        public static bool Valid(CharInfo info)
            => info.x != -1 && info.y != -1 && info.width != -1 && info.height != -1;
    }
}
