using System.Collections.Generic;
using System.IO;

namespace T2Tools.Turrican
{
    public class TOC
    {
        public Dictionary<string,TOCEntry> Entries;
        public bool Dirty
        {
            get
            {
                foreach (var entry in Entries.Values) if (entry.Dirty) return true;
                return false;
            }
        }

        public int ExportTo(string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            foreach(var entry in Entries.Values)
            {
                File.WriteAllBytes(Path.Combine(dir, entry.Name), entry.Data);
            }

            return Entries.Count;
        }
    }
}
