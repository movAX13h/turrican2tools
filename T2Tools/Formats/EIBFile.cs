
using System;

namespace T2Tools.Formats
{
    class EIBFile
    {
        public struct EIBFileEntry
        {
            public int X;
        }

        public int A;
        public int B;
        public int C;

        public EIBFile(byte[] data)
        {
            A = BitConverter.ToInt16(data, 0);
            B = BitConverter.ToInt16(data, 2);
            C = BitConverter.ToInt16(data, 4);


            // taken from disassembly of WORLD1.EXE 
            // gives the number of bytes to load from the eib file (result is always: total length - 6)
            
            //mov ax, eib_second_2
            //inc ax
            int len = (B + 1);
            //mov dx, ax
            //mov ax, eib_first_2
            //inc ax
            //mul dx
            //mov dx, ax
            len *= (A + 1);
            //mov ax, eib_second_2
            //inc ax
            //add ax, dx
            //shl ax, 1
            len = 2 * (len + (B + 1));
            //add ax, [bp + eib_third]
            len += C;

            // len = 2 * ((A + 1) * (B + 1) + (B + 1)) + C
            // len = 2 * (a * b + b) + c
            // len = 2 * b * (a + 1) + c

            // the first loaded byte after the first 6 is always 0 in turrican 2
            // it is added to B and used as mem length for B: lenB = 2 * (B + 1) + D

        }
    }
}
