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
                            var entry = collisionInfo.Entries[i];
                            if (entry.A > 0)
                            {
                                brush.Color = colors[entry.A];
                                gfx.FillRectangle(brush, 16 * x, 16 * y, 8, 8);
                            }

                            if (entry.B > 0)
                            {
                                brush.Color = colors[entry.B];
                                gfx.FillRectangle(brush, 16 * x + 8, 16 * y, 8, 8);
                            }

                            if (entry.C > 0)
                            {
                                brush.Color = colors[entry.C];
                                gfx.FillRectangle(brush, 16 * x, 16 * y + 8, 8, 8);
                            }

                            if (entry.D > 0)
                            {
                                brush.Color = colors[entry.D];
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

        private TOC assets;

        public TilemapMaker(TOC assets)
        {
            this.assets = assets;
        }

        public Bitmap MakeTilesetBitmap(TOCEntry entry, bool collisions) // entry can be COL or PIC
        {
            string picName, palName, colName;
            TOCEntry picEntry, palEntry, colEntry;

            // level number from BLOCK?.PIC or WORLD?.COL file
            if (!int.TryParse(entry.Name.Substring(5, 1), out int levelNumber))
                return null;
            if (levelNumber == 6) levelNumber = 5;

            if (entry.Type == TOCEntryType.CollisionInfo) // get matching block?.pic
            {
                colEntry = entry;
                picName = $"BLOCK{levelNumber}.PIC";
                if (!assets.Entries.ContainsKey(picName))
                    return null;
                picEntry = assets.Entries[picName];
            }
            else // get matching world?.col
            {
                picEntry = entry;

                // collision entry
                colName = $"WORLD{levelNumber}.COL";
                if (!assets.Entries.ContainsKey(colName))
                    return null;
                colEntry = assets.Entries[colName];
            }

            // palette entry
            palName = $"WORLD{levelNumber}.PAL";
            if (!assets.Entries.ContainsKey(palName))
                return null;
            palEntry = assets.Entries[palName];

            // read col data
            COLFile colFile = collisions ? new COLFile(colEntry.Data) : null;

            try
            {
                return FromBitmaps(PICConverter.PICToBitmaps(picEntry.Data, palEntry.Data), colFile);
            }
            catch { return null; }
        }

    }
}
