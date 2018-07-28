using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using T2Tools.Formats;

namespace T2Tools.Turrican
{
    class MapMaker
    {
        private BackgroundWorker worker;
        private Action<int> progressCallback;
        private Action<Bitmap> completeCallback;
        private TOC assets;
        public string Error { get; private set; } = "";

        private TOCEntry mapEntry;
        private TOCEntry tilesetEntry;
        private TOCEntry paletteEntry;

        private Bitmap resultBitmap;

        public PCMFile Map { get; private set; }

        public MapMaker(TOC assets, Action<int> progressCallback, Action<Bitmap> completeCallback)
        {
            this.assets = assets;
            this.progressCallback = progressCallback;
            this.completeCallback = completeCallback;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += make;
            worker.ProgressChanged += reportProgress;
            worker.RunWorkerCompleted += reportResult;
        }
        
        public void Cancel()
        {
            if (worker.IsBusy) worker.CancelAsync();
        }

        public bool Make(TOCEntry entry)
        {
            mapEntry = entry;
            Error = "";

            // level number
            string mapName = mapEntry.Name;
            if (!int.TryParse(mapName.Substring(1, 1), out int levelNumber))
            {
                Error = $"unable to derive level number from map name {mapName}";
                return false;
            }

            if (levelNumber == 6) levelNumber = 5;

            // tileset
            string tilesetName = $"BLOCK{levelNumber}.PIC";
            if (!assets.Entries.ContainsKey(tilesetName))
            {
                Error = $"tileset {tilesetName} not found";
                return false;
            }
            tilesetEntry = assets.Entries[tilesetName];

            // palette 
            string palName = $"WORLD{levelNumber}.PAL";
            if (!assets.Entries.ContainsKey(palName))
            {
                Error = $"palette {palName} not found";
                return false;
            }
            paletteEntry = assets.Entries[palName];
            
            worker.RunWorkerAsync();

            return true;
        }

        private void make(object sender, DoWorkEventArgs e)
        {
            try
            {
                make();
            }
            catch(Exception ex)
            {
                Error = ex.Message;
                return;
            }
        }

        private void make()
        { 
            worker.ReportProgress(0);

            Map = new PCMFile(mapEntry.Data);

            worker.ReportProgress(10);
            if (worker.CancellationPending) return;

            Bitmap[] tiles = PICConverter.PICToBitmaps(tilesetEntry.Data, paletteEntry.Data);

            worker.ReportProgress(40);
            if (worker.CancellationPending) return;

            resultBitmap = new Bitmap(Game.TileSize * Map.Width, Game.TileSize * Map.Height);
            Graphics gfx = Graphics.FromImage(resultBitmap);

            int total = Map.Width * Map.Height;

            for(int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    int id = x + y * Map.Width;
                    int tileId = Map.TilesIndices[y, x];

                    Bitmap tile = tiles[tileId];
                    gfx.DrawImage(tile, x * Game.TileSize, y * Game.TileSize, Game.TileSize, Game.TileSize);
                    worker.ReportProgress(40 + (int)Math.Round(60f * id / total));

                    if (worker.CancellationPending)
                    {
                        Error = "Cancelled";
                        goto end;
                    }
                }
            }

            end:
            gfx.Dispose();
        }

        private void reportProgress(object sender, ProgressChangedEventArgs e)
        {
            if (!worker.CancellationPending)
                progressCallback(e.ProgressPercentage);
        }

        private void reportResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (worker.CancellationPending) resultBitmap.Dispose();
            else completeCallback(string.IsNullOrEmpty(Error) ? resultBitmap : null);
        }

    }
}
