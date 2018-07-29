
using System;

namespace T2Tools.Formats
{
    class EIBFile
    {
        int a;

        public EIBFile(byte[] data)
        {
            a = BitConverter.ToInt16(data, 0);
        }
    }
}
