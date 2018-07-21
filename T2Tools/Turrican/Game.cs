using System;
using System.IO;

namespace T2Tools.Turrican
{
    class Game
    {
        public string Error { get; private set; } = "";
        public TOC Assets { get; private set; }
        public int TotalSize { get; private set; }

        private string inputFile;

        public Game(string file)
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

            TotalSize = (int)(new FileInfo(inputFile)).Length;

            try
            {
                Assets = AssetLoader.Load(inputFile, true);
            }
            catch(Exception e)
            {
                Error = e.Message;
                return false;
            }            

            return true;
        }
    }
}
