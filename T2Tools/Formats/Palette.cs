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
                    Color color = Color.FromArgb(255, 4 * data[i], 4 * data[i + 1], 4 * data[i + 2]);
                    bmp.SetPixel(x, y, color);
                }
            }

            return bmp;
        }
    }
}
