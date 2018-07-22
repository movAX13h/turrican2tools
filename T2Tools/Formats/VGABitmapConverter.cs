using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2Tools.Formats
{
    class VGABitmapConverter
    {
        public static Bitmap ToRGBA(VGABitmap vga)
        {
            var bmp = new Bitmap(vga.Width, vga.Height);

            for(int y = 0; y < vga.Height; ++y)
            {
                for(int x = 0; x < vga.Width; ++x)
                {
                    int k = vga.Data[x + y * vga.Width];
                    if(k != 0)
                        bmp.SetPixel(x, y, Color.FromArgb(vga.Palette[k * 3] * 4, vga.Palette[k * 3 + 1] * 4, vga.Palette[k * 3 + 2] * 4));
                }
            }
            return bmp;

        }
    }
}
