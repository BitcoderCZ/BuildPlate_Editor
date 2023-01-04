using BuildPlate_Editor.Maths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    [Serializable]
    public class BuildPlate
    {
        public List<Entity> entities;
        public int format_version;
        public List<SubChunk> sub_chunks;

        public class SubChunk
        {
            public List<PaletteBlock> block_palette;
            public List<int> blocks;
            public PositionInt position;
        }

        public class PaletteBlock
        {
            public int data;
            public string name;
        }

        public class Entity
        {
            public int changeColor;
            public int multiplicitiveTintChangeColor;
            public string name;
            public PositionDouble position;
            public PositionDouble rotation;
            public PositionDouble shadowPosition;
            public double shadowSize;
        }
        public class PositionDouble
        {
            public double x;
            public double y;
            public double z;
        }
        public class PositionInt
        {
            public int x;
            public int y;
            public int z;

            public static implicit operator Vector3i(PositionInt p) => new Vector3i(p.x, p.y, p.z);

            public override string ToString() => $"X: {x}, Y: {y}, Z: {z}";
        }

        public static BuildPlate Load(string path)
        {
            JsonBuildPlate jsonBuildPlate = Util.JsonDeserialize<JsonBuildPlate>(File.ReadAllText(path));
            string model = Util.Base64Decode(jsonBuildPlate.model);

            return Util.JsonDeserialize<BuildPlate>(model);
        }

        public static void Save(BuildPlate bp, string path, string originalPath)
        {
            JsonBuildPlate jsonBuildPlate = Util.JsonDeserialize<JsonBuildPlate>(File.ReadAllText(originalPath));
            jsonBuildPlate.model = Util.Base64Encode(Util.JsonSerialize(bp));

            File.WriteAllText(path, Util.JsonSerialize(jsonBuildPlate));
        }
    }
}
