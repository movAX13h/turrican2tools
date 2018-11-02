using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFXTool.TFX
{
    class Playroutine
    {
        public PaulaChip Paula;
        TFXFile tfx;
        public byte[] sampledata;
        Track[] tracks;
        int trackstepPosition;
        public int tickCounter = 0;
        public int tempoCounter = 0;
        public int tempoNumber = 0;
        bool stopped = false;
        int songNo;
        int trackstepWait = 0;

        Channel[] channels;

        public int TrackstepPosition => trackstepPosition;
        public event EventHandler TrackstepPositionChanged;
        public event EventHandler TempoChanged;
        public event EventHandler<MacroStartEventArgs> MacroStart;
        public event EventHandler SongEnded;

        public void SetSong(int i)
        {
            songNo = i;
            trackstepPosition = tfx.SongStartPositions[i];
            tempoNumber = 1 + tfx.TempoNumbers[i];
            //Frequency = 55 / (1 + tfx.TempoNumbers[i]);
        }

        void Log(string text)
        {
            //Console.WriteLine(text);
        }

        /// <summary>
        /// performs a single song-position-advance
        /// </summary>
        void TrackStepExecute()
        {
            trackstepWait = 0;

            if(trackstepPosition > tfx.SongEndPositions[songNo] || trackstepPosition >= tfx.Tracksteps.Length)
            {
                Paula.Reset();

                if(trackstepPosition < tfx.Tracksteps.Length)
                    TrackstepPositionChanged?.Invoke(this, EventArgs.Empty);
                SongEnded?.Invoke(this, EventArgs.Empty);
                trackstepWait = 1;
                stopped = true;
                return;
            }

            if(tfx.Tracksteps[trackstepPosition][0] == 0xEFFE)
            {
                var line = tfx.Tracksteps[trackstepPosition];

                switch(line[1])
                {
                    case 0: // stop the player
                        stopped = true;
                        break;
                    case 1: // Play a section starting at position and ending here times
                            // times.  If times is 0000 then section will repeat forever.
                            // line+4=position, line+6=times
                        int position = line[2];
                        int times = line[3];

                        trackstepPosition = position; // TD: error-check, count loops

                        break;
                    case 2: // set the tempo
                            // TD
                        break;
                    case 3: // start a master volume slide (?)
                        if(line[3] == 0xEE)
                        {
                            --tempoNumber;
                            //Frequency = 14;
                            TempoChanged?.Invoke(this, EventArgs.Empty);
                        }
                        else if(line[3] == 0x10)
                        {
                            //Frequency = 30;
                            TempoChanged?.Invoke(this, EventArgs.Empty);
                        }
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }
            }
            else
            {
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

                    // TD: handle 0xFE correctly!
                    if(stepPattern >= 0x80)
                    {
                        if(stepPattern != 0x80)
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

                trackstepWait = 1;
            }

            TrackstepPositionChanged?.Invoke(this, EventArgs.Empty);

            ++trackstepPosition;
        }

        /// <summary>
        /// advances the song-position
        /// </summary>
        public void TrackStep()
        {
            trackstepWait = 0;
            while(trackstepWait == 0)
                TrackStepExecute();
        }

        void Tick()
        {
            do
            {
                while(trackstepWait == 0)
                    TrackStepExecute();

                if(stopped)
                    return;

                // track's patterns handler:
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
                                        trackstepWait = 0;
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
                                        // "Waits aa+1 jiffies"
                                        track.NumWait = step.Macro + 1;
                                    }
                                    else if(step.Note == 0xF4) // F4 xx xx xx	<STOP> disable track
                                    {
                                        track.PatternNo = -1;
                                        break;
                                    }
                                    else if(step.Note == 0xF7) // F7 aa bv cc	<Enve> envelope
                                    {
                                        // "Every b+1, slide channel v's volume aa towards cc." - that's really confusing, when in fact
                                        // it means, that every B+1 jiffies, add or subtract AA from channel[V]'s volume, to reach a value of CC eventually
                                        var channel = channels[step.Channel];
                                        channel.EnveCounter = 0;
                                        channel.EnveTarget = step.Detune;
                                        channel.EnvePeriod = step.Volume + 1;
                                        channel.EnveStep = step.Macro;
                                    }
                                }
                                else
                                {
                                    int note = -1, detune = 0;
                                    if(step.Note < 0x80) // note without wait
                                    {
                                        note = step.Note;
                                        detune = step.Detune; // step.Detune is actual pitch detune, presumably added to the period register
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
                                    // specs call the volume slot "relative volume", when there's nothing relative about it at all.
                                    // it simply means that the paula.volume (0..63) = volumeSlot * 3 
                                    var c = channels[step.Channel];
                                    c.Detune = detune;
                                    c.Note = note + track.Transpose;
                                    c.Volume = step.Volume * 3;
                                    c.MacroNo = step.Macro;
                                    c.MacroPC = 0;
                                    c.EnvePeriod = 0;
                                    c.EnveCounter = 0;
                                    c.EnveStep = 0;
                                    c.EnveTarget = 0;
                                    Log("starting macro " + c.MacroNo + " in tick " + tickCounter + " channel " + step.Channel);

                                    MacroStart?.Invoke(this, new MacroStartEventArgs { Channel = step.Channel, MacroNo = step.Macro });
                                }
                            }
                        }
                        if(track.NumWait > 0)
                            --track.NumWait;
                    }

                }


            }
            while(trackstepWait == 0);

            ++tickCounter;
        }

        /// <summary>
        /// "vertical blanking interrupt"
        /// advances the player-state by one jiffy
        /// </summary>
        public void VBI()
        {
            if(stopped)
                return;

            if(++tempoCounter >= tempoNumber)
            {
                tempoCounter = 0;
                Tick();
            }

            // channel handler (executes macro steps):
            for(int i = 0; i < 8; ++i)
            {
                var channel = channels[i];
                var paulaRegs = Paula.Channels[i];

                if(channel.MacroNo != -1)
                {
                    var macro = tfx.Macros[channel.MacroNo];

                    bool done = false;
                    while(!done)
                    {
                        var step = macro.Steps[channel.MacroPC++];

                        switch(step.Type)
                        {
                            case 0x0: // <DMAoff+Reset> stop efx and dma *
                                paulaRegs.Reset();
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
                                double fsmp = 520 * 4 * Math.Pow(2, (channel.Note + (sbyte)step.B) / 12d - channel.Detune / 255f);
                                paulaRegs.Period = (int)Math.Round(7159091 / fsmp / 2);
                                Log("channel " + i + " set period in tick " + tickCounter);
                                break;
                            case 0x9: // 09 aa bb bb	<SetNote> set freq direct *
                                fsmp = 520 * 4 * Math.Pow(2, step.B / 12d);
                                paulaRegs.Period = (int)Math.Round(7159091 / fsmp / 2);
                                break;
                            case 0xD: // 0D xx xx aa	<AddVolume> add volume
                                channel.Volume += step.D; // TD: add
                                break;
                            case 0xE: // 0E aa xx xx	<SetVolume> set volume
                                // commented because the "wait dma" commands don't actually wait, making the voice stop immediately
                                    //channel.Volume = step.B;
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

                if(channel.EnvePeriod != 0)
                {
                    if(++channel.EnveCounter >= channel.EnvePeriod)
                    {
                        channel.EnveCounter = 0;

                        if(channel.Volume < channel.EnveTarget)
                        {
                            channel.Volume = Math.Min(channel.EnveTarget, channel.Volume + channel.EnveStep);
                        }
                        else if(channel.Volume > channel.EnveTarget)
                        {
                            channel.Volume = Math.Max(channel.EnveTarget, channel.Volume - channel.EnveStep);
                        }
                    }
                }
                paulaRegs.Volume = Math.Min(channel.Volume, 63);
            }
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

        internal int Detune;
        
        internal string NoteName => TFXTool.Note.Format(Note);

        // volume-slide target value
        internal int EnveTarget;
        // step every X jiffies
        internal int EnvePeriod;
        // volume-slide value per step
        internal int EnveStep;
        internal int EnveCounter;


    }
}
