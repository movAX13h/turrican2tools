using Audiolib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TFXTool
{
    public partial class PlayerForm : Form
    {
        AudioContext audio;
        TFXFile tfx;
        byte[] sampleData;
        Playback currentPlayback;

        public PlayerForm(string name, byte[] tfmxData, byte[] samData)
        {
            InitializeComponent();

            Text = "TFMX Player / " + name;
            
            audio = new AudioContext();
            tfx = new TFXFile(tfmxData);
            sampleData = samData;

            updateSongList();
            if (songList.Items.Count > 0)
            {
                songList.Focus();
                songList.Items[0].Selected = true;
            }

            var animationTimer = new Timer { Interval = 1000 / 60 };
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void playSong(int number)
        {
            if (currentPlayback != null)
            {
                currentPlayback.Stop();
                currentPlayback = null;
            }

            currentPlayback = new Playback(audio, tfx, sampleData);
            currentPlayback.Playroutine.TrackstepPositionChanged += Playroutine_TrackstepPositionChanged;
            currentPlayback.Playroutine.MacroStart += Playroutine_MacroStart;
            currentPlayback.Playroutine.SetSong(number);
            updateMuteState();
            currentPlayback.Start();
        }

        private void updateSongList()
        {
            songList.BeginUpdate();
            songList.Items.Clear();
            for (int i = 0; i < 32; i++)
            {
                var item = new ListViewItem(i.ToString());
                item.SubItems.Add(tfx.SongStartPositions[i].ToString());
                item.SubItems.Add(tfx.SongEndPositions[i].ToString());
                item.SubItems.Add(tfx.TempoNumbers[i].ToString());
                songList.Items.Add(item);
            }
            songList.EndUpdate();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            light1.Update();
            light2.Update();
            light3.Update();
            light4.Update();
            light5.Update();
            light6.Update();
            light7.Update();
            light8.Update();
        }

        private void updateMuteState()
        {
            if (currentPlayback != null)
            {
                currentPlayback.PaulaChip.Channels[0].Muted = checkBoxMute0.Checked;
                currentPlayback.PaulaChip.Channels[1].Muted = checkBoxMute1.Checked;
                currentPlayback.PaulaChip.Channels[2].Muted = checkBoxMute2.Checked;
                currentPlayback.PaulaChip.Channels[3].Muted = checkBoxMute3.Checked;
                currentPlayback.PaulaChip.Channels[4].Muted = checkBoxMute4.Checked;
                currentPlayback.PaulaChip.Channels[5].Muted = checkBoxMute5.Checked;
                currentPlayback.PaulaChip.Channels[6].Muted = checkBoxMute6.Checked;
                currentPlayback.PaulaChip.Channels[7].Muted = checkBoxMute7.Checked;
            }

            checkBoxMute0.BackColor = !checkBoxMute0.Checked ? Color.White : Color.Black;
            checkBoxMute1.BackColor = !checkBoxMute1.Checked ? Color.White : Color.Black;
            checkBoxMute2.BackColor = !checkBoxMute2.Checked ? Color.White : Color.Black;
            checkBoxMute3.BackColor = !checkBoxMute3.Checked ? Color.White : Color.Black;
            checkBoxMute4.BackColor = !checkBoxMute4.Checked ? Color.White : Color.Black;
            checkBoxMute5.BackColor = !checkBoxMute5.Checked ? Color.White : Color.Black;
            checkBoxMute6.BackColor = !checkBoxMute6.Checked ? Color.White : Color.Black;
            checkBoxMute7.BackColor = !checkBoxMute7.Checked ? Color.White : Color.Black;

            checkBoxMute0.ForeColor = checkBoxMute0.Checked ? Color.White : Color.Black;
            checkBoxMute1.ForeColor = checkBoxMute1.Checked ? Color.White : Color.Black;
            checkBoxMute2.ForeColor = checkBoxMute2.Checked ? Color.White : Color.Black;
            checkBoxMute3.ForeColor = checkBoxMute3.Checked ? Color.White : Color.Black;
            checkBoxMute4.ForeColor = checkBoxMute4.Checked ? Color.White : Color.Black;
            checkBoxMute5.ForeColor = checkBoxMute5.Checked ? Color.White : Color.Black;
            checkBoxMute6.ForeColor = checkBoxMute6.Checked ? Color.White : Color.Black;
            checkBoxMute7.ForeColor = checkBoxMute7.Checked ? Color.White : Color.Black;
        }

        private void Playroutine_MacroStart(object sender, MacroStartEventArgs e)
        {
            switch (e.Channel)
            {
                case 0: light1.val = 1; break;
                case 1: light2.val = 1; break;
                case 2: light3.val = 1; break;
                case 3: light4.val = 1; break;
                case 4: light5.val = 1; break;
                case 5: light6.val = 1; break;
                case 6: light7.val = 1; break;
                case 7: light8.val = 1; break;
            }
        }

        private void Playroutine_TrackstepPositionChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBoxMute_CheckedChanged(object sender, EventArgs e)
        {
            updateMuteState();
        }

        private void PlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (currentPlayback != null) currentPlayback.Stop();
        }

        private void songList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (songList.SelectedItems.Count == 0) return;
            playSong(int.Parse(songList.SelectedItems[0].Text));
        }
    }
}
