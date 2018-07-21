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

Translated to C# from disassembly.
?

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
| Animated Sprite  | ?             |  |
| Palette          | ?             |  |
| Music            | *.SAM, *.TFX  | [TFMX tracker module](https://www.exotica.org.uk/wiki/TFMX) |
| Sound            | *.SAM         | raw 8-bit signed mono PCM data, containing every sample, per game-level |
| Text             | ?             |  |


## History

- 2018-07-18 unpacker simplified, assets loader by srtuss, repo created
- 2018-07-17 unpacker working thanks to srtuss, TOC readable
- 2018-07-16 translating EIFS unpack function to C#
- 2018-07-14 analysing disassembly of t2.exe
