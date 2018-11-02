using Audiolib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/* written by srtuss */

namespace TFXTool
{
    public partial class EditorForm : Form
    {
        AudioContext actx;
        TFXFile tfx;
        string path;
        byte[] sampledata;

        

        public EditorForm()
        {
            InitializeComponent();

            Load += MusicTest_Load;
            FormClosing += MusicTest_FormClosing;
        }

        private void MusicTest_Load(object sender, EventArgs e)
        {
            actx = new AudioContext();

            tfx = new TFXFile();
            var m = new TFXMacro { Steps = new List<TFXMacroCommand>() };
            m.Steps.Add(new TFXMacroCommand(0x07000000));
            tfx.Macros.Add(m);
            var p = new TFXPattern { Steps = new List<TFXPatternCommand>() };
            p.Steps.Add(new TFXPatternCommand(0xF0000000));
            tfx.Patterns.Add(p);

            UpdateTrackstepView(tfx);
            UpdateSongView(tfx);
            numericUpDownPattern.Maximum = tfx.Patterns.Count - 1;
            numericUpDownMacro.Maximum = tfx.Macros.Count - 1;
            UpdateMacroView(tfx.Macros[(int)numericUpDownMacro.Value]);
            UpdatePatternView(tfx.Patterns[(int)numericUpDownPattern.Value]);
            textBoxMessage.Lines = new string[] { tfx.TextLines[0].TrimEnd(), tfx.TextLines[1].TrimEnd(), tfx.TextLines[2].TrimEnd(), tfx.TextLines[3].TrimEnd() };
            comboBoxPreviewNote.Items.Clear();
            for(int i = 0; i < 64; ++i)
                comboBoxPreviewNote.Items.Add(Note.Format(i));
            comboBoxPreviewNote.SelectedIndex = 42;
            sampledata = new byte[1];


            var def = @"..\..\..\game\unpacked\WORLD1.TFX";
            //def = "last.tfx";
            if(File.Exists(def))
                Open(def);

            var animationTimer = new Timer { Interval = 1000 / 60 };
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
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

        private void MusicTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(player != null)
                player.Stop();

            var n = "last.tfx";
            tfx.Save(n);
            File.WriteAllBytes(Path.ChangeExtension(n, ".sam"), sampledata);
        }

        void Open(string path)
        {
            this.path = path;
            Text = "TFMX Editor (experimental) - " + Path.GetFileName(path);

            sampledata = File.ReadAllBytes(Path.ChangeExtension(path, ".sam"));

            tfx = new TFXFile(path);
            /*txfm = new TFMXFile(@"..\..\assets\WORLD1.TFX");
            txfm = new TFMXFile(@"..\..\assets\WORLD2.TFX");
            txfm = new TFMXFile(@"..\..\assets\WORLD3.TFX");
            txfm = new TFMXFile(@"..\..\assets\WORLD4.TFX");
            txfm = new TFMXFile(@"..\..\assets\WORLD5.TFX");*/
            UpdateTrackstepView(tfx);
            UpdateSongView(tfx);

            numericUpDownPattern.Maximum = tfx.Patterns.Count - 1;
            numericUpDownMacro.Maximum = tfx.Macros.Count - 1;

            UpdatePatternView(tfx.Patterns[(int)numericUpDownPattern.Value]);
            UpdateMacroView(tfx.Macros[(int)numericUpDownMacro.Value]);

            textBoxMessage.Lines = new string[] { tfx.TextLines[0].TrimEnd(), tfx.TextLines[1].TrimEnd(), tfx.TextLines[2].TrimEnd(), tfx.TextLines[3].TrimEnd() };

            comboBoxPreviewNote.Items.Clear();
            for(int i = 0; i < 64; ++i)
                comboBoxPreviewNote.Items.Add(Note.Format(i));
            comboBoxPreviewNote.SelectedIndex = 42;

            var namesFile = Path.ChangeExtension(path, ".txt");
            if(File.Exists(namesFile))
            {
                using(var f = new StreamReader(namesFile))
                {
                    while(f.EndOfStream)
                    {
                        var ss = f.ReadLine().Split(' ');
                        switch(ss[0])
                        {
                            case "pattern":
                                tfx.Patterns[int.Parse(ss[1])].Name = ss[2];
                                break;
                            case "macro":
                                tfx.Macros[int.Parse(ss[1])].Name = ss[2];
                                break;
                        }
                    }
                }
            }
        }

        #region editing
        void UpdateSongView(TFXFile tfx)
        {
            listViewSongs.BeginUpdate();
            listViewSongs.Items.Clear();
            for(int i = 0; i < 32; ++i)
            {
                var item = new ListViewItem(i.ToString());
                item.SubItems.Add(tfx.SongStartPositions[i].ToString());
                item.SubItems.Add(tfx.SongEndPositions[i].ToString());
                item.SubItems.Add(tfx.TempoNumbers[i].ToString());
                listViewSongs.Items.Add(item);
            }
            listViewSongs.EndUpdate();
        }

        void UpdateTrackstepView(TFXFile tfx)
        {
            var listview = listViewTrackstep;
            listview.BeginUpdate();
            for(int i = 0; i < tfx.NumTracksteps; ++i)
            {
                var item = new ListViewItem(i.ToString());

                if(tfx.Tracksteps[i][0] == 0xEFFE)
                    item.BackColor = Color.Azure;

                for(int j = 0; j < 8; ++j)
                {
                    int v = tfx.Tracksteps[i][j];
                    item.SubItems.Add((v >> 8).ToString("X2") + "  " + (v & 0xFF).ToString("X2"));
                }
                if(i < listview.Items.Count)
                    listview.Items[i] = item;
                else
                    listview.Items.Add(item);
            }
            while(listview.Items.Count > tfx.NumTracksteps)
                listview.Items.RemoveAt(listview.Items.Count - 1);
            listview.EndUpdate();
        }

        void UpdatePatternView(TFXPattern pattern)
        {
            var listview = listViewPattern;
            listview.BeginUpdate();
            for(int i = 0; i < pattern.Steps.Count; ++i)
            {
                var step = pattern.Steps[i];

                int note = -1;
                if(step.Note < 0x80)
                    note = step.Note; // note without wait
                else if(step.Note < 0xC0)
                    note = step.Note - 0x80; // note with wait
                else if(step.Note < 0xF0)
                    note = step.Note - 0xC0; // note with portamento

                var item = new ListViewItem(i.ToString());
                item.SubItems.Add(step.Note.ToString("X2"));
                string status = "...";
                switch(step.Note)
                {
                    case 0xF0: status = "End"; break;
                    case 0xF1: status = "Loop"; break;
                    case 0xF2: status = "Jump"; break;
                    case 0xF3: status = "Wait"; break;
                    case 0xF4: status = "STOP"; break;
                    case 0xF5: status = "Kup^"; break;
                    case 0xF6: status = "Vibr"; break;
                    case 0xF7: status = "Enve"; break;
                    case 0xF8: status = "GsPt"; break;
                    case 0xF9: status = "RoPt"; break;
                    case 0xFA: status = "Fade"; break;
                    case 0xFB: status = "PPat"; break;
                    case 0xFC: status = "Port"; break;
                    case 0xFD: status = "Lock"; break;
                    case 0xFE: status = "StCu"; break;
                    case 0xFF: status = "NOP!"; break;
                    default:
                        status = note >= 0 ? Note.Format(note) : "-";
                        break;
                }
                item.SubItems.Add(status);
                item.SubItems.Add(step.Macro.ToString("X2"));
                item.SubItems.Add("...");
                item.SubItems.Add(step.Volume.ToString("X"));
                item.SubItems.Add(step.Channel.ToString("X"));
                item.SubItems.Add(step.Detune.ToString("X2"));
                if(i < listview.Items.Count)
                    listview.Items[i] = item;
                else
                   listview.Items.Add(item);
            }
            while(listview.Items.Count > pattern.Steps.Count)
                listview.Items.RemoveAt(listview.Items.Count - 1);
            listview.EndUpdate();
        }

        void UpdateMacroView(TFXMacro macro)
        {
            var listview = listViewMacro;
            listview.BeginUpdate();
            for(int i = 0; i < macro.Steps.Count; ++i)
            {
                var step = macro.Steps[i];
                /*if(cmd.A == 2) // set beginning of sample
                {
                    Console.WriteLine(cmd.Data & 0xFFFFFF);
                }*/

                var item = new ListViewItem(i.ToString());
                item.SubItems.Add(step.Type.ToString("X2"));
                string desc = "???";
                switch(step.Type)
                {
                    case 0x00: desc = "<DMAoff+Reset> (stop sample & reset all)"; break;
                    case 0x01: desc = "<DMAon> (start sample at selected, begin)"; break;
                    case 0x02: desc = "<SetBegin> (set beginning of sample)"; break;
                    case 0x03: desc = "<SetLen> ..xxxx samples"; break;
                    case 0x04: desc = "<Wait> ..xxxx count (VBI's)"; break;
                    case 0x05: desc = "<Loop> xx/xxxx times/from"; break;
                    case 0x06: desc = "<Cont> xx/xxxx macro/startingpoint"; break;
                    case 0x07: desc = "--- STOP -------------------------------"; break;
                    case 0x08: desc = "<AddNote> xx/xxxx transpose/finetune"; break;
                    case 0x09: desc = "<SetNote> xx/xxxx note/finetune"; break;
                    case 0x0A: desc = "<Reset> clear all effects"; break;
                    case 0x0B: desc = "<Portamento> xx/xxxx speed/mult"; break;
                    case 0x0C: desc = "<Vibrato> xx/../xx speed/intensity"; break;
                    case 0x0D: desc = "<AddVolume> ....xx volume 00-3F"; break;
                    case 0x0E: desc = "<SetVolume> xx.... volume 00-3F"; break;
                    case 0x0F: desc = "<Envelope> xx/xx/xx speed/count/endvol"; break;
                    case 0x11: desc = "<AddBegin> xx/xxxx jiffies/samples"; break;
                    case 0x12: desc = "<AddLen> ..xxxx relative loop length"; break;
                    case 0x13: desc = "<DMAoff> xx...."; break;
                    case 0x14: desc = "<Wait key up> ....xx count (VBI's)"; break;
                    case 0x15: desc = "<Go submacro> xx/xxxx macro/startingpoint"; break;
                    case 0x17: desc = "<Set period> ..xxxx period"; break;
                    case 0x18: desc = "<Sampleloop> xxxxxx"; break;
                    case 0x19: desc = "<Set one shot sample>"; break;
                    case 0x1A: desc = "<Wait on DMA> ..xxxx loopcount"; break;
                    case 0x1F: desc = "<SetPrefNote> xx/xxxx transpose/finetune"; break;
                    case 0x22: desc = "SID setbeg  xxxxxx sample-startadress"; break;
                    case 0x23: desc = "SID setlen xx/xxxx buflen/sourcelen"; break;
                    case 0x24: desc = "SID op3 ofs xxxxxx offset"; break;
                    case 0x25: desc = "SID op3 frq xx/xxxx speed/amplitude"; break;
                    case 0x26: desc = "SID op2 ofs xxxxxx offset"; break;
                    case 0x27: desc = "SID op2 frq xx/xxxx speed/amplitude"; break;
                    case 0x29: desc = "SID op1 xx/xx/xx speed/amplitude/TC"; break;
                    case 0x20: desc = "SID stop xx....flag(1=clear all)"; break;
                }
                item.SubItems.Add(desc);
                item.SubItems.Add((step.Data & 0xFFFFFF).ToString("X6"));

                if(i < listview.Items.Count)
                    listview.Items[i] = item;
                else
                    listview.Items.Add(item);
            }
            while(listview.Items.Count > macro.Steps.Count)
                listview.Items.RemoveAt(listview.Items.Count - 1);
            listview.EndUpdate();
        }



        private void numericUpDownPattern_ValueChanged(object sender, EventArgs e)
        {
            UpdatePatternView(tfx.Patterns[(int)numericUpDownPattern.Value]);
        }

        private void numericUpDownMacro_ValueChanged(object sender, EventArgs e)
        {
            UpdateMacroView(tfx.Macros[(int)numericUpDownMacro.Value]);
        }

        delegate void EditifyHandler_T(ListViewItem item, ListViewItem.ListViewSubItem subitem, string value);

        void AutoEditify(ListView control, EditifyHandler_T handler)
        {
            var cp = Cursor.Position;
            var l = control.PointToClient(cp);
            var item = control.GetItemAt(l.X, l.Y);
            if(item != null)
            {
                var subitem = item.GetSubItemAt(l.X, l.Y);
                if(subitem != null)
                {
                    var l2 = PointToClient(cp);
                    var editifier = new Editifier { Left = l2.X, Top = l2.Y, Width = 100, Text = subitem.Text };
                    editifier.KeyDown += (s, ee) =>
                    {
                        if(ee.KeyCode == Keys.Escape)
                            Controls.Remove(editifier);
                    };
                    editifier.LostFocus += (s, ee) =>
                    {
                        Controls.Remove(editifier);
                    };
                    editifier.EnterPressed += (s, ee) =>
                    {
                        try
                        {
                            handler(item, subitem, editifier.Text);
                        }
                        catch
                        {

                        }
                        Controls.Remove(editifier);
                    };
                    Controls.Add(editifier);
                    editifier.BringToFront();
                    editifier.Select();
                }
            }
        }

        private void listViewTrackstep_DoubleClick(object sender, EventArgs e)
        {
            AutoEditify((ListView)sender, (item, subitem, value) =>
            {
                int index = item.SubItems.IndexOf(subitem);
                if(index > 0)
                {
                    var ss = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int a = int.Parse(ss[0], System.Globalization.NumberStyles.HexNumber);
                    int b = int.Parse(ss[1], System.Globalization.NumberStyles.HexNumber);

                    tfx.Tracksteps[int.Parse(item.Text)][index - 1] = (ushort)(a * 256 + b);

                    UpdateTrackstepView(tfx);

                    //subitem.Text = a.ToString("X2") + "  " + b.ToString("X2");
                }
            });
        }

        class Editifier : TextBox
        {
            public event EventHandler EnterPressed;
            protected override bool ProcessCmdKey(ref Message m, Keys keyData)
            {
                if(keyData == Keys.Enter)
                {
                    if(EnterPressed != null)
                    {
                        EnterPressed.Invoke(this, EventArgs.Empty);
                        return true;
                    }
                }
                return base.ProcessCmdKey(ref m, keyData);
            }
        }


        private void listViewMacro_DoubleClick(object sender, EventArgs e)
        {
            AutoEditify((ListView)sender, (item, subitem, value) =>
            {
                int index = item.SubItems.IndexOf(subitem);
                if(index == 1)
                {
                    var macro = tfx.Macros[(int)numericUpDownMacro.Value];
                    macro.Steps[int.Parse(item.Text)].Type = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                    UpdateMacroView(macro);
                }
                else if(index == 3)
                {
                    var macro = tfx.Macros[(int)numericUpDownMacro.Value];
                    macro.Steps[int.Parse(item.Text)].Parameter = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                    UpdateMacroView(macro);
                }
            });
        }

        private void listViewPattern_DoubleClick(object sender, EventArgs e)
        {
            AutoEditify((ListView)sender, (item, subitem, value) =>
            {
                int index = item.SubItems.IndexOf(subitem);
                var pattern = tfx.Patterns[(int)numericUpDownPattern.Value];
                var step = pattern.Steps[int.Parse(item.Text)];
                if(index == 1)
                {
                    step.Note = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                    UpdatePatternView(pattern);
                }
                else if(index == 3)
                {
                    step.Macro = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                    UpdatePatternView(pattern);
                }
                else if(index == 5)
                {
                    step.Volume = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                    UpdatePatternView(pattern);
                }
                else if(index == 6)
                {
                    step.Channel = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                    UpdatePatternView(pattern);
                }
                else if(index == 7)
                {
                    step.Detune = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                    UpdatePatternView(pattern);
                }
            });
        }

        private void listViewMacro_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
                contextMenuStrip1.Show((Control)sender, e.Location);
        }

        private void listViewPattern_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
                contextMenuStrip1.Show((Control)sender, e.Location);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var c = contextMenuStrip1.SourceControl;
            if(c is ListView lv)
            {
                if(lv == listViewMacro)
                {
                    var macro = tfx.Macros[(int)numericUpDownMacro.Value];

                    int i = int.Parse(lv.SelectedItems[0].Text);
                    macro.Steps.RemoveAt(i);

                    UpdateMacroView(macro);
                }
                else if(lv == listViewPattern)
                {
                    var pattern = tfx.Patterns[(int)numericUpDownPattern.Value];

                    int i = int.Parse(lv.SelectedItems[0].Text);
                    pattern.Steps.RemoveAt(i);

                    UpdatePatternView(pattern);
                }
            }
        }

        private void insertNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var c = contextMenuStrip1.SourceControl;
            if(c is ListView lv)
            {
                if(lv == listViewMacro)
                {
                    var macro = tfx.Macros[(int)numericUpDownMacro.Value];

                    int i = int.Parse(lv.SelectedItems[0].Text);
                    macro.Steps.Insert(i, new TFXMacroCommand(0));

                    UpdateMacroView(macro);
                }
                else if(lv == listViewPattern)
                {
                    var pattern = tfx.Patterns[(int)numericUpDownPattern.Value];

                    int i = int.Parse(lv.SelectedItems[0].Text);
                    pattern.Steps.Insert(i, new TFXPatternCommand(0));

                    UpdatePatternView(pattern);
                }
            }
        }


        private void listViewSongs_DoubleClick(object sender, EventArgs e)
        {
            AutoEditify((ListView)sender, (item, subitem, value) =>
            {
                int index = item.SubItems.IndexOf(subitem);
                if(index == 1)
                {
                    tfx.SongStartPositions[int.Parse(item.Text)] = ushort.Parse(value);

                    UpdateSongView(tfx);
                }
                else if(index == 2)
                {
                    tfx.SongEndPositions[int.Parse(item.Text)] = ushort.Parse(value);

                    UpdateSongView(tfx);
                }
                else if(index == 3)
                {
                    tfx.TempoNumbers[int.Parse(item.Text)] = ushort.Parse(value);

                    UpdateSongView(tfx);
                }
            });
        }

        #endregion

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "TFX module|*.TFX|All Files|*.*" };
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                Open(ofd.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog { Filter = "TFX module|*.TFX" };
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                tfx.Save(sfd.FileName);
                File.WriteAllBytes(Path.ChangeExtension(sfd.FileName, ".sam"), sampledata);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData == (Keys.Control | Keys.O))
            {
                openToolStripMenuItem_Click(this, EventArgs.Empty);
                return true;
            }
            else if(keyData == (Keys.Control | Keys.S))
            {
                saveToolStripMenuItem_Click(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        TfmxplayPlayback player;
        string btx;
        private void button1_Click(object sender, EventArgs e)
        {
            if(player != null)
            {
                player.Stop();
                player = null;
                button1.Text = btx;
            }
            else
            {
                btx = button1.Text;
                button1.Text = "Stop";
                player = new TfmxplayPlayback();
                try
                {
                    player.Start(tfx, sampledata, (int)numericUpDownSubsong.Value);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        PaulaChip activePaulaChip = null;
        private void button2_Click(object sender, EventArgs e)
        {
            if(activePaulaChip != null)
            {
                activePaulaChip.Disconnect();
                activePaulaChip = null;


                button2.Text = "Play";

                return;
            }

            var paula = new PaulaChip(actx);
            paula.Connect(actx.Destination);
            
            var c = paula.Channels[0];
            c.Data = sampledata;
            c.Volume = 40;
            c.StartLoopByte = 0;
            c.OneShot = false;

            var macro = tfx.Macros[(int)numericUpDownMacro.Value];


            int patternNote = comboBoxPreviewNote.SelectedIndex;

            for(int i = 0; i < macro.Steps.Count; ++i)
            {
                var step = macro.Steps[i];
                switch(step.Type)
                {
                    case 0x2: // 02 aa aa aa	<SetBegin> set beginning of sample
                        c.StartByte = step.Parameter;
                        break;
                    case 0x3: // 03 xx aa aa	<SetLen> set length
                        c.LengthWords = step.Parameter & 0xFFFF;
                        break;
                    case 0x8: // 08 aa bb bb	<AddNote> set freq by this note *
                        double fsmp = 520 * 4 * Math.Pow(2, (patternNote + (sbyte)step.B) / 12d);
                        c.Period = (int)Math.Round(7159091 / fsmp / 2);
                        break;
                    case 0x9: // 09 aa bb bb	<SetNote> set freq direct *
                        fsmp = 520 * 4 * Math.Pow(2, step.B / 12d);
                        c.Period = (int)Math.Round(7159091 / fsmp / 2);
                        break;
                    case 0x17: // 17 xx aa aa	<Set period> Absolute period *
                        c.Period = step.Parameter & 0xFFFF;
                        break;
                    case 0x18: // <Sampleloop> set sample loop
                        c.StartLoopByte = step.Parameter;
                        break;
                    case 0x19: // 19 xx xx xx	<Set one shot sample>
                        c.OneShot = true;
                        break;
                    
                }
            }

            activePaulaChip = paula;
            /*var tmr = new Timer { Interval = 2000 };
            tmr.Tick += (s, ee) =>
            {
                tmr.Stop();
                paula.Disconnect();
            };
            tmr.Start();*/

            button2.Text = "Stop";
        }

        Playback currentPlayback;
        ListViewItem lviLastTrackstepHighlight;
        string btx2;
        private void button3_Click(object sender, EventArgs e)
        {
            if(currentPlayback != null)
            {
                currentPlayback.Stop();
                currentPlayback = null;

                button3.Text = btx2;

                if(lviLastTrackstepHighlight != null)
                    lviLastTrackstepHighlight.BackColor = Color.White;
            }
            else
            {
                currentPlayback = new Playback(actx, tfx, sampledata);
                currentPlayback.Playroutine.TrackstepPositionChanged += Playroutine_TrackstepPositionChanged;
                currentPlayback.Playroutine.MacroStart += Playroutine_MacroStart;
                currentPlayback.Playroutine.SetSong((int)numericUpDownSubsong.Value);
                currentPlayback.PaulaChip.Channels[0].Muted = checkBoxMute0.Checked;
                currentPlayback.PaulaChip.Channels[1].Muted = checkBoxMute1.Checked;
                currentPlayback.PaulaChip.Channels[2].Muted = checkBoxMute2.Checked;
                currentPlayback.PaulaChip.Channels[3].Muted = checkBoxMute3.Checked;
                currentPlayback.PaulaChip.Channels[4].Muted = checkBoxMute4.Checked;
                currentPlayback.PaulaChip.Channels[5].Muted = checkBoxMute5.Checked;
                currentPlayback.PaulaChip.Channels[6].Muted = checkBoxMute6.Checked;
                currentPlayback.PaulaChip.Channels[7].Muted = checkBoxMute7.Checked;
                currentPlayback.Start();
                btx2 = button3.Text;
                button3.Text = "Stop";
            }
        }

        private void Playroutine_MacroStart(object sender, MacroStartEventArgs e)
        {
            switch(e.Channel)
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
            if(lviLastTrackstepHighlight != null)
                lviLastTrackstepHighlight.BackColor = Color.White;

            var item = listViewTrackstep.Items[currentPlayback.Playroutine.TrackstepPosition];
            item.BackColor = Color.LightGray;
            item.EnsureVisible();

            lviLastTrackstepHighlight = item;
        }

        private void checkBoxChX_CheckedChanged(object sender, EventArgs e)
        {
            if(currentPlayback != null)
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
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(currentPlayback != null)
                currentPlayback.Playroutine.TrackStep();
        }
    }

    class Note
    {
        static string[] s = { "C-", "C#", "D-", "D#", "E-", "F-", "F#", "G-", "G#", "A-", "A#", "B-" };
        public static string Format(int i)
        {
            i += 6;

            int o = i / 12;
            i -= o * 12;

            return s[i] + o.ToString();
        }
        public static bool TryParse(string v, out int i)
        {
            i = Array.FindIndex(s, o => o[0] == v[0] && o[1] == v[1]);
            if(i < 0)
                return false;
            i += int.Parse(v.Substring(2)) * 12;

            i -= 6;
            return true;
        }
    }
}
