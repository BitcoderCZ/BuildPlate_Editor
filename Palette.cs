using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    public class Palette
    {
        public int[] textures;
        public int data;
        public string name;

        public Palette(string _name, int _data, int[] _textures)
        {
            name = _name;
            data = _data;
            textures = _textures;
        }

        public Palette(string _name, int _data, int _texture) : this(_name, _data, new int[1] { _texture })
        { }
    }
}
