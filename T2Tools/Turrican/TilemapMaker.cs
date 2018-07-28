using System.Drawing;
using T2Tools.Formats;

namespace T2Tools.Turrican
{
    public class TilemapMaker
    {
        /// <summary>
        /// arrange the array of tiles into a single bitmap
        /// </summary>
        public static Bitmap FromBitmaps(Bitmap[] bitmaps, COLFile collisionInfo)
        {
            // 20x50 makes most sense for maps with 1000 tiles

            int num = bitmaps.Length;
            int w = 20;
            int h = (num / w) + 1;

            SolidBrush brush = new SolidBrush(Color.White);
            Color colorA = Color.FromArgb(200, 255, 255, 0);
            Color colorB = Color.FromArgb(200, 255, 0, 255);
            Color colorC = Color.FromArgb(200, 0, 255, 255);
            Color colorD = Color.FromArgb(200, 0, 255, 0);

            Bitmap bmp = new Bitmap(16 * w, 16 * h);
            using (var gfx = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < num; i++)
                {
                    int x = i % w;
                    int y = i / w;

                    gfx.DrawImage(bitmaps[i], 16 * x, 16 * y, 16, 16);

                    if (collisionInfo != null && i < collisionInfo.Entries.Length)
                    {
                        if (collisionInfo.Entries[i].A > 0)
                        {
                            brush.Color = colorA;
                            gfx.FillRectangle(brush, 16 * x, 16 * y, 8, 8);
                        }

                        if (collisionInfo.Entries[i].B > 0)
                        {
                            brush.Color = colorA;
                            gfx.FillRectangle(brush, 16 * x + 8, 16 * y, 8, 8);
                        }

                        if (collisionInfo.Entries[i].C > 0)
                        {
                            brush.Color = colorA;
                            gfx.FillRectangle(brush, 16 * x, 16 * y + 8, 8, 8);
                        }

                        if (collisionInfo.Entries[i].D > 0)
                        {
                            brush.Color = colorA;
                            gfx.FillRectangle(brush, 16 * x + 8, 16 * y + 8, 8, 8);
                        }
                    }
                }
            }

            return bmp;
        }
    }
}
