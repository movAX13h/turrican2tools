using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2Tools.Turrican
{
    public class AssetLoader
    {
        public static TOC Load(byte[] exeData, bool gapsAsEntries = false)
        {
            List<TOCEntry> entries = new List<TOCEntry>();

            // locate the packed TOC in the binary
            if(BitConverter.ToInt32(exeData, exeData.Length - 4) != 0x53464945) // "EIFS"
                throw new Exception("error loading toc");
            int packedTocLength = BitConverter.ToUInt16(exeData, exeData.Length - 6);
            int packedTocPos = exeData.Length - 6 - packedTocLength;

            // unpack the TOC and parse it
            var tocBuffer = new byte[10000];
            int length = UnpackBlock(tocBuffer, exeData.SubArray(packedTocPos, packedTocLength));
            if(length < 1)
                throw new Exception("error unpacking toc");
            ParseToc(entries, tocBuffer);

            //var fff = new StreamWriter("debug.txt");
            //int numGood = 0;

            // unpack all files
            var buffer = new byte[1024];
            foreach(var entry in entries)
            {
                entry.Data = new byte[entry.Size];

                List<BlockInfo> blockInfos = new List<BlockInfo>();

                // keep reading packed blocks until we reach the end pointer
                // a block produces 1024 bytes of unpacked data at max!
                int readPos = entry.PackedStart;
                int writePos = 0;
                int numStreamRead = 0;
                for(; readPos < entry.PackedEnd; )
                {
                    int blockLength = exeData[readPos] + exeData[readPos + 1] * 256;

                    blockInfos.Add(new BlockInfo { Position = readPos, Length = blockLength });

                    var numBytes = UnpackBlock(buffer, exeData.SubArray(readPos, blockLength + 6));
                    Array.Copy(buffer, 0, entry.Data, writePos, numBytes);

                    numStreamRead += blockLength;
                    writePos += numBytes;
                    readPos += blockLength + 2;
                }
                blockInfos.Add(new BlockInfo { Position = readPos, Length = 0 });

                if(writePos != entry.Size)
                    throw new Exception("unpacking error");

                // verify the block-address table:
                for(int i = 0; i < blockInfos.Count; ++i)
                {
                    int address = BitConverter.ToInt32(exeData, readPos);
                    if(address != blockInfos[i].Position)
                        throw new Exception("bad BAT entry");
                    readPos += 4;
                }

                entry._BATEnd = readPos;

                // int length2 = BitConverter.ToUInt16(exeData, readPos); readPos += 2;
                //if(BitConverter.ToUInt32(exeData, readPos) != 0x53464945) // "EIFS"
                //    throw new Exception("bad magic id");
            }

            //fff.WriteLine(numGood + " good out of " + entries.Count);

            //fff.Flush();

            if(gapsAsEntries)
            {
                List<TOCEntry> tmp = new List<TOCEntry>(entries);
                tmp.Sort((a, b) => a.PackedStart.CompareTo(b.PackedStart));

                for(int i = 0; i + 1 < tmp.Count; ++i)
                {
                    var gap = new TOCEntry();
                    gap.Name = tmp[i].Name + ".gap";
                    gap.Index = tmp[i].Index;
                    gap.PackedStart = tmp[i]._BATEnd;
                    gap.PackedEnd = tmp[i + 1].PackedStart;

                    gap.Data = exeData.SubArray(tmp[i]._BATEnd, tmp[i + 1].PackedStart - tmp[i]._BATEnd);
                    gap.Size = gap.Data.Length;

                    entries.Add(gap);
                }
            }

            var toc = new TOC();
            toc.Entries = new Dictionary<string, TOCEntry>();
            foreach(var entry in entries)
                toc.Entries.Add(entry.Name, entry);
            return toc;
        }

        struct BlockInfo
        {
            public int Position;
            public int Length;
            public override string ToString()
            {
                return Position.ToString();
            }
        }

        public static TOC Load(string exePath, bool gapsAsEntries = false)
        {
            return Load(File.ReadAllBytes(exePath), gapsAsEntries);
        }

        static void ParseToc(List<TOCEntry> toc, byte[] data)
        {
            using(var f = new BinaryReader(new MemoryStream(data)))
            {
                int index = 0;
                while(f.BaseStream.Position < f.BaseStream.Length)
                {
                    var nameBytes = f.ReadBytes(12);
                    if(nameBytes[0] == 0)
                        break;
                    toc.Add(new TOCEntry
                    {
                        Index = index++,
                        Name = Encoding.ASCII.GetString(nameBytes).Trim(),
                        Size = f.ReadInt32(),
                        PackedStart = f.ReadInt32(),
                        PackedEnd = f.ReadInt32()
                    });
                }
            }
        }

        static byte[] GenerateToc(TOCEntry[] entries)
        {
            byte[] data = new byte[7200];
            var f = new BinaryWriter(new MemoryStream(data));
            for(int i = 0; i < entries.Length; ++i)
            {
                var entry = entries[i];

                if(entry.Name.Length > 12)
                    throw new Exception("filename must not be more than 12 characters");

                f.Write(Encoding.ASCII.GetBytes(entry.Name.PadRight(12, ' ')));
                f.Write(entry.Size);
                f.Write(entry.PackedStart);
                f.Write(entry.PackedEnd);
            }
            return data;
        }

        public static byte[] GenerateEXE(byte[] startupProgramData, TOC assets)
        {
            if(startupProgramData.Length != 12832)
                throw new Exception("startup program must be 12832 bytes long");

            List<TOCEntry> entries = new List<TOCEntry>();
            foreach(var asset in assets.Entries)
                if(!asset.Value.Name.EndsWith(".gap"))
                    entries.Add(asset.Value);
            entries.Sort((a, b) => a.Index.CompareTo(b.Index));

            var f = new BinaryWriter(new MemoryStream());
            f.Write(startupProgramData);

            foreach(var entry in entries)
            {
                entry.PackedStart = (int)f.BaseStream.Position;
                var bat = PackFile2(f, entry.Data, 1024);
                entry.PackedEnd = (int)f.BaseStream.Position;

                foreach(var batEntry in bat)
                    f.Write(batEntry);
            }

            PackFile(f, GenerateToc(entries.ToArray()), int.MaxValue);
            
            var buffer = ((MemoryStream)f.BaseStream).GetBuffer();
            Array.Resize(ref buffer, (int)f.BaseStream.Position);
            return buffer;
        }

        static void PackFile(BinaryWriter f, byte[] data, int blockSize)
        {
            var start = f.BaseStream.Position;
            for(int pos = 0; pos < data.Length; )
            {
                int plainBlockSize = Math.Min(blockSize, data.Length - pos);

                PackBlock(f, data, pos, plainBlockSize);

                pos += plainBlockSize;
            }
            f.Write((ushort)(f.BaseStream.Position - start));
            f.Write(0x53464945); // "EIFS"
        }

        static List<int> PackFile2(BinaryWriter f, byte[] data, int blockSize)
        {
            List<int> bat = new List<int>();
            var start = f.BaseStream.Position;
            for(int pos = 0; pos < data.Length;)
            {
                int plainBlockSize = Math.Min(blockSize, data.Length - pos);
                bat.Add((int)f.BaseStream.Position);
                PackBlock(f, data, pos, plainBlockSize);

                pos += plainBlockSize;
            }
            bat.Add((int)f.BaseStream.Position);
            return bat;
        }

        static void PackBlock(BinaryWriter f, byte[] data, int offset, int length)
        {
            f.Write((ushort)(length + 1));
            f.Write((byte)128); // select uncompressed stream
            for(var i = 0; i < length; ++i)
                f.Write((byte)(data[i + offset] ^ 0x6B));
        }



        /// <summary>
        /// The unpacking function extracted from T2.EXE assembly.
        /// </summary>
        /// <param name="unpacked">output buffer</param>
        /// <param name="packed">input buffer</param>
        /// <param name="offset">(original parameter, for due diligence)</param>
        /// <returns>number of bytes written to the output buffer</returns>
        static int UnpackBlock(byte[] unpacked, byte[] packed, ushort offset = 0)
        {
            int inputLength = packed[offset] + packed[offset + 1] * 256;

            int bitStorage = packed[offset + 3] * 256 + packed[offset + 4];
            offset += 2;


            if(packed[offset] == 128) // select decoder
            {
                // simple XOR-cipher decoder:
                for(int i = 1, w = 0, r = offset + 1; i < inputLength; ++w, ++r, ++i)
                {
                    unpacked[w] = (byte)(packed[offset + -2 + r] ^ 0x6B);
                }
                return inputLength - 1;
            }
            else
            {
                // LZ77 variant with XOR-cipher:

                int readPos = 3;
                int writePos = 0;

                for(int bitCounter = 16; readPos < inputLength; bitStorage <<= 1, bitCounter--)
                {
                    if(bitCounter == 0)
                    {
                        bitStorage = packed[offset + readPos] * 256 + packed[offset + readPos + 1];
                        readPos += 2;
                        bitCounter = 16;
                    }

                    if((bitStorage & 0x8000) == 0)
                    {
                        // copy the next data-byte from input to output (and decipher):

                        unpacked[writePos] = (byte)(packed[offset + readPos] ^ 0x6B);
                        readPos++;
                        writePos++;
                    }
                    else
                    {
                        int cmd1 = packed[offset + readPos] << 4;
                        int cmd2 = packed[offset + readPos + 1];
                        cmd1 += cmd2 >> 4;

                        readPos++;

                        if(cmd1 == 0)
                        {
                            // repeat an input byte in the output, a given number of times (and deciphering it)

                            int cnt = cmd2 * 256 + (packed[offset + readPos + 1] + 16);
                            readPos += 2;

                            int data = packed[offset + readPos] ^ 0x6B;
                            for(int i = 0; i < cnt; ++i)
                            {
                                unpacked[writePos + i] = (byte)data;
                            }

                            readPos++;
                            writePos += cnt;
                        }
                        else
                        {
                            // copy an already unpacked string, within the output buffer:
                            int numCopy = (packed[offset + readPos] & 0xF) + 3;
                            readPos++;
                            cmd1 = writePos - cmd1;
                            for(int i = 0, k = writePos; i < numCopy; ++cmd1, ++k, ++i)
                            {
                                unpacked[k] = unpacked[cmd1];
                            }
                            writePos += numCopy;
                        }
                    }
                }

                return writePos; // return the number of bytes that were written to the output
            }



        }

    }
}