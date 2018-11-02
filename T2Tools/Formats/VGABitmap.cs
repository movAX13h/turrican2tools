using System;
using System.Drawing;

namespace T2Tools.Formats
{
    /// <summary>
    /// a palette bitmap with 6-bit colors
    /// </summary>
    public class VGABitmap
    {
        public int Width, Height;
        public byte[] Data;
        public byte[] Palette;

        public void SetFromBitmap(Bitmap src)
        {
            Width = src.Width;
            Height = src.Height;
            Data = new byte[src.Width * src.Height];
            for(int y = 0; y < src.Height; ++y)
            {
                for(int x = 0; x < src.Width; ++x)
                {
                    var rgb = src.GetPixel(x, y);
                    int r = rgb.R / 4;
                    int g = rgb.G / 4;
                    int b = rgb.B / 4;
                    if(rgb.A == 0)
                        continue;

                    int k = -1;
                    for(int i = 0; i < 256; ++i)
                    {
                        if(Palette[i * 3] == r && Palette[i * 3 + 1] == g && Palette[i * 3 + 2] == b)
                        {
                            k = i;
                            break;
                        }
                    }

                    if(k == -1)
                        throw new Exception("color not found");

                    Data[x + y * src.Width] = (byte)k;
                }
            }
        }

        public VGABitmap(int width, int height, byte[] palette)
        {
            Palette = (byte[])palette.Clone();
            Width = width;
            Height = height;
            Data = new byte[Width * Height];
        }

        public VGABitmap(Bitmap src, byte[] palette)
        {
            Palette = palette;
            SetFromBitmap(src);
        }

    }

}