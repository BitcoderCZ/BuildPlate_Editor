using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor.UI
{
    public class UIButton : GUIElement
    {
        public UItext uiText;

        public string Text
        {
            get => uiText.Text;
            set { uiText.Text = value; uiText.UpdateMesh(); }
        }

        public UIImage uiImage;

        public float PadTop;
        public float PadBottom;
        public float PadLeft;
        public float PadRight;

        public Action<MouseButton> OnClick;

        public UIButton(string text, float x, float y, float textScale, Font font, int texID, bool mantainAspect = false, float padT = 0.05f, float padB = 0.05f, float padL = 0.05f, float padR = 0.05f)
        {
            uiText = new UItext(text, x, y, textScale, font);
            Text = text;

            uiImage = new UIImage(x, y, uiText.Width, uiText.Height, texID, mantainAspect, 1f);

            PadTop = padT;
            PadBottom = padB;
            PadLeft = padL;
            PadRight = padR;

            UpdateApparence();
        }

        public void UpdateApparence()
        {
            uiText.SetPos(Position.X + PadLeft, Position.Y + PadBottom);
            uiImage.Position = new Vector3(Position.X, Position.Y, uiImage.Position.Z);

            uiImage.Width = uiText.Width + PadLeft + PadRight;
            uiImage.Height = uiText.Height + PadBottom + PadTop;
            uiImage.UpdateVerts();
        }

        public override void Render(Shader s)
        {
            uiImage.Render(s);
            uiText.Render(s);
        }

        public override void OnMouseDown(MouseButton button, bool onElement)
        {
            if (onElement)
                OnClick?.Invoke(button);
        }
    }
}