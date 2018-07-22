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
                foreach (var item in Entries) if (item.Value.Dirty) return true;
                return false;
            }
        }
    }
}
