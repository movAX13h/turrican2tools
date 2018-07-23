using System.Collections.Generic;

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
    }
}
