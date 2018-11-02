using System;
using System.Drawing;
using T2Tools.Formats;

namespace T2Tools.Turrican
{
    public class EntityDrawer
    {
        /// <summary>
        /// paint the positions into the given bitmap (full level, for instance)
        /// </summary>
        public static void Draw(Bitmap bmp, EIBFile eib)
        {
            using(var g = Graphics.FromImage(bmp))
            {
                var font = new Font("Arial", 10);
                var sfmnt = StringFormat.GenericDefault;
                sfmnt.Alignment = StringAlignment.Center;

                // draw a grid
                int bls = 256;
                for(var i = 0; i < eib.Height; ++i)
                    for(int j = 0; j < eib.Width; ++j)
                        g.DrawRectangle(new Pen(Color.AliceBlue), j * bls, i * bls, bls, bls);

                // draw points
                for(var i = 0; i < eib.Height; ++i)
                {
                    for(int j = 0; j < eib.Width; ++j)
                    {
                        var region = eib.Regions[i, j];
                        foreach(var point in region.Points)
                        {
                            var r = new Rectangle(point.LocalX * 8 + j * bls, point.LocalY * 8 + i * bls, 0, 0);
                            r.Inflate(8, 8);
                            var rnd = new Random(point.ID);
                            // 255, 0, 100
                            g.FillEllipse(new SolidBrush(Color.FromArgb(rnd.Next(256), rnd.Next(20), rnd.Next(100))), r);

                            g.DrawString(point.ID.ToString(), font, new SolidBrush(Color.White), r, sfmnt);
                        }
                    }

                }
            }
        }
    }
}
