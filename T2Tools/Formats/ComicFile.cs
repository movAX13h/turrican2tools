using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace T2Tools.Formats
{
    public class ComicFile
    {
        private const int PaletteSize = 256 * 3;
        private const int DirectoryEntrySize = 14;
        private const int RunMarker = 0xC0;

        public static Bitmap[] Decode(byte[] data, byte[] directory)
        {
            if (data == null || data.Length <= PaletteSize)
                throw new InvalidDataException("Comic data is missing its palette or image data.");
            if (directory == null || directory.Length == 0 || directory.Length % DirectoryEntrySize != 0)
                throw new InvalidDataException("Comic directory has an invalid size.");

            Color[] palette = new Color[256];
            for (int i = 0; i < palette.Length; i++)
            {
                palette[i] = Color.FromArgb(255,
                    VGABitmapConverter.Convert6BitTo8Bit(data[i * 3]),
                    VGABitmapConverter.Convert6BitTo8Bit(data[i * 3 + 1]),
                    VGABitmapConverter.Convert6BitTo8Bit(data[i * 3 + 2]));
            }

            List<byte> pixels = new List<byte>();
            int offset = PaletteSize;
            while (offset < data.Length)
            {
                byte value = data[offset++];
                if (value >= RunMarker)
                {
                    int count = value & 0x3F;
                    if (count == 0 || offset >= data.Length)
                        throw new InvalidDataException("Comic data contains an invalid run.");

                    value = data[offset++];
                    for (int i = 0; i < count; i++) pixels.Add(value);
                }
                else
                {
                    pixels.Add(value);
                }
            }

            int imageCount = directory.Length / DirectoryEntrySize;
            int requiredPixels = 0;
            for (int imageIndex = 0; imageIndex < imageCount; imageIndex++)
            {
                int entryOffset = imageIndex * DirectoryEntrySize;
                int width = BitConverter.ToUInt16(directory, entryOffset + 8);
                int height = BitConverter.ToUInt16(directory, entryOffset + 10);
                if (width == 0 || height == 0)
                    throw new InvalidDataException("Comic directory contains an image with invalid dimensions.");

                requiredPixels += width * height;
            }

            if (pixels.Count != requiredPixels)
                throw new InvalidDataException("Comic image data does not match the dimensions in its directory.");

            Bitmap[] images = new Bitmap[imageCount];
            int pixelOffset = 0;
            for (int imageIndex = 0; imageIndex < imageCount; imageIndex++)
            {
                int entryOffset = imageIndex * DirectoryEntrySize;
                int width = BitConverter.ToUInt16(directory, entryOffset + 8);
                int height = BitConverter.ToUInt16(directory, entryOffset + 10);
                Bitmap image = new Bitmap(width, height);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        image.SetPixel(x, y, palette[pixels[pixelOffset++]]);
                    }
                }
                images[imageIndex] = image;
            }

            return images;
        }
    }
}