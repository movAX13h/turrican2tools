using System;
using System.Collections.Generic;
using System.Drawing;

namespace T2Tools.Formats
{
    public class BlockPicConverter
    {
        public static Bitmap[] BlockPicToBitmaps(byte[] blockPic, byte[] palette)
        {
            if(blockPic.Length != 256001)
                throw new Exception("blockpic data must be 256001 bytes long");
            if(palette.Length != 768)
                throw new Exception("blockpic data must be 768 bytes long");

            //      pixel order (EGA):
            //
            //       0   4   8  12   1   5   9  13   2   6  10  14   3   7  11  15
            //      16  20  24  28  17  21  25  29  18  22  26  30  19  23  27  31
            //      ...

            int numTilesPerBlockPic = 1000;
            List<Bitmap> bitmaps = new List<Bitmap>(numTilesPerBlockPic);
            int ptr = 0;
            for(int i = 0; i < numTilesPerBlockPic; ++i)
            {
                var bmp = new Bitmap(16, 16);
                for(int page = 0; page < 4; ++page)
                {
                    for(int y = 0; y < 16; ++y)
                    {
                        for(int x = 0; x < 4; ++x)
                        {
                            int k = blockPic[ptr++];
                            bmp.SetPixel(x * 4 + page, y, Color.FromArgb(
                                VGABitmapConverter.Convert6BitTo8Bit(palette[k * 3]),
                                VGABitmapConverter.Convert6BitTo8Bit(palette[k * 3 + 1]),
                                VGABitmapConverter.Convert6BitTo8Bit(palette[k * 3 + 2])));
                        }
                    }
                }
                bitmaps.Add(bmp);
            }
            return bitmaps.ToArray();
        }
    }
}
