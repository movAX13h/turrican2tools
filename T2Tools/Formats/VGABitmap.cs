using System;
using System.Collections.Generic;
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


        #region various tests and helpers
        public int CountMaskedPixels()
        {
            int n = 0;
            for(int y = 0; y < Height; ++y)
            {
                for(int x = 0; x < Width; ++x)
                {
                    if(Data[x + y * Width] != 0)
                        ++n;
                }
            }
            return n;
        }
        public int CountColors()
        {
            HashSet<int> c = new HashSet<int>();
            for(int i = 0; i < 256; ++i)
                c.Add(Palette[i * 3] | Palette[i * 3 + 1] << 8 | Palette[i * 3 + 2] << 16);
            return c.Count;
        }
        public byte[] Test()
        {
            List<byte> d = new List<byte>();
            for(int y = 0; y < Height; ++y)
            {
                for(int x = 0; x < Width; ++x)
                {
                    byte k = Data[x + y * Width];
                    if(k != 0)
                        d.Add(k);
                }
            }
            return d.ToArray();
        }
        public byte[] Test2()
        {
            List<byte> d = new List<byte>();
            for(int i = 0; i < 4; ++i)
            {
                for(int y = 0; y < Height; ++y)
                {
                    for(int j = 0; j < Width; j += 4)
                    {
                        int x = j + i;
                        if(x >= Width)
                            continue;

                        byte k = Data[x + y * Width];
                        if(k != 0)
                            d.Add(k);
                    }
                }
            }
            return d.ToArray();
        }
        public void SaveExtra(string path)
        {
            int ups = 32;
            var bmp = new Bitmap(Width * ups, Height * ups);
            using(var g = Graphics.FromImage(bmp))
            {
                int index = 0;
                for(int j = 0; j < 4; ++j)
                {
                    for(int y = 0; y < Height; ++y)
                    {
                        for(int i = 0; i < Width; i += 4)
                        {
                            if(i + j >= Width)
                                continue;
                            int x = i + j;
                            int k = Data[y * Width + x];
                            if(k != 0)
                            {
                                g.FillRectangle(new SolidBrush(Color.FromArgb(Palette[k * 3] * 4, Palette[k * 3 + 1] * 4, Palette[k * 3 + 2] * 4)), x * ups, y * ups, ups, ups);

                                string text = k.ToString("X");
                                text = index.ToString();
                                g.DrawString(text, new Font("Consolas", 10), new SolidBrush(Color.FromArgb(255, 0, 0, 0)), x * ups, y * ups);


                                ++index;
                            }
                        }
                    }
                }
            }
            bmp.Save(path);
        }

        #endregion
    }

}