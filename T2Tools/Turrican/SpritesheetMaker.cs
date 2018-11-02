using System;
using System.Collections.Generic;
using System.Drawing;
using T2Tools.Formats;

namespace T2Tools.Turrican
{
    class SpritesheetMaker
    {
        public static Bitmap Make(Bitmap[] frames, int spacingX = 0, int firstFrame = 0, int numFrames = int.MaxValue, List<Rectangle> frameRectangles = null)
        {
            int end = Math.Min(frames.Length, firstFrame + numFrames);
            int ww = 0, hh = 0;
            for (int i = firstFrame; i < end; ++i)
            {
                var frame = frames[i];
                ww += frame.Width + (i != 0 ? spacingX : 0);
                hh = Math.Max(frame.Height, hh);
            }

            var bmp = new Bitmap(ww, hh);
            using (var g = Graphics.FromImage(bmp))
            {
                int x = 0;
                for (int i = firstFrame; i < end; ++i)
                {
                    var rgbbmp = frames[i];
                    var r = new Rectangle(x, hh - rgbbmp.Height, rgbbmp.Width, rgbbmp.Height);
                    g.DrawImage(rgbbmp, r);
                    frameRectangles?.Add(r);
                    x += rgbbmp.Width + spacingX;
                }
            }
            return bmp;
        }

        public static Bitmap Make(VGABitmap[] frames, int spacingX = 0, int firstFrame = 0, int numFrames = int.MaxValue, List<Rectangle> frameRectangles = null)
        {
            Bitmap[] bitmaps = new Bitmap[frames.Length];
            for(int i = 0; i < frames.Length; i++) bitmaps[i] = VGABitmapConverter.ToRGBA(frames[i]);
            return Make(bitmaps, spacingX, firstFrame, numFrames, frameRectangles);
        }
    }
}
