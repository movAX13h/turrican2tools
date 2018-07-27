using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFXTool
{
    /// <summary>
    /// invokes tfmxplay.exe to play tfx songs
    /// </summary>
    class TfmxplayPlayback
    {
        string playerPath = "tfmxplay.exe";
        bool started = false;
        Process process;

        public void Start(TFXFile tfx, byte[] sampledata, int subsong)
        {
            if (!File.Exists(playerPath)) throw new Exception($"tfmx player '{playerPath}' not found");

            if (started) throw new Exception("already started");
            started = true;

            var tmp = Path.GetTempFileName();
            var tfxpath = Path.ChangeExtension(tmp, ".tfx");
            var sampath = Path.ChangeExtension(tmp, ".sam");
            tfx.Save(tfxpath);
            File.WriteAllBytes(sampath, sampledata);
            var sui = new ProcessStartInfo
            {
                FileName = playerPath,
                Arguments = (subsong != 0 ? "-p " + subsong : "") + " -b 1 " + tfxpath,
                WindowStyle = ProcessWindowStyle.Minimized
            };
            process = Process.Start(sui);
        }

        public void Stop()
        {
            if(process != null)
            {
                if(!process.HasExited)
                    process.Kill();
                process = null;
            }
        }
    }
}
