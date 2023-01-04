using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemPlus;

namespace BuildPlate_Editor.UI
{
    public class UISlider : GUIElement
    {
        private UISliceImage background;
        private UISliceImage slider;
        private UItext uiText;

        public float Value;
        private float minVal;
        private float maxVal;
        private int decimalPlaces;

        private bool holding = false;
        private float holdOffset;

        public UISlider(float x, float y, float width, float height, float textScale, Font font, float _minVal, float _maxVal, 
            float value, int _decimalPlaces)
        {
            Position = new OpenTK.Vector3(x, y, 1f);
            Width = width;
            Height = height;
            minVal = _minVal;
            maxVal = _maxVal;
            decimalPlaces = _decimalPlaces;

            background = new UISliceImage(x, y, width, height, 2, GUI.Textures["TextBox"], false);
            slider = new UISliceImage(map(value, minVal, maxVal, Position.X, (Position.X + Width) - height / 3f), y, height / 3f,
                height, 2, GUI.Textures["Button"], true);
            uiText = new UItext(value.ToString(), x, y, textScale, font);
            uiText.Position = new OpenTK.Vector3(x + width / 2 - uiText.Width / 2, y + height / 2 - uiText.Height / 2, 1f);
            uiText.SetPos();
        }

        public override void Update(float delta)
        {
            if (holding) {
                float mx = (float)Program.Window.mousePos.X / Program.Window.Width * 2f - 1f;

                float x = MathPlus.Clamp(mx + holdOffset, Position.X, Position.X + Width - slider.Width);
                slider.Position = new OpenTK.Vector3(x, slider.Position.Y, 1f);
                slider.Move();

                Value = map(x, Position.X, Position.X + Width - slider.Width, minVal, maxVal);
                Value = MathPlus.Clamp(Value, minVal, maxVal);
                uiText.Text = MathPlus.Round(Value, decimalPlaces).ToString().Replace(',', '.');
                uiText.UpdateMesh();
                uiText.Position = new OpenTK.Vector3(Position.X + Width / 2 - uiText.Width / 2, 
                    Position.Y + Height / 2 - uiText.Height / 2, 1f);
                uiText.SetPos();
            }
        }

        public override void OnMouseDown(MouseButton button, bool onElement)
        {
            if (onElement) {
                float x = (float)Program.Window.mousePos.X / Program.Window.Width * 2f - 1f;
                if (x >= slider.Position.X && x < slider.Position.X + slider.Width) {
                    holdOffset = slider.Position.X - x;
                    holding = true;
                }
            }
        }
        public override void OnMouseUp(MouseButton button, bool onElement)
        {
            holding = false;
        }

        private float lastVal = 0f;
        public override void Render(Shader s)
        {
            if (lastVal != Value) {
                lastVal = Value;
                slider.Position = new OpenTK.Vector3(map(Value, minVal, maxVal, Position.X, (Position.X + Width) - Height / 3f),
                    slider.Position.Y, 1f);
                slider.Move();
                uiText.Text = MathPlus.Round(Value, decimalPlaces).ToString().Replace(',', '.');
                uiText.UpdateMesh();
            }

            background.Render(s);
            slider.Render(s);
            uiText.Render(s);
        }

        private static float map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }
    }
}
