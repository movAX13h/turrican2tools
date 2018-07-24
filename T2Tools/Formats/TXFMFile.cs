using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// specs from:
// ftp://ftp.modland.com/pub/documents/format_documentation/The%20Final%20Musicsystem%20eXtended%20Professional%20v2.0%20%28.mdat,%20.smpl%29.txt

namespace T2Tools.Formats
{
    public class TXFMFile
    {
        static string ReadString(BinaryReader f, int nc)
        {
            return Encoding.ASCII.GetString(f.ReadBytes(nc));
        }
        static int ReadWord(BinaryReader f)
        {
            var a = f.ReadBytes(2);
            Array.Reverse(a);
            return BitConverter.ToInt16(a, 0);
        }
        static int ReadInt(BinaryReader f)
        {
            var a = f.ReadBytes(4);
            Array.Reverse(a);
            return BitConverter.ToInt32(a, 0);
        }
        static short[] ReadWords(BinaryReader f, int n)
        {
            var a = new short[n];
            for(int i = 0; i < n; ++i)
                a[i] = (short)ReadWord(f);
            return a;
        }

        static int GetInt(byte[] data, int offset)
        {
            return data[offset] << 24 | data[offset + 1] << 16 | data[offset + 2] << 8 | data[offset + 3];
        }

        public string[] TextLines = new string[6];
        public short[] SongStartPositions;
        public short[] SongEndPoitions;
        public short[] TempoNumbers;

        public TXFMFile(string path)
        {
            using(var f = new BinaryReader(File.OpenRead(path)))
            {
                var magic = ReadString(f, 10);
                if(!magic.Equals("TFMX-SONG "))
                    throw new Exception("bad magic number");

                int unk1 = ReadWord(f);
                int unk2 = ReadInt(f);

                for(int i = 0; i < 6; ++i)
                    TextLines[i] = ReadString(f, 40);

                SongStartPositions = ReadWords(f, 32);
                SongEndPoitions = ReadWords(f, 32);
                TempoNumbers = ReadWords(f, 32);

                f.BaseStream.Position += 16;

                int trackstepPointer = ReadInt(f);
                int patternDataPointer = ReadInt(f);
                int macroDataPointer = ReadInt(f);

                if(trackstepPointer == 0 && patternDataPointer == 0 && macroDataPointer == 0)
                {
                    throw new NotImplementedException();

                    trackstepPointer = 0x600;
                    patternDataPointer = 0x200;
                    macroDataPointer = 0x400;
                }

            }


        }
    }
}
