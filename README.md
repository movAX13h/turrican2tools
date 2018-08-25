# Turrican II Tools

Tools for the MS-DOS version of Turrican II, written in C# for .NET

File formats reverse-engineered 
by [movAX13h](https://github.com/movAX13h/) and [srtuss](https://github.com/srtuss), July 2018. 

## Features

 - load/unpack original game data
 - view/play/export files
 - [custom TFMX player by srtuss](https://github.com/movAX13h/turrican2tools/tree/master/TFXTool)
 - save sprites and tilesets as PNG
 - modify all files (hex editor)
 - repack game data

## EIFS - Executable Integrated File System

All data of the game is packed and appended to the main game executable in form of EIFS blocks. The last EIFS block is read first by the game and contains a TOC for all other files.

### Identification
6 bytes at end of file/section:
```
2 bytes ... length of section
4 bytes ... signature "EIFS"
```

### Unpacker

Turrican II stores its asset files as an obfuscated, or compressed and obfuscate stream of bytes.
The stream may be split into sequential compressed blocks that must be decompressed individually, each giving a portion of the unpacked file.
Every asset file uses a block-size of 1024 bytes (that means that every compressed block produces 1024 unpacked bytes), with exception of the TOC file, which is treated as a single block, and may exceed that limit.

Every compressed block starts with a word N, which states the length of the block, followed by N bytes of data.

The first byte of this data selects wether LZ compression is used.

If LZ is used, the stream is a series of instructions, with respective payload:

 - **move**: decipher a byte from the input stream, and write it to the output buffer
 - **repeat**: repeat a string that was decompressed before, by performing a string-copy within the output buffer
 - **fill**: decipher the given byte, and write it to the output buffer a given number of times

If LZ is not used, the data is an uncompressed block of ciphered bytes.

Turrican II uses an XOR cipher of 0x6B. For details see 
[AssetLoader.cs](https://github.com/movAX13h/turrican2tools/blob/master/T2Tools/Turrican/AssetLoader.cs)


### Table Of Contents (TOC)

Consists of a list of 24-byte entries. The list has a fixed maximum size, holding space for 300 entries (7200 bytes). Empty entries are filled with 00.
```
12 byte ... filename, maximum 12 characters, padded with whitespace characters (0x20)
 4 byte ... unpacked file length
 4 byte ... packed stream start position (counted from the beginning of the exe file)
 4 byte ... packed stream end position (counted from the beginning of the exe file)
```
For details see 
[AssetLoader.cs](https://github.com/movAX13h/turrican2tools/blob/master/T2Tools/Turrican/AssetLoader.cs)

## File Types
 
The game uses the following file types:

| Extension | Type(s) | Description | View/Play |
| --------- |:-------:|:----------- |:--:|
| .EXE | Executables | SETUP.EXE, INTRO.EXE, WORLD[1-5].EXE, ENDPART.EXE | |
| .BOB | Sprites | May contain multiple frames of varying size | x |
| .MC  | Sprites | Boss sprites | |
| .PCX | Sprite | UI, intro ground and T2 logo | x |
| .PIC | Tileset | Tilesets per world | x |
| .RAW | Bitmap | CRUSH1.RAW, CRUSH2.RAW | |
| .MAP | Map | Maps for each stage (tilebased grid) | x |
| .PAL | Palette | Colors for all parts of the game | x |
| .COL | Collisions | Tile based collision info per world | x |
| .EIB | Entities | Grid based list of entities for each cell of a stage (IDs are hardcoded in the game) | x |
| .TFX | Music | [TFMX](https://www.exotica.org.uk/wiki/TFMX) songs by Chris Hülsbeck | x |
| .SAM | Sound | raw 8-bit signed mono PCM data, containing every sample, per world | |
| .LNG | Language file | All strings in 4 languages | x |
| .TXT | Text | TEXT2.TXT contains the intro text | x |
| .FNT | Pixelfont | Font | |
| .FON | Font | Textmode font (used by the setup) |
| .DAT | unknown | same extension used for various file types | |
| .DIR | unknown | COMIC.DIR, 224 bytes |
| .P   | unknown | ? |
| .CHG | unknown | W3.CHG, 72 bytes
| .ANI | unknown | LINE.ANI, 1368 bytes
| .DLT | unknown | ANIM2.DLT, 262211 bytes

## Tools used

DOSBox Debugger, IDAPro, VisualStudio, HEX editors, custom binary visualizers