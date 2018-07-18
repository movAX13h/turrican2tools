namespace T2Tools.Turrican
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
