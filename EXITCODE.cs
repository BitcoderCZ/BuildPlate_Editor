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

        TexturesFile_Existance = 2, // doesn't exist
        TexturesFile_Path = 3,

        Failed_BuildPlate_Existance = 4, // doesn't exist
        Failed_BuildPlate_Extension = 5,
        Failed_BuildPlate_Parse = 6,

        World_Load_TextureArray = 7,
        World_ReLoad_TextureArray = 8,
        World_Render_Block = 10,
        World_Unknown = 20,
    }
}
