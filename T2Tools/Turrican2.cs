using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2Tools
{
    public class Turrican2
    {
        public byte[] Data { get; private set; }
        public string Error { get; private set; } = "";
        public TOC TOC { get; private set; }

        private string inputFile;

        public Turrican2(string file)
        {
            inputFile = file;
        }

        public bool Load()
        {
            if (!File.Exists(inputFile))
            {
                Error = $"input file '{inputFile}' not found";
                return false;
            }

            Data = File.ReadAllBytes(inputFile);

            TOC = AssetLoader.Load(Data);


            return true;
        }
    }
}
