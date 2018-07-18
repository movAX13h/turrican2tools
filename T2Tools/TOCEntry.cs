using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2Tools
{
    class TOCEntry
    {
        public string Name;
        public int Size;
        public int PackedStart, PackedEnd;

        public byte[] Data;

        public override string ToString()
        {
            return Name;
        }

    }
}
