using System;
using System.IO;
using T2Tools.Utils;

namespace T2Tools.Turrican
{
    class Game
    {
        public static int TileSize = 16;

        private string descriptionFile = "descriptions.json";
        public DescriptionsList Descriptions { get; private set; }

        public string Error { get; private set; } = "";
        public TOC Assets { get; private set; }

        public byte[] LoadedData { get; private set; }
        public int NumBytesLoaded { get { return LoadedData.Length; } }

        private string inputFile;        

        public Game(string file)
        {
            inputFile = file;
        }

        public bool Load()
        {
            if (File.Exists(descriptionFile))
            {
                Descriptions = FileUtils.ObjectFromJsonFile<DescriptionsList>(descriptionFile);
            }

            if (!File.Exists(inputFile))
            {
                Error = $"input file '{inputFile}' not found";
                return false;
            }

            LoadedData = File.ReadAllBytes(inputFile);

            try
            {
                Assets = AssetLoader.Load(inputFile, false);
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
