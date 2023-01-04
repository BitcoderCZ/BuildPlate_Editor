using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor.UI
{
    public class UISliceButton : GUIElement
    {
        public UItext uiText;

        public string Text
        {
            get => uiText.Text;
            set { uiText.Text = value; uiText.UpdateMesh(); }
        }

        public UISliceImage uiImage;

        public float PadTop;
        public float PadBottom;
        public float PadLeft;
        public float PadRight;

        public Action<MouseButton> OnClick;

        public UISliceButton(string text, float x, float y, float textScale, Font font, int texID, int border, Action<MouseButton> _onClick = null,
            bool mantainAspect = false, float padT = 0.05f, float padB = 0.05f, float padL = 0.05f, float padR = 0.05f)
        {
            Position = new Vector3(x, y, -1f);
            uiText = new UItext(text, x, y, textScale, font);
            Text = text;

            OnClick = _onClick;

            PadTop = padT;
            PadBottom = padB;
            PadLeft = padL;
            PadRight = padR;

            Width = uiText.Width + PadLeft + PadRight;
            Height = uiText.Height + PadBottom + PadTop;

            uiImage = new UISliceImage(x, y, uiText.Width + PadLeft + PadRight, uiText.Height + PadBottom + PadTop, border,
                texID, mantainAspect);


            UpdateApparence();
        }

        public void UpdateApparence()
        {
            uiText.SetPos(Position.X + PadLeft, Position.Y + PadBottom);
            uiImage.Position = new Vector3(Position.X, Position.Y, uiImage.Position.Z);
            uiImage.Move();
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
