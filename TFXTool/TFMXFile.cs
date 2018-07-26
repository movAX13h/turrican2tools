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
    public class TFXFile
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
        static ushort[] ReadWords(BinaryReader f, int n)
        {
            var a = new ushort[n];
            for(int i = 0; i < n; ++i)
                a[i] = (ushort)ReadWord(f);
            return a;
        }
        static int[] ReadInts(BinaryReader f, int n)
        {
            var a = new int[n];
            for(int i = 0; i < n; ++i)
                a[i] = ReadInt(f);
            return a;
        }

        static int GetInt(byte[] data, int offset)
        {
            return data[offset] << 24 | data[offset + 1] << 16 | data[offset + 2] << 8 | data[offset + 3];
        }




        public string[] TextLines = new string[6];

        public ushort[] SongStartPositions;

        public ushort[] SongEndPoitions;

        /// <summary>
        /// If the tempo number is greater than 15, it is used as a beats-per-minute figure,
        /// with a beat taking 24 jiffies.  If not, then it is used as a divide-by value into
        /// a frequency of 50Hz.  (0=50Hz, 1=25Hz, 2=16.7 Hz...)
        /// </summary>
        public ushort[] TempoNumbers;

        /// <summary>
        /// The trackstep contains all the sequencing information as far as
        /// which patterns get started when.  It is an array of 8 word records,
        /// one for each track.  The high byte of each word contains the pattern
        /// number, which will be (musically) transposed by the two's-complement value in
        /// the least significant byte; or $80 if the last position is to be
        /// held (transpose is set to the least sig.  byte as above); or $FF
        /// if the channel is to stop running; or $FE to stop the voice indicated
        /// in the least significant byte of the command.
        /// 
        /// When the first word of a line is $EFFE, no track data is loaded.  At that
        /// point, the entire line is used as a command.  The word after $EFFE is used
        /// to select a command, and any remaining words are used as parameters to the
        /// command.
        /// </summary>
        public ushort[][] Tracksteps;
        public int NumTracksteps;

        public List<TFXPattern> Patterns;
        public List<TFXMacro> Macros;

        public TFXFile()
        {
            Patterns = new List<TFXPattern>();
            Macros = new List<TFXMacro>();
            SongStartPositions = new ushort[32];
            SongEndPoitions = new ushort[32];
            TempoNumbers = new ushort[32];
            NumTracksteps = 1;
            Tracksteps = new ushort[1][];
            Tracksteps[0] = new ushort[8] { 0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00 };
            TextLines = new string[6] { "", "", "", "", "", "" };
        }

        public TFXFile(string path)
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

                // try to find out how many tracksteps there are:
                NumTracksteps = 0;
                for(int i = 0; i < 32; ++i)
                {
                    if(SongEndPoitions[i] > SongStartPositions[i])
                        NumTracksteps = Math.Max(NumTracksteps, 1 + SongEndPoitions[i]);
                }

                Tracksteps = new ushort[NumTracksteps][];
                f.BaseStream.Position = trackstepPointer;
                for(var i = 0; i < NumTracksteps; ++i)
                    Tracksteps[i] = ReadWords(f, 8);

                var at = f.BaseStream.Position;

                // try to find out how many patterns there are:
                int numPatterns = 0;
                for(int i = 0; i < NumTracksteps; ++i)
                {
                    if(Tracksteps[i][0] == 0xEFFE) // it's a command
                        continue;

                    for(int j = 0; j < 8; ++j)
                    {
                        int patternIndex = Tracksteps[i][j] >> 8;
                        if(patternIndex < 128)
                            numPatterns = Math.Max(numPatterns, patternIndex + 1);
                    }
                }

                // TITLE.TFX
                // suggests 103 patterns, but appears to contain 151 pattern pointers

                f.BaseStream.Position = patternDataPointer;
                var patternPointers = ReadInts(f, numPatterns);

                int numMacros = 0;

                Patterns = new List<TFXPattern>(numPatterns);
                for(int i = 0; i < numPatterns; ++i)
                {
                    var pattern = new TFXPattern { Steps = new List<TFXPatternCommand>() };
                    f.BaseStream.Position = patternPointers[i];
                    while(true)
                    {
                        int cmd = ReadInt(f);
                        pattern.Steps.Add(new TFXPatternCommand(cmd));
                        if((cmd & 0xFF000000) == 0xF0000000) // "pattern end"
                            break;

                        if(((cmd >> 30) & 0x3) < 3)
                        {
                            int macro = (cmd >> 16) & 0xFF;

                            numMacros = Math.Max(numMacros, macro + 1);
                        }
                    }
                    Patterns.Add(pattern);
                }

                f.BaseStream.Position = macroDataPointer;
                var macroPointers = ReadInts(f, numMacros);

                Macros = new List<TFXMacro>(numMacros);
                for(int i = 0; i < numMacros; ++i)
                {
                    var macro = new TFXMacro { Steps = new List<TFXMacroCommand>() };
                    f.BaseStream.Position = macroPointers[i];
                    while(true)
                    {
                        int cmd = ReadInt(f);
                        macro.Steps.Add(new TFXMacroCommand(cmd));
                        if(((cmd >> 24) & 0xFF) == 7)
                            break;
                    }
                    Macros.Add(macro);
                }



                /*for(int i = 0; i + 1 < patternPointers.Length; ++i)
                {
                    Console.WriteLine(patternPointers[i] + "\t" + (patternPointers[i + 1] - patternPointers[i]));
                }*/

                at = f.BaseStream.Position;

            }


        }


        static void WriteWord(BinaryWriter f, int v)
        {
            f.Write((byte)(v >> 8));
            f.Write((byte)v);
        }
        static void WriteInt(BinaryWriter f, int v)
        {
            f.Write((byte)(v >> 24));
            f.Write((byte)(v >> 16));
            f.Write((byte)(v >> 8));
            f.Write((byte)v);
        }

        public void Save(string path)
        {
            using(var f = new BinaryWriter(File.OpenWrite(path)))
            {
                f.BaseStream.Position = 0x200;

                int trackstepAddr = (int)f.BaseStream.Position;
                for(int i = 0; i < NumTracksteps; ++i)
                    for(int j = 0; j < 8; ++j)
                        WriteWord(f, Tracksteps[i][j]);

                List<int> pointers = new List<int>(Patterns.Count);
                foreach(var pattern in Patterns)
                {
                    pointers.Add((int)f.BaseStream.Position);
                    foreach(var step in pattern.Steps)
                        WriteInt(f, step.Data);
                }
                int patternPointersAddr = (int)f.BaseStream.Position;
                foreach(var pointer in pointers)
                    WriteInt(f, pointer);

                pointers = new List<int>(Macros.Count);
                foreach(var macro in Macros)
                {
                    pointers.Add((int)f.BaseStream.Position);
                    foreach(var step in macro.Steps)
                        WriteInt(f, step.Data);
                }
                int macroPointersAddr = (int)f.BaseStream.Position;
                foreach(var pointer in pointers)
                    WriteInt(f, pointer);

                var fileLength = f.BaseStream.Position;

                // now write the header
                f.BaseStream.Position = 0;
                f.Write(Encoding.ASCII.GetBytes("TFMX-SONG "));
                WriteWord(f, 0);
                WriteInt(f, 0);
                for(int i = 0; i < 6; ++i)
                    f.Write(Encoding.ASCII.GetBytes(TextLines[i].PadRight(40, ' ')));
                foreach(var w in SongStartPositions)
                    WriteWord(f, w);
                foreach(var w in SongEndPoitions)
                    WriteWord(f, w);
                foreach(var w in TempoNumbers)
                    WriteWord(f, w);

                f.BaseStream.Position += 16;

                WriteInt(f, trackstepAddr);
                WriteInt(f, patternPointersAddr);
                WriteInt(f, macroPointersAddr);


                f.BaseStream.SetLength(fileLength);
            }
        }
    }

    public class TFXPattern
    {
        public string Name;
        public List<TFXPatternCommand> Steps;
    }
    public class TFXMacro
    {
        public string Name;
        public List<TFXMacroCommand> Steps;
    }

    public class TFXPatternCommand
    {
        public int Data;
        public TFXPatternCommand(uint data)
        {
            Data = (int)data;
        }
        public TFXPatternCommand(int data)
        {
            Data = data;
        }
        public override string ToString()
        {
            return Data.ToString("X8");
        }

        public int Note
        {
            get => (Data >> 24) & 0xFF;
            set
            {
                Data &= 0xFFFFFF;
                Data |= value << 24;
            }
        }
        public int Macro
        {
            get => (Data >> 16) & 0xFF;
            set
            {
                Data = (int)(Data & 0xFF00FFFF);
                Data |= (value & 0xFF) << 16;
            }
        }
        public int Volume
        {
            get => (Data >> 12) & 0xF;
            set
            {
                Data = (int)(Data & 0xFFFF0FFF);
                Data |= (value & 0xF) << 12;
            }
        }
        public int Channel
        {
            get => (Data >> 8) & 0xF;
            set
            {
                Data = (int)(Data & 0xFFFFF0FF);
                Data |= (value & 0xF) << 8;
            }
        }
        public int Detune
        {
            get => Data & 0xFF;
            set
            {
                Data = (int)(Data & 0xFFFFFF00);
                Data |= value & 0xFF;
            }
        }
    }

    public class TFXMacroCommand
    {
        public int Data;
        public TFXMacroCommand(int data)
        {
            Data = data;
        }
        public override string ToString()
        {
            return Data.ToString("X8");
        }

        public int Type
        {
            get => (Data >> 24) & 0xFF;
            set
            {
                Data &= 0xFFFFFF;
                Data |= value << 24;
            }
        }
        public int Parameter
        {
            get => Data & 0xFFFFFF;
            set
            {
                Data = (int)(Data & 0xFF000000);
                Data |= value & 0xFFFFFF;
            }
        }
        public int B => (Data >> 16) & 0xFF;
        public int C => (Data >> 8) & 0xFF;
        public int D => Data & 0xFF;
    }
}
