using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TetrisBot
{
    public class PixelCheck
    {
        public byte r, b, g, version;
        public string color;
        public PixelCheck(byte r, byte g, byte b, byte version, string color)
        {
            this.r = r; this.g = g; this.b = b; this.version = version; this.color = color;
        }
        public bool isEqual(Color p)
        {
            if (r == p.R && g == p.G && b == p.B)
                return true;
            return false;
        }

    }
}
