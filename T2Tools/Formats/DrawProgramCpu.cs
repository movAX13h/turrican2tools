using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2Tools.Formats
{
    class DrawProgramCpu
    {
        public event EventHandler<CpuWriteEvent> Write;
        public event EventHandler EgaSequencerReset;

        public byte[] Text;
        public byte[] Data;
        int pc = 0; // (EIP)

        int ds = 0;
        public int DS => ds;

        int ax = 0; // word Accumulator
        int bx = 0; // word Base
        int cx = 0; // word Counter
        int dx = 0; // word Data
        int di = 0; // word DestinationPtr
        int si = 0; // word SourcePtr
        int ah
        {
            get => ax >> 8;
            set => ax = (ax & 0xFF) + (value & 0xFF) * 256;
        }
        int cl
        {
            get => cx & 0xFF;
            set => cx = (cx & 0xFF00) + (value & 0xFF);
        }
        int ch
        {
            get => cx >> 8;
            set => cx = (cx & 0xFF) + (value & 0xFF) * 256;
        }

        public int DI
        {
            set => di = value;
            get => di;
        }


        void ExecuteInstruction()
        {
            int at = pc;
            int r, imm;
            int opcode = Text[pc++];
            switch(opcode)
            {
                case 0x1E: // push ds
                    // ...
                    break;
                case 0x1F: // pop ds
                    // ...
                    break;
                case 0x32: // xor reg, reg
                    r = Text[pc++];
                    switch(r)
                    {
                        case 0xED: // xor ch, ch
                            ch = 0;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case 0x43: // inc bx
                    ++bx;
                    break;
                case 0x47: // inc di
                    ++di;
                    break;
                case 0x73: // jnb rel8
                    r = (sbyte)Text[pc++];
                    if(false) // < condition
                    {
                        pc += r;
                    }
                    break;
                case 0x81: // { arith imm16 }
                    r = Text[pc++];
                    imm = Text[pc] + Text[pc + 1] * 256;
                    pc += 2;
                    switch(r)
                    {
                        case 0xC7: // add, di, imm16
                            di += imm;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case 0x83: // { arith imm8 }
                    r = Text[pc++];
                    imm = Text[pc++];
                    switch(r)
                    {
                        case 0xC7: // add, di, imm8
                            di += imm;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case 0x8E: // mov reg, reg
                    r = Text[pc++];
                    switch(r)
                    {
                        case 0xD8: // mov ds, ax
                            ds = ax;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case 0x8B: // mov reg, reg
                    r = Text[pc++];
                    switch(r)
                    {
                        case 0xC1: // mov ax, cx
                            ax = cx;
                            break;
                        case 0xDF: // mov bx, di
                            bx = di;
                            break;
                        case 0xFB: // mov di, bx
                            di = bx;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case 0xA4: // movsb
                    // Move byte at address DS:(E)SI to address ES:(E)DI.
                    //Dest[di++] = Data[si++];
                    Write.Invoke(this, new CpuWriteEvent { Address = di, Value = si < Data.Length ? Data[si] : 40 });
                    ++di; ++si;
                    break;
                case 0xA5: // movew
                    // Move word at address DS:(E)SI to address ES:(E)DI.
                    //Dest[di++] = Data[si++];
                    //Dest[di++] = Data[si++];
                    Write.Invoke(this, new CpuWriteEvent { Address = di, Value = si < Data.Length ? Data[si] : 40 });
                    ++di; ++si;
                    Write.Invoke(this, new CpuWriteEvent { Address = di, Value = si < Data.Length ? Data[si] : 40 });
                    ++di; ++si;
                    break;
                case 0xB1: // mov cl, imm8
                    imm = Text[pc++];
                    cl = imm;
                    break;
                case 0xB8: // mov ax, imm16
                    imm = Text[pc] + Text[pc + 1] * 256;
                    pc += 2;
                    ax = imm;
                    break;
                case 0xBA: // mov dx, imm16
                    imm = Text[pc] + Text[pc + 1] * 256;
                    pc += 2;
                    dx = imm;
                    break;
                case 0xBE: // mov si, imm16
                    imm = Text[pc] + Text[pc + 1] * 256;
                    si = imm;
                    pc += 2;
                    break;
                case 0xCB: // retf
                    // Far return to calling procedure.
                    pc = -1;
                    break;
                case 0xD0:
                    r = Text[pc++];
                    switch(r)
                    {
                        case 0xC4: // rol ah, 1
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case 0xEF: // out dx, ax
                    switch(dx)
                    {
                        case 0x3C4: // EGA: sequencer address reg
                            if(EgaSequencerReset != null)
                                EgaSequencerReset.Invoke(this, EventArgs.Empty);
                            break;
                    }
                    break;
                case 0xF3: // rep
                    switch(Text[pc])
                    {
                        case 0xA4: // rep movsb
                        case 0xA5: // rep movsw
                            ++pc;
                            while(cx != 0)
                            {
                                --pc;
                                ExecuteInstruction();
                                --cx;
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                default:
                    throw new Exception("unknown instruction \"" + opcode.ToString("X") + "\" at $" + at.ToString("X"));
            }
        }

        public void Call(int addr)
        {
            pc = addr;
            while(pc != -1)
            {
                ExecuteInstruction();
            }
            int end = 0;
        }
    }

    class CpuWriteEvent
    {
        public int Address;
        public int Value;
    }
}
