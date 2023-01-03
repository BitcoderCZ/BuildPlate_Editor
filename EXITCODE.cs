using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    public enum EXITCODE : int
    {
        Normal = 0,
        OpenGL_LowVersion = 1,
        World_Load_TextureArray = 2,
        World_Render_Block = 3,
        World_Unknown = 4,
    }
}
