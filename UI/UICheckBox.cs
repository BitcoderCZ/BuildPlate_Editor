using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor.UI
{
    public class UICheckBox : GUIElement
    {
        private UIImage background;
        private UIImage check;
        public bool Checked;

        public UICheckBox(float x, float y, float size, bool _checked)
        {
            Position = new Vector3(x, y, 1f);
            Width = size / Program.Window.AspectRatio;
            Height = size;
            Checked = _checked;

            background = new UIImage(x, y, Width, size, GUI.Textures["TextBox"], false);
            check = new UIImage(x, y, Width - ((Width / 7.5f) * 2f), size - ((size / 7.5f) * 2f), GUI.Textures["Button"], false);

            Move();
        }

        public void Move()
        {
            background.Position = Position;
            check.Position = new Vector3(Position.X + Width / 7.5f, Position.Y + Height / 7.5f, 1f);
        }

        public override void OnMouseDown(MouseButton button, bool onElement)
        {
            if (onElement)
                Checked = !Checked;
        }

        public override void Render(Shader s)
        {
            background.Render(s);
            if (Checked)
                check.Render(s);
        }
    }
}
