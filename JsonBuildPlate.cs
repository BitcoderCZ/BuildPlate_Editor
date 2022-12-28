using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    [Serializable]
    public class JsonBuildPlate
    {
        public double blocksPerMeter { get; set; }
        public BuildPlateDimension dimension { get; set; }
        public string eTag { get; set; }
        public Guid id { get; set; }
        public string isModified { get; set; }
        public string lastUpdated { get; set; }
        public string locked { get; set; }
        public string model { get; set; }
        public int numberOfBlocks { get; set; }
        public BuildPlateOffset offset { get; set; }
        public int order { get; set; }
        public int requiredLevel { get; set; }
        public string surfaceOrientation { get; set; }
        public Guid templateId { get; set; }
        public string type { get; set; }
    }
}
