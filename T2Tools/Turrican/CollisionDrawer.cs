using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T2Tools.Formats;

namespace T2Tools.Turrican
{
    class CollisionDrawer
    {
        internal static void Fill(Graphics gfx, COLFile collisionInfo, int tileId, int x, int y)
        {
            SolidBrush brush = new SolidBrush(Color.White);
            Dictionary<int, Color> colors = new Dictionary<int, Color>() {
                { 0x01, Color.FromArgb(200, 255, 255, 0) },
                { 0x80, Color.FromArgb(200, 255, 0, 255) },
                { 0x7F, Color.FromArgb(200, 0, 255, 255) },
                { 0xD3, Color.FromArgb(200, 0, 255, 0) },
                { 0x31, Color.FromArgb(200, 10, 100, 200) }
            };

            if (tileId < collisionInfo.Entries.Length)
            {
                var entry = collisionInfo.Entries[tileId];
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
            else // no collision info available for this tile
            {
                gfx.DrawLine(Pens.Red, 16 * x + 2, 16 * y + 2, 16 * x + 14, 16 * y + 14);
                gfx.DrawLine(Pens.Red, 16 * x + 14, 16 * y + 2, 16 * x + 2, 16 * y + 14);
            }
        }
    }
}
