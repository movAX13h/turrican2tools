# Turrican II Tools
Tools for the MS-DOS version of Turrican II.

## EIFS - Executable Integrated File System

All data of the game is packed and appended to the main game executable in form of EIFS blocks. The last EIFS block is ready first by the game and contains a TOC for all other files.

### Identification
6 bytes at end of file/section:
```
2 bytes ... length of section
4 bytes ... signature "EIFS"
```

### Unpacker

Turrican II stores it's asset-files as an obfuscated, or compressed and obfuscate stream of bytes.
The stream may be split into sequential compressed blocks, that must be decompressed individually, each giving a portion of the unpacked file.
Every asset-file uses a block-size of 1024 bytes (that means that every compressed block produces 1024 unpacked bytes), with exception of the TOC file, which is treated as a single block, and may exceed that limit.

Every compressed block starts with a word, which states the length of the block.

The next bytes selects wether LZ compression is used in addition to the XOR cipher.

In the case of files like audio samples for example, LZ will likely not produce a meaningful compression ratio, since the data is not necessarily predictable. The compressor will chose the mode that gives the smaller packed file.

The compression algorithm used is an LZ variant. The input stream is a series of instructions, with respective payload:
´´´
move   ... decipher a byte from the input stream, and write it to the output buffer
repeat ... repeat a string that was decompressed before, by performing a string-copy within the output buffer
fill   ... decipher the given byte, and write it the output buffer a given number of times
´´´

The XOR cipher in Turrican II is 0x6B.

Translated to C# from disassembly.

### Table Of Contents (TOC)

Consists of a list of 24-byte entries. The list has a fixed maximum size, holding space for 300 entries (7200 bytes). Empty entries are filled with 00.
```
12 byte ... filename, maximum 12 characters, padded with whitespace characters (0x20)
 4 byte ... unpacked file length
 4 byte ... packed stream start position (counted from the beginning of the exe file)
 4 byte ... packed stream end position (counted from the beginning of the exe file)
```


## File Types
 
The game uses the following file types:

| Name | Extension | Description |
| ---- |:---------:|:----------- |
| Static Sprite    | ?             | asfsdf sadffdfads ffsfdsfkjaöfj öljkaölj sdölkfj dsölkjsd öjödlj döljk ölkjsdö jödsl dkljösdlkj sdöjdsö ljdsölk asödlj sdölkj döslkjö jödlkj dklj ölkjö lkjöalkdj ödslkj ölkjasdf  fasd |
| Animated Sprite  | *.BOB         |  |
| Palette          | ?             |  |
| Music            | *.SAM, *.TFX  | [TFMX tracker module](https://www.exotica.org.uk/wiki/TFMX) |
| Sound            | *.SAM         | raw 8-bit signed mono PCM data, containing every sample, per game-level |
| Text             | ?             |  |

### .BOB Files

BOB files contain one frame for still image sprites, or multiple frames of animated sprites.

## History

- 2018-07-18 unpacker simplified, assets loader by srtuss, repo created
- 2018-07-17 unpacker working thanks to srtuss, TOC readable
- 2018-07-16 translating EIFS unpack function to C#
- 2018-07-14 analysing disassembly of t2.exe
