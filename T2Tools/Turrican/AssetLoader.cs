using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2Tools.Turrican
{
    class AssetLoader
    {
        public static TOC Load(byte[] exeData)
        {
            List<TOCEntry> entries = new List<TOCEntry>();

            // locate the packed TOC in the binary
            if(BitConverter.ToInt32(exeData, exeData.Length - 4) != 0x53464945) // "EIFS"
                throw new Exception("error loading toc");
            int packedTocLength = BitConverter.ToInt16(exeData, exeData.Length - 6);
            int packedTocPos = exeData.Length - 6 - packedTocLength;

            // unpack the TOC and parse it
            var tocBuffer = new byte[10000];
            int length = UnpackBlock(tocBuffer, exeData.SubArray(packedTocPos, packedTocLength));
            if(length < 1)
                throw new Exception("error unpacking toc");
            ParseToc(entries, tocBuffer);

            // unpack all files
            var buffer = new byte[1024];
            foreach(var entry in entries)
            {
                entry.Data = new byte[entry.Size];
                
                // keep reading packed blocks until we reach the end pointer
                // a block produces 1024 bytes of unpacked data at max!
                for(int readPos = entry.PackedStart, writePos = 0; readPos < entry.PackedEnd; )
                {
                    int blockLength = exeData[readPos] + exeData[readPos + 1] * 256;

                    var numBytes = UnpackBlock(buffer, exeData.SubArray(readPos, blockLength + 6));
                    Array.Copy(buffer, 0, entry.Data, writePos, numBytes);

                    writePos += numBytes;
                    readPos += blockLength + 2;
                }
            }

            var toc = new TOC();
            toc.Entries = new Dictionary<string, TOCEntry>();
            foreach(var entry in entries)
                toc.Entries.Add(entry.Name, entry);
            return toc;
        }

        public static TOC Load(string exePath)
        {
            return Load(File.ReadAllBytes(exePath));
        }

        static void ParseToc(List<TOCEntry> toc, byte[] data)
        {
            using(var f = new BinaryReader(new MemoryStream(data)))
            {
                while(f.BaseStream.Position < f.BaseStream.Length)
                {
                    var nameBytes = f.ReadBytes(12);
                    if(nameBytes[0] == 0)
                        break;
                    toc.Add(new TOCEntry
                    {
                        Name = Encoding.ASCII.GetString(nameBytes).Trim(),
                        Size = f.ReadInt32(),
                        PackedStart = f.ReadInt32(),
                        PackedEnd = f.ReadInt32()
                    });
                }
            }
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