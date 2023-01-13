using BuildPlate_Editor.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    public struct RaycastResult
    {
        public Vector3i HitPos;
        public Vector3i LastPos;

        public RaycastResult(Vector3i hitPos, Vector3i lastPos)
        {
            HitPos = hitPos;
            LastPos = lastPos;
        }

        public RaycastResult(Vector3i hitPos) : this(hitPos, hitPos)
        { }
    }
}
