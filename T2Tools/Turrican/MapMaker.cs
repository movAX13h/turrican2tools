﻿using System;
using System.ComponentModel;
using System.Drawing;
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

        public MapMaker(TOC assets, Action<int> progressCallback, Action<Bitmap> completeCallback)
        {
            this.assets = assets;
            this.progressCallback = progressCallback;
            this.completeCallback = completeCallback;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += make;
            worker.ProgressChanged += reportProgress;
            worker.RunWorkerCompleted += reportResult;
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

        public void Cancel()
        {
            if (worker.IsBusy) worker.CancelAsync();
        }

        private void make(object sender, DoWorkEventArgs e)
        {
            //try
            {
                make();
            }/*
            catch(Exception ex)
            {
                Error = ex.Message;
                return;
            }*/
        }

        private void make()
        { 
            worker.ReportProgress(0);

            PCMFile map = new PCMFile(mapEntry.Data);

            worker.ReportProgress(10);

            Bitmap[] tiles = BlockPicConverter.BlockPicToBitmaps(tilesetEntry.Data, paletteEntry.Data);

            worker.ReportProgress(20);

            resultBitmap = new Bitmap(Game.TileSize * map.Width, Game.TileSize * map.Height);
            Graphics gfx = Graphics.FromImage(resultBitmap);

            int total = map.Width * map.Height;

            for(int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    int id = x + y * map.Width;
                    int tileId = map.TilesIndices[y, x];

                    Bitmap tile = tiles[tileId];
                    gfx.DrawImage(tile, x * Game.TileSize, y * Game.TileSize, Game.TileSize, Game.TileSize);
                    worker.ReportProgress(20 + 80 * id / total);
                }
            }

            gfx.Dispose();
        }

        private void reportProgress(object sender, ProgressChangedEventArgs e)
        {
            progressCallback(e.ProgressPercentage);
        }

        private void reportResult(object sender, RunWorkerCompletedEventArgs e)
        {
            completeCallback(string.IsNullOrEmpty(Error) ? resultBitmap : null);
        }

    }
}