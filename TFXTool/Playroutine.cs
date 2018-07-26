using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T2Tools.Formats;

namespace TFXTool
{
    class Playroutine
    {
        public PaulaChip Paula;
        TFXFile tfx;
        public byte[] sampledata;
        Track[] tracks;
        int trackstepPosition;
        bool advanceTrackstep = false;
        public int tickCounter = 0;
        public int Frequency = 11;

        Channel[] channels;

        public int TrackstepPosition => trackstepPosition;
        public event EventHandler TrackstepPositionChanged;
        public event EventHandler TempoChanged;
        public event EventHandler<MacroStartEventArgs> MacroStart;

        public void SetSong(int i)
        {
            trackstepPosition = tfx.SongStartPositions[i];
            advanceTrackstep = true;
        }

        void Log(string text)
        {
            Console.WriteLine(text);
        }

        public void VBI()
        {
            again:
            if(advanceTrackstep)
            {
                advanceTrackstep = false;

                while(tfx.Tracksteps[trackstepPosition][0] == 0xEFFE)
                {
                    switch(tfx.Tracksteps[trackstepPosition][1])
                    {
                        case 0: // stop the player
                            break;
                        case 1: // Play a section starting at position and ending here times
                                // times.  If times is 0000 then section will repeat forever.
                                // line+4=position, line+6=times
                            break;
                        case 2: // set the tempo
                            break;
                        case 3: // start a master volume slide (?)
                            if(tfx.Tracksteps[trackstepPosition][3] == 0xEE)
                            {
                                Frequency = 14;
                                TempoChanged?.Invoke(this, EventArgs.Empty);
                            }
                            else if(tfx.Tracksteps[trackstepPosition][3] == 0x10)
                            {
                                Frequency = 30;
                                TempoChanged?.Invoke(this, EventArgs.Empty);
                            }
                            break;
                        case 4:
                            break;
                    }

                    ++trackstepPosition;
                }

                for(int i = 0; i < 8; ++i)
                {
                    var track = tracks[i];
                    var d = tfx.Tracksteps[trackstepPosition][i];
                    int stepPattern = d >> 8;
                    int stepTranspose = (sbyte)(d & 0xFF);

                    // init pattern
                    track.PatternPC = 0;
                    track.NumWait = 0;
                    track.NumPatternLoop = 0;

                    // TD: handle 0x80 and 0xFE correctly!
                    if(stepPattern >= 0x80)
                    {
                        if(stepPattern == 0x80)
                        {
                            //track.PatternPC = 0;
                        }
                        else
                        {
                            track.PatternNo = -1;
                        }
                    }
                    else
                    {
                        Log("starting pattern " + track.PatternNo + " in tick " + tickCounter);

                        track.PatternNo = stepPattern;
                        track.Transpose = stepTranspose;
                    }
                }

                TrackstepPositionChanged?.Invoke(this, EventArgs.Empty);

                ++trackstepPosition;
            }

            for(int i = 0; i < 8; ++i)
            {
                var track = tracks[i];
                if(track.PatternNo != -1) // track is not stopped
                {
                    if(track.NumWait == 0) // pattern is not waiting
                    {
                        while(track.NumWait == 0)
                        {
                            var pattern = tfx.Patterns[track.PatternNo];
                            var step = pattern.Steps[track.PatternPC];

                            ++track.PatternPC;

                            if(step.Note >= 0xF0)
                            {
                                if(step.Note == 0xF0) // F0 xx xx xx	<End> end pattern
                                {
                                    advanceTrackstep = true;
                                    break;
                                }
                                else if(step.Note == 0xF1) // F1 aa bb bb	<Loop> repeat block
                                {
                                    int loopcount = step.Macro;
                                    if(loopcount == 0 || track.NumPatternLoop < loopcount)
                                    {
                                        track.PatternPC = step.Data & 0xFFFF;
                                        ++track.NumPatternLoop;
                                    }
                                    else
                                        track.NumPatternLoop = 0;
                                }
                                else if(step.Note == 0xF3) // F3 aa xx xx	<Wait> rest
                                {
                                    track.NumWait = step.Macro + 1;
                                }
                                else if(step.Note == 0xF4) // F4 xx xx xx	<STOP> disable track
                                {
                                    track.PatternNo = -1;
                                    break;
                                }
                            }
                            else
                            {
                                // TD: volume, detune
                                int note = -1;
                                if(step.Note < 0x80) // note without wait
                                {
                                    note = step.Note;
                                    // TD: step.Detune is actual pitch detune
                                }
                                else if(step.Note < 0xC0) // note with wait
                                {
                                    note = step.Note - 0x80;
                                    track.NumWait = 1 + step.Detune;
                                }
                                else if(step.Note < 0xF0) // note with portamento
                                {
                                    note = step.Note - 0xC0;
                                    // TD: portamento
                                }

                                // bind the note to the channel:
                                var c = channels[step.Channel];
                                c.Note = note + track.Transpose;
                                c.Volume = step.Volume;
                                c.MacroNo = step.Macro;
                                c.MacroPC = 0;
                                Log("starting macro " + c.MacroNo + " in tick " + tickCounter + " channel " + step.Channel);

                                MacroStart?.Invoke(this, new MacroStartEventArgs { Channel = step.Channel, MacroNo = step.Macro });
                            }
                        }
                    }
                    if(track.NumWait > 0)
                        --track.NumWait;
                }

            }

            for(int i = 0; i < 8; ++i)
            {
                var channel = channels[i];
                if(channel.MacroNo != -1)
                {
                    var macro = tfx.Macros[channel.MacroNo];
                    var paulaRegs = Paula.Channels[i];

                    bool done = false;
                    while(!done)
                    {
                        var step = macro.Steps[channel.MacroPC++];

                        switch(step.Type)
                        {
                            case 0x0: // <DMAoff+Reset> stop efx and dma *
                                paulaRegs.Reset();
                                paulaRegs.Volume = 63;
                                break;
                            case 0x2: // 02 aa aa aa	<SetBegin> set beginning of sample
                                paulaRegs.StartByte = step.Parameter;
                                paulaRegs.Data = sampledata;
                                break;
                            case 0x3: // 03 xx aa aa	<SetLen> set length
                                paulaRegs.LengthWords = step.Parameter & 0xFFFF;
                                break;
                            case 0x7: // 07 xx xx xx	<STOP> end macro *
                                channel.MacroNo = -1; // stop macro execution
                                done = true;
                                break;
                            case 0x8: // 08 aa bb bb	<AddNote> set freq by this note *
                                double fsmp = 520 * 4 * Math.Pow(2, (channel.Note + (sbyte)step.B) / 12d);
                                paulaRegs.Period = (int)Math.Round(7159091 / fsmp / 2);
                                Log("channel " + i + " set period in tick " + tickCounter);
                                break;
                            case 0x9: // 09 aa bb bb	<SetNote> set freq direct *
                                fsmp = 520 * 4 * Math.Pow(2, step.B / 12d);
                                paulaRegs.Period = (int)Math.Round(7159091 / fsmp / 2);
                                break;
                            case 0x17: // 17 xx aa aa	<Set period> Absolute period *
                                paulaRegs.Period = step.Parameter & 0xFFFF;
                                break;
                            case 0x18: // <Sampleloop> set sample loop
                                paulaRegs.StartLoopByte = step.Parameter;
                                break;
                            case 0x19: // 19 xx xx xx	<Set one shot sample>
                                paulaRegs.OneShot = true;
                                break;

                        }
                    }
                }
            }

            if(advanceTrackstep)
                goto again;

            ++tickCounter;
        }

        public Playroutine(TFXFile tfx)
        {
            this.tfx = tfx;
            tracks = new Track[8];
            for(int i = 0; i < tracks.Length; ++i)
                tracks[i] = new Track();

            channels = new Channel[8];
            for(int i = 0; i < channels.Length; ++i)
                channels[i] = new Channel();

            SetSong(0);
        }
    }

    class MacroStartEventArgs : EventArgs
    {
        public int Channel;
        public int MacroNo;
    }

    /// <summary>
    /// track state
    /// </summary>
    class Track
    {
        internal int PatternNo = -1;
        internal int Transpose;

        internal int NumWait;
        internal int PatternPC;
        internal int NumPatternLoop;
    }

    /// <summary>
    /// channel state
    /// </summary>
    class Channel
    {
        internal int MacroNo = -1;
        internal int MacroPC;

        internal int Note;
        internal int Volume;
        
        internal string NoteName => TFXTool.Note.Format(Note);
    }
}
