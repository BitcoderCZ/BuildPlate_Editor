using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor.UI
{
    public interface IRenderObject
    {
        Vector3 Position { get; }
        bool Active { get; set; }

        void Render(Shader s);
    }
}
