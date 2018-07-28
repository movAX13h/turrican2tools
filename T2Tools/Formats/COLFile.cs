namespace T2Tools.Formats
{
    public class COLFile
    {
        public struct COLFileEntry
        {
            public byte A, B, C, D;
        }

        public COLFileEntry[] Entries;

        public COLFile(byte[] data)
        {
            int num = data.Length / 4;
            Entries = new COLFileEntry[num];

            for (int i = 0; i < num; i++)
            {
                Entries[i] = new COLFileEntry() {
                    A = data[i * 4],
                    B = data[i * 4 + 1],
                    C = data[i * 4 + 2],
                    D = data[i * 4 + 3]
                };
            }
        }
    }



}
