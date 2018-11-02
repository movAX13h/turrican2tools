using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2Tools.Formats
{
    public class BOBDecoder
    {
        public List<VGABitmap> DecodeFrames(BOBFile bob)
        {
            List<VGABitmap> frames = new List<VGABitmap>(bob.Frames.Count);
            foreach(var frame in bob.Frames)
            {
                //System.IO.File.WriteAllBytes("dp.com", frame.DrawProgram);

                var cpu = new DrawProgramCPU { Text = frame.DrawProgram, Data = frame.PixelData };
                byte[] screenbuffer = new byte[frame.Width * frame.Height];
                //byte[] dest = new byte[5000];

                int page = -1;

                // this number was determined by experimentation
                // the draw-program appears to align lines of pixels along these boundaries
                // might have something to do with how EGA works, or simply a kink of the game
                int rowBytes = 86;

                cpu.Write += (s, e) =>
                {
                    //dest[e.Address] = (byte)e.Value;

                    int pageAddr = e.Address;
                    int x = (pageAddr % rowBytes) * 4;
                    int y = pageAddr / rowBytes;

                    switch(page)
                    {
                        case 0: break;
                        case 1: x -= 3; break;
                        case 2: x -= 6; break;
                        case 3: x -= 9; break;
                        default:
                            return;
                    }

                    if(x + y * frame.Width < screenbuffer.Length)
                        screenbuffer[x + y * frame.Width] = (byte)e.Value;
                };
                // the draw-program calls this at the beginning of every EGA page:
                cpu.EgaSequencerReset += (s, e) =>
                {
                    ++page;
                };
                cpu.Call(0); // call the draw-program

                var vgabmp = new VGABitmap(frame.Width, frame.Height, bob.Palette);
                vgabmp.Data = screenbuffer;
                frames.Add(vgabmp);
            }

            return frames;
        }
    }
}
