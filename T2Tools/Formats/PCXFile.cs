/////////////////////////////////////////////////////////////////////////////////
// based on PCX Plugin for Paint.NET
// Copyright (C) 2006 Joshua Bell (inexorabletash@gmail.com)
// Portions Copyright (C) 2006 Rick Brewster, et. al.
// See License.txt for complete licensing and attribution information.
/////////////////////////////////////////////////////////////////////////////////

// References: 
//  * PCX File Format - http://courses.ece.uiuc.edu/ece390/books/labmanual/graphics-pcx.html
//  * PCX File Format - http://www.fileformat.info/format/pcx/
//  * EGA Color Palette - http://wasteland.wikispaces.com/EGA+Colour+Palette

// Test images:
//  * 1, 2, 8, 24: http://www.efg2.com/Lab/Library/Delphi/Graphics/FileFormatsAndConversion.htm (PCX.ZIP)
//  * 8, 24: http://www.fileformat.info/format/pcx/sample/index.htm
//  * 1: http://memory.loc.gov/ammem/help/view.html

// NOTE: Only supports saving 256-color RLE encoded images

////////////////////////////////////////////////////////////////////////////////////////////////////
//Modified by EzArIk(Thomas C. Maylam [on (GB)11/20/2016]) to support the use of preset Pallettes.//
////////////////////////////////////////////////////////////////////////////////////////////////////
// modified by Philip Wagner (filip.sound@gmail.com) for standalone usage 
// (no paint.net) and reading only
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.IO;

namespace T2Tools.Formats
{
    public class PCXFile
    {
        public Color[] Palette { get { return palette.Colors; } }
        public Bitmap Bitmap { get { return bitmap; } }

        private PcxPalette palette;
        private Bitmap bitmap;

        #region File Structure, Header and Constants

        ////////////////////////////////////////////////////////////
        // PCX File Structure
        //
        //    Header        128 bytes
        //
        //    Pixel Data    scan0 plane0
        //                  scan0 plane1
        //                  ..
        //                  scan0 planeN
        //                  scan1 plane0
        //                  scan1 plane1
        //                  ..
        //                  scan1 planeN
        //                  ...
        //                  scanM planeN
        //
        //    Palette       0x0C         
        //    (8-bit only)  r0,g0,b0
        //                  r1,g1,b1
        //                  ...
        //                  r256,g256,b256
        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        // struct PCXHeader 
        // {
        //     BYTE Manufacturer;  // Constant Flag   10 = ZSoft .PCX
        //     BYTE Version;       // Version Information
        //                         // 0 = Version 2.5
        //                         // 2 = Version 2.8 w/palette information
        //                         // 3 = Version 2.8 w/o palette information
        //                         // 4 = (PC Paintbrush for Windows)
        //                         // 5 = Version 3.0
        //     BYTE Encoding;      // 1 = .PCX run length encoding
        //     BYTE BitsPerPixel;  // Number of bits/pixel per plane (1, 2, 4 or 8)
        //     WORD XMin;          // Picture Dimensions 
        //     WORD YMin;          // (Xmin, Ymin) - (Xmax - Ymax) inclusive
        //     WORD XMax;
        //     WORD YMax;
        //     WORD HDpi;          // Horizontal Resolution of creating device
        //     WORD VDpi;          // Vertical Resolution of creating device
        //     BYTE ColorMap[48];  // Color palette for 16-color palette
        //     BYTE Reserved;
        //     BYTE NPlanes;       // Number of color planes
        //     WORD BytesPerLine;  // Number of bytes per scan line per color plane (always even for .PCX files)
        //     WORD PaletteInfo;   // How to interpret palette - 1 = color/BW, 2 = grayscale
        //     BYTE Filler[58];
        // };
        ////////////////////////////////////////////////////////////

        private enum PcxId : byte
        {
            ZSoftPCX = 10
        };

        private enum PcxVersion : byte
        {
            Version2_5 = 0,
            Version2_8_Palette = 2,
            Version2_8_DefaultPalette = 3,
            Version3_0 = 5
        };

        private enum PcxEncoding : byte
        {
            None = 0,
            RunLengthEncoded = 1
        };

        private enum PcxPaletteType : byte
        {
            Indexed = 1,
            Grayscale = 2
        };

        private const int PcxRleMask = 0xC0;
        private const int PcxPaletteMarker = 0x0C;

        private class PcxHeader
        {
            public PcxId id = PcxId.ZSoftPCX;
            public PcxVersion version = PcxVersion.Version3_0;
            public PcxEncoding encoding = PcxEncoding.RunLengthEncoded;
            public byte bitsPerPixel;
            public ushort xMin;
            public ushort yMin;
            public ushort xMax;
            public ushort yMax;
            public ushort hDpi;
            public ushort vDpi;
            public byte[] colorMap = new byte[48];
            public byte reserved = 0;
            public byte nPlanes;
            public ushort bytesPerLine;
            public PcxPaletteType paletteInfo;
            public byte[] filler = new byte[58];

            public PcxHeader(Stream input)
            {
                id = (PcxId)ReadByte(input);
                version = (PcxVersion)ReadByte(input);
                encoding = (PcxEncoding)ReadByte(input);
                bitsPerPixel = ReadByte(input);
                xMin = ReadUInt16(input);
                yMin = ReadUInt16(input);
                xMax = ReadUInt16(input);
                yMax = ReadUInt16(input);
                hDpi = ReadUInt16(input);
                vDpi = ReadUInt16(input);
                for (int i = 0; i < colorMap.Length; i++)
                    colorMap[i] = ReadByte(input);
                reserved = ReadByte(input);
                nPlanes = ReadByte(input);
                bytesPerLine = ReadUInt16(input);
                paletteInfo = (PcxPaletteType)ReadUInt16(input);
                for (int i = 0; i < filler.Length; i++)
                    filler[i] = ReadByte(input);
            }

            private byte ReadByte(Stream input)
            {
                int byteRead = input.ReadByte();
                if (byteRead == -1)
                    throw new EndOfStreamException();

                return (byte)byteRead;
            }

            private ushort ReadUInt16(Stream input)
            {
                int b1 = input.ReadByte();
                if (b1 == -1)
                    throw new EndOfStreamException();

                int b2 = input.ReadByte();
                if (b2 == -1)
                    throw new EndOfStreamException();

                return (ushort)(b2 * 256 + b1);
            }
        }

        #endregion

        #region Palette

        /**
		 * PcxPalette 
		 * 
		 * Always either 16 or 256 entries
		 */
        private class PcxPalette
        {
            public static readonly uint[] MONO_PALETTE = new uint[] { 0x000000, 0xFFFFFF, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000 };
            public static readonly uint[] CGA_PALETTE = new uint[] { 0x000000, 0x00AAAA, 0xAA00AA, 0xAAAAAA, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000 };
            public static readonly uint[] EGA_PALETTE = new uint[] { 0x000000, 0x0000A8, 0x00A800, 0x00A8A8, 0xA80000, 0xA800A8, 0xA85400, 0xA8A8A8, 0x545454, 0x5454FE, 0x54FE54, 0x54FEFE, 0xFE5454, 0xFE54FE, 0xFEFE54, 0xFEFEFE };

            private Color[] m_palette;

            public Color[] Colors { get { return m_palette; } }

            public uint Size
            {
                get { return (uint)m_palette.Length; }
            }

            public Color this[uint index]
            {
                get { return m_palette[index]; }
                set { m_palette[index] = value; }
            }

            public PcxPalette(uint size)
            {
                if (size != 2 && size != 16 && size != 256)
                    throw new FormatException("Unsupported palette size");

                m_palette = new Color[size];
            }
            /*
            public PcxPalette(ColorPalette palette)
            {
                Color[] entries = palette.Entries;
                int length = entries.Length;

                // Ignore transparent padding entries from OctreeQuantizer
                int padding = Array.FindIndex(entries, c => c.ToArgb() == Color.Transparent.ToArgb());
                if (padding != -1)
                    length = padding;

                uint size;
                if (length <= 2)
                    size = 2;
                else if (length <= 16)
                    size = 16;
                else if (length <= 256)
                    size = 256;
                else
                    throw new FormatException("Unsupported palette size");

                m_palette = new Color[size];

                for (uint i = 0; i < length; i++)
                    m_palette[i] = entries[i];

                // Fill rest of the palette with black
                for (uint i = size - 1; i >= length; i--)
                    m_palette[i] = Color.Black;
            }
            */

            public PcxPalette(Stream input, int size)
            {
                if (size != 16 && size != 256)
                    throw new FormatException("Unsupported palette size");

                m_palette = new Color[size];

                for (int i = 0; i < m_palette.Length; ++i)
                {
                    int red = input.ReadByte();
                    if (red == -1)
                        throw new EndOfStreamException();

                    int green = input.ReadByte();
                    if (green == -1)
                        throw new EndOfStreamException();

                    int blue = input.ReadByte();
                    if (blue == -1)
                        throw new EndOfStreamException();

                    m_palette[i] = Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
                }
            }

            public static PcxPalette FromColorMap(byte[] colorMap)
            {
                if (colorMap == null)
                    throw new ArgumentNullException("colorMap");
                if (colorMap.Length != 48)
                    throw new FormatException("Trying to read an unsupported palette size from a header ColorMap");

                PcxPalette palette = new PcxPalette(16);

                uint index = 0;
                for (uint i = 0; i < 16; i++)
                {
                    byte r = colorMap[index++];
                    byte g = colorMap[index++];
                    byte b = colorMap[index++];

                    palette[i] = Color.FromArgb(255, r, g, b);
                }

                return palette;
            }

            public static PcxPalette FromEgaPalette(uint[] egaPalette)
            {
                if (egaPalette == null)
                    throw new ArgumentNullException("egaPalette");
                if (egaPalette.Length != 16)
                    throw new FormatException("Trying to read an unsupported palette size from a header ColorMap");

                PcxPalette palette = new PcxPalette(16);

                for (uint i = 0; i < 16; i++)
                {
                    byte r = (byte)((egaPalette[i] >> 16) & 0xff);
                    byte g = (byte)((egaPalette[i] >> 8) & 0xff);
                    byte b = (byte)((egaPalette[i]) & 0xff);

                    palette[i] = Color.FromArgb(255, r, g, b);
                }

                return palette;
            }

        }

        #endregion

        public bool Load(byte[] data)
        {
            Stream input = new MemoryStream(data);

            //
            // Load and validate header
            //
            PcxHeader header = new PcxHeader(input);

            if (header.id != PcxId.ZSoftPCX)
                throw new FormatException("Not a PCX file.");

            if (header.version != PcxVersion.Version3_0 &&
                header.version != PcxVersion.Version2_8_Palette &&
                header.version != PcxVersion.Version2_8_DefaultPalette &&
                header.version != PcxVersion.Version2_5)
                throw new FormatException(String.Format("Unsupported PCX version: {0}", header.version));

            if (header.bitsPerPixel != 1 &&
                header.bitsPerPixel != 2 &&
                header.bitsPerPixel != 4 &&
                header.bitsPerPixel != 8)
                throw new FormatException(String.Format("Unsupported PCX bits per pixel: {0} bits per pixel", header.bitsPerPixel));

            int width = header.xMax - header.xMin + 1;
            int height = header.yMax - header.yMin + 1;
            if (width < 0 || height < 0 || width > 0xffff || height > 0xffff)
                throw new FormatException(String.Format("Invalid image dimensions: ({0},{1})-({2},{3})", header.xMin, header.yMin, header.xMax, header.yMax));

            // Pixels per line, including PCX's even-number-of-pixels buffer
            int pixelsPerLine = header.bytesPerLine * 8 /*bitsPerByte*/ / header.bitsPerPixel;

            // Bits per pixel, including all bit planes
            int bitsPerPixel = header.bitsPerPixel * header.nPlanes;

            if (bitsPerPixel != 1 &&
                bitsPerPixel != 2 &&
                bitsPerPixel != 4 &&
                bitsPerPixel != 8 &&
                bitsPerPixel != 24)
                throw new FormatException(String.Format("Unsupported PCX bit depth: {0}", bitsPerPixel));

            //
            // Load the palette
            //
            if (bitsPerPixel == 1)
            {
                // HACK: Monochrome images don't always include a resonable palette in v3.0.
                // Default them to black and white in all cases

                palette = PcxPalette.FromEgaPalette(PcxPalette.MONO_PALETTE);
            }
            else if (bitsPerPixel < 8)
            {
                // 16-color palette in the ColorMap portion of the header

                switch (header.version)
                {
                    case PcxVersion.Version2_5:
                    case PcxVersion.Version2_8_DefaultPalette:
                        {
                            uint[] paletteEga;

                            switch (bitsPerPixel)
                            {
                                // 4-color CGA palette 
                                case 2:
                                    paletteEga = PcxPalette.CGA_PALETTE;
                                    break;

                                // 16-color EGA palette
                                default:
                                case 4:
                                    paletteEga = PcxPalette.EGA_PALETTE;
                                    break;
                            }

                            palette = PcxPalette.FromEgaPalette(paletteEga);
                            break;
                        }

                    default:
                    case PcxVersion.Version2_8_Palette:
                    case PcxVersion.Version3_0:
                        {
                            palette = PcxPalette.FromColorMap(header.colorMap);
                            break;
                        }

                }
            }
            else if (bitsPerPixel == 8)
            {
                // 256-color palette is saved at the end of the file, with one byte marker

                long dataPosition = input.Position;
                input.Seek(-(1 + (256 * 3)), SeekOrigin.End);

                if (input.ReadByte() != PcxPaletteMarker)
                    throw new FormatException("PCX palette marker not present in file");

                palette = new PcxPalette(input, 256);

                input.Seek(dataPosition, SeekOrigin.Begin);
            }
            else
            {
                // Dummy palette for 24-bit images
                palette = new PcxPalette(256);
            }

            //
            // Load the pixel data
            //
            bitmap = new Bitmap(width, height);

            // Accumulate indices across bit planes
            uint[] indexBuffer = new uint[width];

            for (int y = 0; y < height; y++)
            {
                Array.Clear(indexBuffer, 0, width);

                // Decode the RLE byte stream
                PcxByteReader byteReader = (header.encoding == PcxEncoding.RunLengthEncoded)
                    ? new PcxRleByteReader(input)
                    : (PcxByteReader)new PcxRawByteReader(input);

                // Read indices of a given length out of the byte stream
                PcxIndexReader indexReader = new PcxIndexReader(byteReader, header.bitsPerPixel);

                // Planes are stored consecutively for each scan line
                for (int plane = 0; plane < header.nPlanes; plane++)
                {
                    for (int x = 0; x < pixelsPerLine; x++)
                    {
                        uint index = indexReader.ReadIndex();

                        // Account for padding bytes
                        if (x < width)
                            indexBuffer[x] = indexBuffer[x] | (index << (plane * header.bitsPerPixel));
                    }
                }

                for (int x = 0; x < width; x++)
                {
                    uint index = indexBuffer[x];
                    Color color;

                    if (bitsPerPixel == 24)
                    {
                        byte r = (byte)((index) & 0xff);
                        byte g = (byte)((index >> 8) & 0xff);
                        byte b = (byte)((index >> 16) & 0xff);

                        color = Color.FromArgb(255, r, g, b);
                    }
                    else
                    {
                        color = palette[index];
                    }

                    bitmap.SetPixel(x, y, color);
                }
            }

            return true;
        }


        #region Index Reader
        /**
		 * Classes to handle reading indices of various bit depths to/from encoded streams.
		 * 
		 */

        private class PcxIndexReader
        {
            private PcxByteReader m_reader;
            private uint m_bitsPerPixel;
            private uint m_bitMask;

            private uint m_bitsRemaining = 0;
            private uint m_byteRead;

            public PcxIndexReader(PcxByteReader reader, uint bitsPerPixel)
            {
                if (!(bitsPerPixel == 1 || bitsPerPixel == 2 || bitsPerPixel == 4 || bitsPerPixel == 8))
                    throw new ArgumentException("bitsPerPixel must be 1, 2, 4 or 8", "bitsPerPixel");

                m_reader = reader;
                m_bitsPerPixel = bitsPerPixel;
                m_bitMask = (uint)((1 << (int)m_bitsPerPixel) - 1);
            }

            public uint ReadIndex()
            {
                // NOTE: This does not work for non-power-of-two bits per pixel (e.g. 6)
                // since it does not concatenate shift adjacent bytes together

                if (m_bitsRemaining == 0)
                {
                    m_byteRead = m_reader.ReadByte();
                    m_bitsRemaining = 8;
                }

                // NOTE: Reads from the most significant bits
                uint index = (m_byteRead >> (int)(8 - m_bitsPerPixel)) & m_bitMask;
                m_byteRead = m_byteRead << (int)m_bitsPerPixel;
                m_bitsRemaining -= m_bitsPerPixel;

                return index;
            }
        }
        #endregion

        #region RLE Decoding

        /**
		 * Classes to handle RLE encoding/decoding to/from streams.
		 * 
		 */
        private abstract class PcxByteReader
        {
            public abstract byte ReadByte();
        }

        private class PcxRawByteReader : PcxByteReader
        {
            private Stream m_stream;
            public PcxRawByteReader(Stream stream)
            {
                m_stream = stream;
            }
            public override byte ReadByte()
            {
                return (byte)m_stream.ReadByte();
            }
        }

        private class PcxRleByteReader : PcxByteReader
        {
            private Stream m_stream;
            private uint m_count = 0;
            private byte m_rleValue;

            public PcxRleByteReader(Stream input)
            {
                m_stream = input;
            }

            public override byte ReadByte()
            {
                if (m_count > 0)
                {
                    m_count--;
                    return m_rleValue;
                }

                byte code = (byte)m_stream.ReadByte();

                if ((code & PcxRleMask) == PcxRleMask)
                {
                    m_count = (uint)(code & (PcxRleMask ^ 0xff));
                    m_rleValue = (byte)m_stream.ReadByte();

                    m_count--;
                    return m_rleValue;
                }

                return code;
            }
        }

        #endregion
    }
}
