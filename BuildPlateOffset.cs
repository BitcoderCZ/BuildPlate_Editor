using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    [Serializable]
    public class BuildPlateOffset
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public static implicit operator Vector3(BuildPlateOffset b) => new Vector3((float)b.x, (float)b.y, (float)b.z);
    }
}
