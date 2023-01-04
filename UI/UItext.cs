using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor.UI
{
    public class UItext : GUIElement
    {
        public string Text;
        private UIImage[] chars;
        public float Scale;
        private Font font;

        public static UItext CreateCenter(string text, int xOff, int yOff, float scale, Font font)
        {
            float realWidth = 0f;
            for (int i = 0; i < text.Length; i++) {
                realWidth += (float)font.Sizes[text[i]].Width / Util.Width * scale;
            }
            float off = realWidth / 2f;
            return new UItext(text, (float)xOff / Util.Width - off, (float)yOff / Util.Height, scale, font);
        }

        public UItext(string _text, float x, float y, float scale, Font _font)
        {
            Position = new Vector3(x, y, 0f);
            Text = _text;
            chars = new UIImage[Text.Length];
            Scale = scale;
            font = _font;

            UpdateMesh();

            Active = true;
        }

        public override void Render(Shader s)
        {
            if (!Active)
                return;

            for (int i = 0; i < chars.Length; i++)
                chars[i].Render(s);
        }

        public void UpdateMesh()
        {
            chars = new UIImage[Text.Length];

            float xOff = 0f;
            float maxHeight = 0f;

            for (int i = 0; i < Text.Length; i++)
                CreateCharMesh(Text[i], i, ref xOff, ref maxHeight);

            Width = xOff;
            Height = maxHeight;
        }

        private void CreateCharMesh(char ch, int i, ref float xOff, ref float maxHeight)
        {
            float width = (float)font.Sizes[ch].Width / Util.Width * Scale;
            float height = (float)font.Sizes[ch].Height / Util.Height * Scale;

            if (height > maxHeight)
                maxHeight = height;

            float yOff = 0f;

            if (ch == 'p' || ch == 'q' || ch == 'g' || ch == 'y')
                yOff = 0.01f;

            yOff *= Scale;

            chars[i] = new UIImage(Position.X + xOff, Position.Y - yOff, width, height, font.Chars[ch], false);

            xOff += width;
        }

        public void SetPos()
            => SetPos(Position.X, Position.Y);
        public void SetPos(float x, float y)
        {
            Position = new Vector3(x, y, Position.Z);

            float xOff = 0f;
            for (int i = 0; i < Text.Length; i++) {
                float yOff = 0f;
                if (Text[i] == 'p' || Text[i] == 'q' || Text[i] == 'g' || Text[i] == 'y')
                    yOff = 0.01f;
                yOff *= Scale;

                chars[i].Position = new Vector3(Position.X + xOff, Position.Y - yOff, chars[i].Position.Z);

                xOff += (float)font.Sizes[Text[i]].Width / Util.Width * Scale;
            }
        }
    }
}
