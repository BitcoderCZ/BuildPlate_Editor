using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor.UI
{
    public interface IGUIElement : IRenderObject
    {
        float Width { get; }
        float Height { get; }

        void Update(float delta);

        void OnMouseDown(MouseButton button, bool onElement);
        void OnMouseUp(MouseButton button, bool onElement);
        void OnKeyPress(char keyChar);
        void OnKeyDown(Key key, KeyModifiers modifiers);
        void OnKeyUp(Key key, KeyModifiers modifiers);
    }
}
