using System;
using System.Collections.Generic;
using System.IO;

namespace T2Tools.Formats
{
    public class EIBFile
    {
        public int D;
        public int E;
        public int F;
        public int G;
        public int H;
        public int I;
        public int J;

        public int Width, Height;
        public EIBRegion[,] Regions;

        public EIBFile(byte[] data)
        {
            int numRegionsXMinusOne = BitConverter.ToInt16(data, 0);
            int numRegionsYMinusOne = BitConverter.ToInt16(data, 2);
            int dataBlockSize = BitConverter.ToInt16(data, 4);
            D = BitConverter.ToInt16(data, 6);
            E = BitConverter.ToInt16(data, 8);
            F = BitConverter.ToInt16(data, 10);
            G = BitConverter.ToInt16(data, 12);
            H = BitConverter.ToInt16(data, 14);
            I = BitConverter.ToInt16(data, 16);
            J = BitConverter.ToInt16(data, 18);

            // taken from disassembly of WORLD1.EXE 
            // gives the number of bytes to load from the eib file (result is always: total length - 6)

            //mov ax, eib_second_2
            //inc ax
            int len = (numRegionsYMinusOne + 1);
            //mov dx, ax
            //mov ax, eib_first_2
            //inc ax
            //mul dx
            //mov dx, ax
            len *= (numRegionsXMinusOne + 1);
            //mov ax, eib_second_2
            //inc ax
            //add ax, dx
            //shl ax, 1
            len = 2 * (len + (numRegionsYMinusOne + 1));
            //add ax, [bp + eib_third]
            len += dataBlockSize;

            // len = 2 * ((A + 1) * (B + 1) + (B + 1)) + C
            // len = 2 * (a * b + b) + c
            // len = 2 * b * (a + 1) + c

            // the first loaded byte after the first 6 is always 0 in turrican 2
            // it is added to B and used as mem length for B: lenB = 2 * (B + 1) + D


            Width = numRegionsXMinusOne + 1;
            Height = numRegionsYMinusOne + 1;
            Regions = new EIBRegion[Height, Width];

            int tableLen = (Width + 1) * (Width + 1) * 2;
            int blockLoc = 6 + tableLen;

            blockLoc = data.Length - dataBlockSize;

            int offsetsLoc = blockLoc - Width * Height * 2;

            if(blockLoc + dataBlockSize != data.Length)
                throw new Exception("EIB bad size");

            int pos = blockLoc;

            if(false)
            {
                int numChunks = 0;

                while(pos < blockLoc + dataBlockSize)
                {
                    if(data[pos] == 0xFF)
                    {
                        ++pos;
                        Console.WriteLine();
                        ++numChunks;
                        continue;
                    }
                    Console.Write(data[pos] + " " + data[pos + 1] + " " + data[pos + 2] + ", ");
                    pos += 3;
                }
                Console.WriteLine(numChunks + " chunks read of " + Width * Height);
            }

            byte[] block = new byte[dataBlockSize];
            Array.Copy(data, blockLoc, block, 0, dataBlockSize);
            short[] offsets = new short[Width * Height];
            for(int i = 0; i < Width * Height; ++i)
                offsets[i] = BitConverter.ToInt16(data, offsetsLoc + i * 2);

            for(int i = 0; i < Height; ++i)
            {
                for(int j = 0; j < Width; ++j)
                {
                    int pos2 = offsets[i * Width + j];

                    if(pos2 >= block.Length)
                    {
                        Regions[i, j] = new EIBRegion { Points = new List<EIBPoint>() };
                        continue;
                    }

                    pos = pos2;
                    var region = new EIBRegion { Points = new List<EIBPoint>() };
                    for(; block[pos] != 0xFF; pos += 3)
                        region.Points.Add(new EIBPoint { ID = block[pos], LocalX = block[pos + 1], LocalY = block[pos + 2] });
                    if(block[pos] != 0xFF)
                        throw new Exception("expected 0xFF");
                    ++pos;
                    Regions[i, j] = region;
                }
            }
        }

        public EIBFile(string path) : this(File.ReadAllBytes(path))
        {}

        public void Save(string path)
        {
            using(var f = new BinaryWriter(File.OpenWrite(path)))
            {
                List<int> offsets = new List<int>();
                List<byte> block = new List<byte>();
                foreach(var region in Regions)
                {
                    offsets.Add(block.Count);
                    foreach(var point in region.Points)
                    {
                        block.Add((byte)point.ID);
                        block.Add((byte)point.LocalX);
                        block.Add((byte)point.LocalY);
                    }
                    block.Add(0xFF);
                }

                f.Write((short)(Width - 1));
                f.Write((short)(Height - 1));
                f.Write((short)block.Count);
                f.Write((short)D);
                f.Write((short)E);
                f.Write((short)F);
                f.Write((short)G);
                f.Write((short)H);
                f.Write((short)I);
                f.Write((short)J);

                foreach(var pointer in offsets)
                    f.Write((short)pointer);

                f.Write(block.ToArray());
            }
        }

        
    }

    public class EIBPoint
    {
        public int ID;
        public int LocalX;
        public int LocalY;
    }

    /// <summary>
    /// A square of 256x256 screen pixels.
    /// </summary>
    public class EIBRegion
    {
        public List<EIBPoint> Points;
    }
}
