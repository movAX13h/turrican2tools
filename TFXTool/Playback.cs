using Audiolib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using T2Tools.Formats;

namespace TFXTool
{
    class Playback
    {
        AudioContext actx;
        Timer timer;
        public PaulaChip PaulaChip;
        public Playroutine Playroutine;
        public Playback(AudioContext actx, TFXFile tfx, byte[] sampledata)
        {
            this.actx = actx;
            PaulaChip = new PaulaChip(actx);


            Playroutine = new Playroutine(tfx);
            Playroutine.sampledata = sampledata;
            Playroutine.Paula = PaulaChip;
            Playroutine.TrackstepPositionChanged += Playroutine_TrackstepPositionChanged;
            Playroutine.TempoChanged += Playroutine_TempoChanged;


            timer = new Timer();
            timer.Tick += (s, ee) =>
            {
                Playroutine.VBI();

            };

        }

        private void Playroutine_TrackstepPositionChanged(object sender, EventArgs e)
        {
            Console.WriteLine("trackstep pos " + Playroutine.TrackstepPosition + " in tick " + Playroutine.tickCounter);
        }

        private void Playroutine_TempoChanged(object sender, EventArgs e)
        {
            timer.Interval = 1000 / Playroutine.Frequency;
        }

        public void Start()
        {
            timer.Interval = 1000 / Playroutine.Frequency;
            PaulaChip.Connect(actx.Destination);
            timer.Start();
        }
        public void Stop()
        {
            PaulaChip.Disconnect();
            timer.Stop();
        }
    }
}
