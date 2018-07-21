using System.IO;
using System.Text.RegularExpressions;

namespace T2Tools.Turrican
{
    public enum TOCEntryType { Unknown, Text, Language, StaticSprite, AnimatedSprite,
        Picture, Bitmap, PixelFont, TextmodeFont, Palette,
        Music, Sound, Executable, Map, DAT, DIR }

    public class TOCEntry
    {
        public int Index;
        public string Name;
        public int Size;
        public int PackedStart, PackedEnd;
        public int _BATEnd;
        public byte[] Data;

        public TOCEntryType Type
        {
            get
            {
                switch(Path.GetExtension(Name).ToLower())
                {
                    case ".txt": return TOCEntryType.Text;
                    case ".lng": return TOCEntryType.Language;
                    case ".pcx": return TOCEntryType.StaticSprite;
                    case ".bob": return TOCEntryType.AnimatedSprite;
                    case ".pic": return TOCEntryType.Picture;
                    case ".raw": return TOCEntryType.Bitmap;
                    case ".fnt": return TOCEntryType.PixelFont;
                    case ".fon": return TOCEntryType.TextmodeFont;
                    case ".pal": return TOCEntryType.Palette;
                    case ".tfx": return TOCEntryType.Music;
                    case ".sam": return TOCEntryType.Sound;
                    case ".exe": return TOCEntryType.Executable;
                    case ".pcm": return TOCEntryType.Map;
                    case ".dat": return TOCEntryType.DAT;
                    case ".dir": return TOCEntryType.DIR;
                    default:     return TOCEntryType.Unknown;
                }                
            }
        }

        public string TypeString
        {
            get
            {
                return Regex.Replace(Type.ToString(), "(\\B[A-Z])", " $1");
            }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
