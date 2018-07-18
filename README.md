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

Consists of a list of 24 byte entries.
```
12 byte ... filename, space-padded right
 4 byte ... ? 
 4 byte ... ?
 4 byte ... ?
```


## File Types
 
The game uses the following file types:

| Name | Extension | Description |
| ---- |:---------:|:----------- |
| Static Sprite    | ?      | asfsdf sadffdfads ffsfdsfkjaöfj öljkaölj sdölkfj dsölkjsd öjödlj döljk ölkjsdö jödsl dkljösdlkj sdöjdsö ljdsölk asödlj sdölkj döslkjö jödlkj dklj ölkjö lkjöalkdj ödslkj ölkjasdf  fasd |
| Animated Sprite  | ?      |  |
| Palette          | ?      |  |
| Music            | ?      |  |
| Sound            | ?      |  |
| Text             | ?      |  |


## History

- 2018-07-18 unpacker simplified, assets loader by srtuss, repo created
- 2018-07-17 unpacker working thanks to srtuss, TOC readable
- 2018-07-16 translating EIFS unpack function to C#
- 2018-07-14 analysing disassembly of t2.exe
