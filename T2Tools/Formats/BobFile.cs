using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2Tools.Formats
{
    class BOBFile
    {
        public byte[] Palette;
        public List<BOBFrame> Frames;

        public BOBFile(string path) : this(File.ReadAllBytes(path))
        { }

        public BOBFile(byte[] data)
        {
            using(var f = new BinaryReader(new MemoryStream(data)))
            {
                Palette = f.ReadBytes(256 * 3);
                int one = f.ReadInt16(); // "1"
                if(one != 1)
                    throw new Exception("unsupported file type");
                int numFrames = f.ReadInt16();
                f.BaseStream.Position += 1;

                Frames = new List<BOBFrame>(numFrames);

                for(int i = 0; i < numFrames; ++i)
                {
                    int settings = f.ReadInt32();
                    int width = f.ReadInt16() + 1;
                    int height = f.ReadInt16() + 1;
                    int renderPgmSize = f.ReadInt16();
                    int pixelDataSize = f.ReadInt16();

                    int zero = f.ReadInt32(); // always "0"
                    if(zero != 0)
                        throw new Exception("format error");

                    var drawProgram = f.ReadBytes(renderPgmSize);
                    var pixelData = f.ReadBytes(pixelDataSize);

                    Frames.Add(new BOBFrame
                    {
                        DrawProgram = drawProgram,
                        PixelData = pixelData,
                        Settings = settings,
                        Width = width,
                        Height = height
                    });
                }

                // 16 zerobytes trail each bob file
            }
        }
    }

    class BOBFrame
    {
        /// <summary>
        /// a x86 program to draw the sprite, consisting of unrolled loops and pokes to the EGA card
        /// </summary>
        public byte[] DrawProgram;
        /// <summary>
        /// data that is accessed by the draw program
        /// </summary>
        public byte[] PixelData;
        /// <summary>
        /// yet unknown purpose
        /// </summary>
        public int Settings;

        public int Width;
        public int Height;
    }
}
