using Audiolib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TFXTool
{
    public partial class PlayerForm : Form
    {
        private AudioContext audio;
        private TFXFile tfx;
        private byte[] sampleData;
        private Playback currentPlayback;
        private int currentSongNumber;

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
            currentSongNumber = number;

            if (currentPlayback != null)
            {
                currentPlayback.Stop();
                currentPlayback = null;
            }
            
            try
            {
                currentPlayback = new Playback(audio, tfx, sampleData);
                currentPlayback.Playroutine.TrackstepPositionChanged += Playroutine_TrackstepPositionChanged;
                currentPlayback.Playroutine.MacroStart += Playroutine_MacroStart;
                currentPlayback.Playroutine.SongEnded += Playroutine_SongEnded;
                currentPlayback.Playroutine.SetSong(number);
                updateMuteState();
                currentPlayback.Start();

                songLabel.Text = (currentSongNumber + 1).ToString();
                patternLabel.Text = "Pattern: ...";
            }
            catch
            {
                playNextSong();
            }
        }

        private void playNextSong()
        {
            if (songList.CheckedItems.Count == 0)
            {
                MessageBox.Show("Unable to play next song because there are no songs checked!", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ListViewItem item;

            do
            {
                currentSongNumber++;
                if (currentSongNumber > songList.Items.Count - 1) currentSongNumber = 0;
                item = songList.Items[currentSongNumber];
            }
            while (!item.Checked);

            songList.SelectedItems.Clear();
            item.Selected = true;
            item.EnsureVisible();
        }
        
        private void playPrevSong()
        {
            if (songList.CheckedItems.Count == 0)
            {
                MessageBox.Show("Unable to play previous song because there are no songs checked!", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ListViewItem item;

            do
            {
                currentSongNumber--;
                if (currentSongNumber < 0) currentSongNumber = songList.Items.Count - 1;
                item = songList.Items[currentSongNumber];
            }
            while (!item.Checked);

            songList.SelectedItems.Clear();
            item.Selected = true;
        }

        private void updateSongList()
        {
            songList.BeginUpdate();
            songList.Items.Clear();
            for (int i = 0; i < 31; i++)
            {
                ushort start = tfx.SongStartPositions[i];
                ushort end = tfx.SongEndPositions[i];
                ushort tempo = tfx.TempoNumbers[i];

                var item = new ListViewItem(i.ToString());
                item.SubItems.Add(start.ToString());
                item.SubItems.Add(end.ToString());
                item.SubItems.Add(tempo.ToString());
                item.Checked = !(start == 0 && end == 0);
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
            patternLabel.InvokeIfRequired(() =>
            {
                patternLabel.Text = "Pattern: " + currentPlayback.Playroutine.TrackstepPosition.ToString();
            });
        }

        private void Playroutine_SongEnded(object sender, EventArgs e)
        {
            playNextSong();
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

        private void nextButton_Click(object sender, EventArgs e)
        {
            playNextSong();
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            playPrevSong();
        }
    }
}
