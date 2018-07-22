using System;
using System.Drawing;

namespace T2Tools.Formats
{
    class Palette
    {
        public static Bitmap ToBitmap(byte[] data)
        {
            Bitmap bmp = new Bitmap(8, 8);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    int i = x + y * bmp.Width;
                    byte r = (byte)Math.Floor(data[i] * 255f / 63f);
                    byte g = (byte)Math.Floor(data[i + 1] * 255f / 63f);
                    byte b = (byte)Math.Floor(data[i + 2] * 255f / 63f);

                    Color color = Color.FromArgb(255, r, g, b);
                    bmp.SetPixel(x, y, color);
                }
            }

            return bmp;
        }
    }
}
