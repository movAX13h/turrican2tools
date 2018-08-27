using Audiolib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// useful information about the Paula chip
// http://ada.untergrund.net/?p=boardthread&id=40
// http://amigadev.elowar.com/read/ADCD_2.1/Hardware_Manual_guide/node0060.html
// http://www.polynominal.com/Commodore-Amiga/commodore-amiga-500-paula.htm

namespace TFXTool.TFX
{
    public class PaulaChip : CodeProcessorNode
    {
        AudioContext audioContext;
        int numChannels = 8;
        public Channel[] Channels;
        int fcpu = 7159091;
        float mixVolume = .17f;
        int interruptCounter = 0;
        double interruptFrequency = 50;
        public event EventHandler Interrupt;

        public PaulaChip(AudioContext audioContext) : base(audioContext)
        {
            this.audioContext = audioContext;

            Channels = new Channel[numChannels];
            for(int i = 0; i < numChannels; ++i)
            {
                Channels[i] = new Channel();
                Channels[i].Pan = (((i + 1) & 2) == 0) ? .5f : -.5f;
            }

            AudioProcess += PaulaChip_AudioProcess;
        }

        void ClockChip(float[] addLeft, float[] addRight, int offset, int numFrames)
        {
            float cpuCyclesPerFrame = (float)(7159091 / audioContext.Samplerate);

            for(int i = 0; i < numChannels; ++i)
            {
                var channel = Channels[i];

                if(channel.Muted)
                    continue;

                if(channel.LengthWords > 0 && channel.Period > 0)
                {
                    float vmul = (channel.Volume & 63) / 63f;
                    vmul *= mixVolume;

                    float vmulLeft = vmul * (1 - Math.Max(channel.Pan, 0));
                    float vmulRight = vmul * (1 - Math.Max(-channel.Pan, 0));

                    int period = channel.Period * 2;
                    for(int j = 0; j < numFrames; ++j)
                    {

                        var fv = (sbyte)channel.Data[Math.Min(channel.StartByte + channel.Position, channel.Data.Length - 1)] / 128f;
                        addLeft[offset + j] += fv * vmulLeft;
                        addRight[offset + j] += fv * vmulRight;

                        while(channel.Prp >= period)
                        {
                            if(++channel.Position >= channel.LengthWords * 2)
                            {
                                if(channel.OneShot)
                                {
                                    channel.Period = 0;
                                    break;
                                }
                                else
                                    channel.Position = channel.StartLoopByte;

                            }
                            channel.Prp -= period;
                        }
                        channel.Prp += cpuCyclesPerFrame;

                        if(channel.Period == 0)
                            break;
                    }
                }
            }
        }

        private void PaulaChip_AudioProcess(object sender, AudioProcessEventArgs e)
        {
            for(int pos = 0, td; pos < e.NumFrames; pos += td)
            {
                if(interruptCounter <= 0)
                {
                    Interrupt?.Invoke(this, EventArgs.Empty);
                    interruptCounter = (int)Math.Round(audioContext.Samplerate / interruptFrequency);
                }
                td = Math.Min(interruptCounter, e.NumFrames - pos);
                ClockChip(e.OutAdd[0], e.OutAdd[1], pos, td);
                interruptCounter -= td;
            }
        }

        public void Reset()
        {
            foreach(var channel in Channels)
                channel.Reset();
        }

        public class Channel
        {
            // fs = fcpu / period / 2

            public bool Muted;
            public float Pan;

            public byte[] Data;
            public float Prp;
            public int Position;
            public int StartByte;
            /// <summary>
            /// not an actual register
            /// </summary>
            public int StartLoopByte;
            /// <summary>
            /// not an actual register
            /// </summary>
            public bool OneShot;
            /// <summary>
            /// in words
            /// </summary>
            public int LengthWords;
            public int Period;
            public int Volume;

            public void Reset()
            {
                Prp = 0;
                Position = 0;
                Period = 0;
                StartByte = 0;
                StartLoopByte = 0;
                OneShot = false;
                Volume = 0;
                LengthWords = 0;
            }
        }
    }
}
