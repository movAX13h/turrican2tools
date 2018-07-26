using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2Tools.Formats
{
    public class TilemapMaker
    {
        /// <summary>
        /// arrange the array of tiles into a single bitmap
        /// </summary>
        public static Bitmap FromBitmaps(Bitmap[] bitmaps)
        {
            // 20x50 makes most sense for maps with 1000 tiles
            int blocksX = 20, blocksY = (bitmaps.Length + blocksX - 1) / blocksX;

            var bmp = new Bitmap(16 * blocksX, 16 * blocksY);
            using(var g = Graphics.FromImage(bmp))
            {
                for(int i = 0; i < blocksY; ++i)
                {
                    for(int j = 0; j < blocksX; ++j)
                    {
                        int a = j + i * blocksX;
                        if(a < bitmaps.Length)
                            g.DrawImage(bitmaps[a], new Rectangle(j * 16, i * 16, 16, 16));
                    }
                }
            }
            return bmp;
        }
    }
}
