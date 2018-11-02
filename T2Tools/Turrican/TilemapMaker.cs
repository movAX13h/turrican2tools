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
                        CollisionDrawer.Draw(gfx, collisionInfo, i, x, y);
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

            if (entry.Type == TOCEntryType.CollisionInfo) // get matching block?.pic
            {
                colEntry = entry;
                picName = $"BLOCK{levelNumber}.PIC";
                if (!assets.Entries.ContainsKey(picName)) return null;
                picEntry = assets.Entries[picName];
            }
            else // get matching world?.col
            {
                if (levelNumber == 6) levelNumber = 5; 

                picEntry = entry;

                // collision entry
                colName = $"WORLD{levelNumber}.COL";
                if (!assets.Entries.ContainsKey(colName)) return null;
                colEntry = assets.Entries[colName];
            }

            if (levelNumber == 6) levelNumber = 5;

            // palette entry
            palName = $"WORLD{levelNumber}.PAL";
            if (!assets.Entries.ContainsKey(palName)) return null;
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
