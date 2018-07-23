using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2Tools.Formats
{
    public class VGABitmapConverter
    {
        public static int Convert6BitTo8Bit(int v)
        {
            return (v * 255 + 31) / 63;
            //return ((v & 1) != 0) ? v * 4 + 3 : v * 4;
        }
        public static Bitmap ToRGBA(VGABitmap vga)
        {
            var bmp = new Bitmap(vga.Width, vga.Height);

            for(int y = 0; y < vga.Height; ++y)
            {
                for(int x = 0; x < vga.Width; ++x)
                {
                    int k = vga.Data[x + y * vga.Width];
                    if(k != 0)
                        bmp.SetPixel(x, y, Color.FromArgb(Convert6BitTo8Bit(vga.Palette[k * 3]), Convert6BitTo8Bit(vga.Palette[k * 3 + 1]), Convert6BitTo8Bit(vga.Palette[k * 3 + 2])));
                }
            }
            return bmp;

        }
    }
}
