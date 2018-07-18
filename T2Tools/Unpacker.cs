using System;

namespace T2Tools
{
    class Unpacker
    {
        public static int Unpack(out byte[] unpacked, byte[] packed, ushort offset)
        {
            unpacked = new byte[100000];
            for(int i = 0; i < unpacked.Length; ++i)
                unpacked[i] = 0xAB;


            int inputLength = packed[offset] + packed[offset + 1] * 256;

            int bitStorage = packed[offset + 3] * 256 + packed[offset + 4];
            offset += 2;


            if(packed[offset] == 128) // select decoder
            {
                // simple XOR-cipher decoder:
                for(int i = 1, w = 0, r = offset + 1; i < inputLength; ++w, ++r, ++i)
                {
                    unpacked[w + 2] = (byte)(packed[offset + 2 + r] ^ 0x6B);
                }
                return inputLength - 1;
            }
            else
            {
                // LZ77 variant with XOR-cipher:

                int readPos = 3;
                int writePos = 0;

                for(int bitCounter = 16; readPos < inputLength; bitStorage <<= 1, bitCounter--)
                {
                    if(bitCounter == 0)
                    {
                        bitStorage = packed[offset + readPos] * 256 + packed[offset + readPos + 1];
                        readPos += 2;
                        bitCounter = 16;
                    }

                    if((bitStorage & 0x8000) == 0)
                    {
                        // copy the next data-byte from input to output (and decipher):

                        unpacked[writePos + 2] = (byte)(packed[offset + readPos] ^ 0x6B); // "+2" is a guess
                        readPos++;
                        writePos++;
                    }
                    else
                    {
                        int cmd1 = packed[offset + readPos] << 4;
                        int cmd2 = packed[offset + readPos + 1];
                        cmd1 += cmd2 >> 4;

                        readPos++;

                        if(cmd1 == 0)
                        {
                            // i am uncertain about this part
                            // it appears to be a command for filling the same input byte into the output, a given number of times (and deciphering it)
                            // when decoding the TOC, this block gets called once, at the very end
                            // it fills in 1967 bytes of 0x22, making the whole decoded output exactly 7200 bytes long
                            // this might be on purpose, to initialize memory in the game

                            int cnt = cmd2 * 256 + (packed[offset + readPos + 1] + 16);
                            readPos += 2;

                            int A = offset + readPos;
                            for(int di = 0; di < cnt; ++di)
                            {
                                unpacked[writePos + di + 2] = (byte)(packed[offset + 2 + A] ^ 0x6B);
                            }

                            readPos++;
                            writePos += cnt;
                        }
                        else
                        {
                            // copy an already unpacked string, within the output buffer:
                            int numCopy = (packed[offset + readPos] & 0xF) + 3;
                            readPos++;
                            cmd1 = writePos - cmd1;
                            for(int j = 0, k = writePos; j < numCopy; ++cmd1, ++k, ++j)
                            {
                                unpacked[k + 2] = unpacked[cmd1 + 2];
                            }
                            writePos += numCopy;
                        }
                    }
                }

                return writePos; // return the number of bytes that were written to the output
            }
        }

        public static byte[] Unpack(byte[] packed, int offset, int length)
        {
            var k = new byte[length];
            Array.Copy(packed, offset, k, 0, length);
            Unpack(out byte[] unpacked, k, 0);
            return unpacked;
        }
    }
}
