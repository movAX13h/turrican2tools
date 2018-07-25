using System;
using System.IO;
using System.Text;

namespace T2Tools.Formats
{
    public class PCMFile
    {
        /// <summary>
        /// width of the level
        /// </summary>
        public int Width;
        /// <summary>
        /// height of the level
        /// </summary>
        public int Height;
        /// <summary>
        /// tile-index within the BLOCK?.PIC file
        /// </summary>
        public short[,] TilesIndices;

        /// <summary>
        /// unknown parameters, flags or settings
        /// </summary>
        public int Unk1, Unk2, Unk3, Unk4, Unk5, Unk6;

        public PCMFile(int width, int height)
        {
            Width = width;
            Height = height;
            TilesIndices = new short[Height, Width];
        }

        public PCMFile(string path) : this(File.ReadAllBytes(path))
        {}

        public PCMFile(byte[] data)
        {
            if(!Encoding.ASCII.GetString(data, 0, 12).Equals("Puzzle V2.0 "))
                throw new Exception("bad file");
            Unk1 = BitConverter.ToInt16(data, 12);
            Unk2 = BitConverter.ToInt16(data, 14);
            Unk3 = BitConverter.ToInt16(data, 16);
            Unk4 = BitConverter.ToInt16(data, 18);
            int widthBytes = BitConverter.ToInt16(data, 20);
            int height = BitConverter.ToInt16(data, 22);

            if(!Encoding.ASCII.GetString(data, 24, 4).Equals("MAP "))
                throw new Exception("bad file");

            Unk5 = BitConverter.ToInt16(data, 28);
            Unk6 = BitConverter.ToInt16(data, 30);

            Width = widthBytes / 2;
            Height = height;
            TilesIndices = new short[Height, Width];
            int ptr = data.Length - widthBytes * Height;
            for(int y = 0; y < Height; ++y)
            {
                for(int x = 0; x < Width; ++x)
                {
                    int v = BitConverter.ToInt16(data, ptr);
                    int index = v >> 2;
                    if(index >= 1000)
                        index = 0;
                    TilesIndices[y, x] = (short)index;
                    ptr += 2;
                }
            }
        }

        /// <summary>
        /// save a .PCM file
        /// </summary>
        public void Save(Stream f)
        {
            var b = new BinaryWriter(f);
            b.Write(Encoding.ASCII.GetBytes("Puzzle V2.0 "));
            b.Write((short)Unk1);
            b.Write((short)Unk2);
            b.Write((short)Unk3);
            b.Write((short)Unk4);
            b.Write((short)(Width * 2));
            b.Write((short)Height);
            b.Write(Encoding.ASCII.GetBytes("MAP "));
            b.Write((short)Unk5);
            b.Write((short)Unk6);
            for(int y = 0; y < Height; ++y)
                for(int x = 0; x < Width; ++x)
                    b.Write((short)(TilesIndices[y, x] * 4));
        }

        /// <summary>
        /// save a .PCM file
        /// </summary>
        public void Save(string path)
        {
            using(var f = File.OpenWrite(path))
            {
                Save(f);
            }
        }
    }
}
