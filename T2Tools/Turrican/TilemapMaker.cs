using System.Collections.Generic;
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
            Dictionary<int, Color> colors = new Dictionary<int,Color>() {
                { 0x01, Color.FromArgb(200, 255, 255, 0) },
                { 0x80, Color.FromArgb(200, 255, 0, 255) },
                { 0x7F, Color.FromArgb(200, 0, 255, 255) },
                { 0xD3, Color.FromArgb(200, 0, 255, 0) },
                { 0x31, Color.FromArgb(200, 10, 100, 200) }
            };

            Bitmap bmp = new Bitmap(16 * w, 16 * h);
            using (var gfx = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < num; i++)
                {
                    int x = i % w;
                    int y = i / w;

                    gfx.DrawImage(bitmaps[i], 16 * x, 16 * y, 16, 16);

                    if (collisionInfo != null)
                    {
                        if (i < collisionInfo.Entries.Length)
                        {
                            if (collisionInfo.Entries[i].A > 0)
                            {
                                brush.Color = colors[collisionInfo.Entries[i].A];
                                gfx.FillRectangle(brush, 16 * x, 16 * y, 8, 8);
                            }

                            if (collisionInfo.Entries[i].B > 0)
                            {
                                brush.Color = colors[collisionInfo.Entries[i].B];
                                gfx.FillRectangle(brush, 16 * x + 8, 16 * y, 8, 8);
                            }

                            if (collisionInfo.Entries[i].C > 0)
                            {
                                brush.Color = colors[collisionInfo.Entries[i].C];
                                gfx.FillRectangle(brush, 16 * x, 16 * y + 8, 8, 8);
                            }

                            if (collisionInfo.Entries[i].D > 0)
                            {
                                brush.Color = colors[collisionInfo.Entries[i].D];
                                gfx.FillRectangle(brush, 16 * x + 8, 16 * y + 8, 8, 8);
                            }
                        }
                        else
                        {
                            gfx.DrawLine(Pens.Red, 16 * x + 2, 16 * y + 2, 16 * x + 14, 16 * y + 14);
                            gfx.DrawLine(Pens.Red, 16 * x + 14, 16 * y + 2, 16 * x + 2, 16 * y + 14);
                        }
                    }
                }
            }

            return bmp;
        }
    }
}
