namespace T2Tools.Turrican
{
    public class TOCEntry
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
