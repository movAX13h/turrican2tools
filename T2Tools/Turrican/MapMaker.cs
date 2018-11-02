using System;
using System.ComponentModel;
using System.Drawing;
using T2Tools.Formats;

namespace T2Tools.Turrican
{
    class MapMaker
    {
        private BackgroundWorker worker;
        private Action<int> progressCallback;
        private Action<bool> completeCallback;
        private TOC assets;
        public string Error { get; private set; } = "";

        private TOCEntry mapEntry; // map file (for example: L1-1.PCM)
        private TOCEntry tilesetEntry;
        private TOCEntry paletteEntry;
        private TOCEntry entitiesEntry;
        private TOCEntry collisionsEntry;

        public Bitmap TilesBitmap { get; private set; }
        public Bitmap CollisionsBitmap { get; private set; }
        public Bitmap EntitiesBitmap { get; private set; }
        public Bitmap GridBitmap { get; private set; }

        public PCMFile Map { get; private set; }

        public MapMaker(TOC assets, Action<int> progressCallback, Action<bool> completeCallback)
        {
            this.assets = assets;
            this.progressCallback = progressCallback;
            this.completeCallback = completeCallback;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += make;

            worker.ProgressChanged += (sender, e) =>
            {
                if (!worker.CancellationPending)
                    progressCallback(e.ProgressPercentage);
            };

            worker.RunWorkerCompleted += ( sender,  e) =>
            {
                if (worker.CancellationPending) TilesBitmap.Dispose();
                else completeCallback(string.IsNullOrEmpty(Error));
            };
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

            // entities
            if (levelNumber < 6)
            {
                string eibName = $"WORLD{mapName.Substring(1, 3)}.EIB";
                if (!assets.Entries.ContainsKey(eibName))
                {
                    Error = $"entities {eibName} not found";
                    return false;
                }
                entitiesEntry = assets.Entries[eibName];
            }
            else entitiesEntry = null;

            if (levelNumber == 6) levelNumber = 5; // level 6 is using palette of level 5

            // collisions
            string collisionsName = $"WORLD{levelNumber}.COL";
            if (!assets.Entries.ContainsKey(collisionsName))
            {
                Error = $"collisions {collisionsName} not found";
                return false;
            }
            collisionsEntry = assets.Entries[collisionsName];

            // palette 
            string palName = $"WORLD{levelNumber}.PAL";
            if (!assets.Entries.ContainsKey(palName))
            {
                Error = $"palette {palName} not found";
                return false;
            }
            paletteEntry = assets.Entries[palName];
            
            worker.RunWorkerAsync(); // calls make()
            return true;
        }

        private void make(object sender, DoWorkEventArgs e)
        {
            try
            {
                worker.ReportProgress(0);

                Map = new PCMFile(mapEntry.Data);

                COLFile colFile = new COLFile(collisionsEntry.Data);
                EIBFile entitiesFile = entitiesEntry != null ? new EIBFile(entitiesEntry.Data) : null;

                worker.ReportProgress(10);
                if (worker.CancellationPending) return;

                // get tileset bitmaps
                Bitmap[] tiles = PICConverter.PICToBitmaps(tilesetEntry.Data, paletteEntry.Data);

                worker.ReportProgress(40);
                if (worker.CancellationPending) return;

                TilesBitmap = new Bitmap(Game.TileSize * Map.Width, Game.TileSize * Map.Height);
                CollisionsBitmap = new Bitmap(Game.TileSize * Map.Width, Game.TileSize * Map.Height);
                EntitiesBitmap = new Bitmap(Game.TileSize * Map.Width, Game.TileSize * Map.Height);
                GridBitmap = new Bitmap(Game.TileSize * Map.Width, Game.TileSize * Map.Height);

                // draw map tiles and collisions
                Graphics tilesGfx = Graphics.FromImage(TilesBitmap);
                Graphics collGfx = Graphics.FromImage(CollisionsBitmap);                

                int total = Map.Width * Map.Height;

                for (int y = 0; y < Map.Height; y++)
                {
                    for (int x = 0; x < Map.Width; x++)
                    {
                        int id = x + y * Map.Width;
                        int tileId = Map.TilesIndices[y, x];

                        // draw cell
                        Bitmap tile = tiles[tileId];
                        tilesGfx.DrawImage(tile, x * Game.TileSize, y * Game.TileSize, Game.TileSize, Game.TileSize);
                        CollisionDrawer.Draw(collGfx, colFile, tileId, x, y);

                        worker.ReportProgress(40 + (int)Math.Round(50f * id / total));

                        if (worker.CancellationPending)
                        {
                            Error = "Cancelled";
                            goto end;
                        }
                    }
                }

                // draw entity info
                if (entitiesFile != null) EntityDrawer.Draw(EntitiesBitmap, entitiesFile);

                // draw grid
                Graphics gridGfx = Graphics.FromImage(GridBitmap);
                Pen pen = new Pen(Color.FromArgb(100, 200, 200, 0), 1);
                for (int x = 0; x < Map.Width; x++) gridGfx.DrawLine(pen, x * Game.TileSize, 0, x * Game.TileSize, Map.Height * Game.TileSize);
                for (int y = 0; y < Map.Height; y++) gridGfx.DrawLine(pen, 0, y * Game.TileSize, Map.Width * Game.TileSize, y * Game.TileSize);
                gridGfx.Dispose();

                end:
                tilesGfx.Dispose();
                collGfx.Dispose();                
            }
            catch(Exception ex)
            {
                Error = ex.Message;
                return;
            }
        }

        public void Cancel()
        {
            if (worker.IsBusy) worker.CancelAsync();
        }

    }
}
