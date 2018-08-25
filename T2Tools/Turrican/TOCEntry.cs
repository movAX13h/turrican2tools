using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace T2Tools.Turrican
{
    public enum TOCEntryType { Unknown, Gap, Text, Language, StaticSprite, AnimatedSprite,
        Tileset, Bitmap, PixelFont, TextmodeFont, Palette, EntitiesList,
        Music, Sound, Executable, Map, DAT, DIR, CollisionInfo, BossSprite }

    public class TOCEntry
    {
        public int Index;
        public string Name;
        public int Size;
        public int PackedStart, PackedEnd;
        public int _BATEnd;
        public byte[] Data;
        public bool Dirty = false;

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
                    case ".pic": return TOCEntryType.Tileset;
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
                    case ".gap": return TOCEntryType.Gap;
                    case ".eib": return TOCEntryType.EntitiesList;
                    case ".col": return TOCEntryType.CollisionInfo;
                    case ".mc":  return TOCEntryType.BossSprite;
                    default:     return TOCEntryType.Unknown;
                }                
            }
        }

        public Color Color
        {
            get
            {
                switch(Type)
                {
                    case TOCEntryType.AnimatedSprite: return Color.Orange;
                    case TOCEntryType.StaticSprite: return Color.OrangeRed;
                    case TOCEntryType.BossSprite: return Color.DarkOrange;
                    case TOCEntryType.Bitmap: return Color.RosyBrown;
                    case TOCEntryType.CollisionInfo: return Color.HotPink;
                    case TOCEntryType.EntitiesList: return Color.ForestGreen;
                    case TOCEntryType.Palette: return Color.Yellow;
                    case TOCEntryType.Music: return Color.CornflowerBlue;
                    case TOCEntryType.Sound: return Color.BlueViolet;
                    case TOCEntryType.Map: return Color.DeepSkyBlue;
                    case TOCEntryType.Tileset: return Color.SkyBlue;
                    case TOCEntryType.Text: return Color.Teal;
                    case TOCEntryType.Language: return Color.Thistle;
                    default: return Color.White;
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
