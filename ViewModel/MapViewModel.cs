using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;
using GridSim.Model;

namespace GridSim.ViewModel
{
        public class MapViewModel : BaseViewModel
    {
        private bool isCreatingScenario = true;
        public bool IsCreatingScenario
        {
            get { return isCreatingScenario; }
            set
            {
                isCreatingScenario = value;
                OnPropertyChanged();
            }
        }

        public TileViewModel[,] tiles;
        public ObservableCollection<ViewModel.TileViewModel> Map { get; set; }
        public ObservableCollection<ViewModel.ElementViewModel> Elements { get; set; }
        private Dictionary<TileViewModel, ElementViewModel> ElementOnTile { get; set; }

        public void loadMapFromFile(string path)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                string jsonMap = File.ReadAllText(path);
            var mapData = JsonSerializer.Deserialize<EntireMapData>(jsonMap);
            TileModel[,] updatedtiles = new TileModel[mapData.Rows, mapData.Columns];
            for (int r = 0; r < mapData.Rows; ++r)
            {
                for (int c = 0; c < mapData.Columns; ++c)
                {
                    TileData tileData = mapData.Tiles.FirstOrDefault(t => t.Row == r && t.Col == c);
                    if (tileData != null)
                    {
                        TileModel temp = new TileModel(TileModel.stringToEnum(tileData.Type));
                        TileTypes tileType = (TileTypes)Enum.Parse(typeof(TileTypes), tileData.Type);
                        updatedtiles[r, c] = temp;
                    }
                    else
                    {
                        updatedtiles[r, c] = new TileModel(TileTypes.Path);
                    }
                }
            }
            updateMap(updatedtiles);
            });
        }
        public void updateMap(TileModel[,] updatedTiles)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (updatedTiles.GetLength(0) != this.Rows || updatedTiles.GetLength(1) != this.Columns)
            {
                Debug.WriteLine("Map size mismatch");
                return;
            }
            else
            {
            for (int r = 0; r < this.Rows; ++r) 
            {
                for (int c = 0; c < this.Columns; ++c)
                {
                    
                    if (updatedTiles[r, c].Type != tiles[r, c].Type)
                    {
                            FromSververChangeTileType(tiles[r, c], updatedTiles[r, c].Type);
                    }
                }
            }

            }
            });
        }

        public void resetMap()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                foreach (TileViewModel tile in Map)
                {
                    if (tile.Type != TileTypes.Path)
                    {
                        FromSververChangeTileType(tile, TileTypes.Path);
                    }
                }
            });
        }

        private int rows;
        public int Rows
        {
            get { return rows; }
            set 
            { 
                rows = value; 
                OnPropertyChanged();
            }
        }

        private int columns;
        public int Columns
        {
            get { return columns; }
            set 
            {
                columns = value;
                OnPropertyChanged();
            }
        }

        private int canvasWidth;
        public int CanvasWidth
        {
            get { return canvasWidth; }
            set
            {
                canvasWidth = value;
                OnPropertyChanged();
            }
        }

        private int canvasHeight;
        public int CanvasHeight
        {
            get { return canvasHeight; }
            set
            {
                canvasHeight = value;
                OnPropertyChanged();
            }
        }
        private TileTypes HeldtileType;
        private string tileTypeToAdd = nameof(TileTypes.Path);
        public string TileTypeToAdd
        {
            get { return tileTypeToAdd; }
            set 
            {
                tileTypeToAdd = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand CycleTileTypeCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }

        private MainViewModel mainViewModel;

        public MapViewModel(MainViewModel mainViewModel, int rows, int columns, int height, int width)
        {
            this.mainViewModel = mainViewModel;
            Map = new ObservableCollection<ViewModel.TileViewModel>();
            Elements = new ObservableCollection<ViewModel.ElementViewModel>();
            ElementOnTile = new Dictionary<TileViewModel, ElementViewModel>();
            SetupAdjust(width, columns, height, rows);
            CreateAndPlaceTiles(CanvasHeight / this.rows, CanvasWidth / this.columns);
            CycleTileTypeCommand = new RelayCommand(param => cycleTileType(), null);
            SaveCommand = new RelayCommand(param => SaveMap("C:\\Users\\OmarM\\Documents\\omarKan\\DA298A_Root\\GridSim\\MapGenerator\\map.json"), null/*canExecute => IsCreatingScenario*/);
            LoadCommand = new RelayCommand(param => loadMapFromFile("C:\\Users\\OmarM\\Documents\\omarKan\\DA298A_Root\\GridSim\\MapGenerator\\ValidMaps\\map.json"), null/*canExecute => IsCreatingScenario*/);


            //addElementOnTile(tiles[6, 7], ElementTypes.FireFighter, 50, 50);
        }

        private void CreateAndPlaceTiles(int tileHeight, int tileWidth)
        {
            this.tiles = new TileViewModel[this.rows, this.columns];
            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Columns; c++)
                {
                    int canvasX = c * tileWidth;
                    int canvasY = r * tileHeight;
                    tiles[r, c] = new TileViewModel(this, tileWidth, tileHeight, canvasX, canvasY, r, c);
                    Map.Add(tiles[r, c]);
                }
            }
        }

        public TileViewModel getTile(int row, int col)
        {
            return tiles[row, col];
        }

        private void SetupAdjust(int width, int columns, int height, int rows)
        {
            this.Rows = rows;
            this.Columns = columns;
            CanvasWidth = width - (width % columns); //justera aspect ratio, (tror det her aspect ratio iaf)
            CanvasHeight = height - (height % rows);
        }

        public void debugPrint()
        {
            Debug.WriteLine("Printing map");
            Debug.WriteLine($"Rows: {Rows} Columns: {Columns}");
            List<TileViewModel> tilesOnFire = new List<TileViewModel>();
            List<TileViewModel> tilesOnSmoke = new List<TileViewModel>();
            List<TileViewModel> tilesWithVictim = new List<TileViewModel>();
            List<TileViewModel> tilesWithHazard = new List<TileViewModel>();
            List<TileViewModel> tilesWithWall = new List<TileViewModel>();

            foreach (TileViewModel tile in Map)
            {
                switch (tile.Type) 
                {
                    case TileTypes.Fire:
                        tilesOnFire.Add(tile);
                        break;
                    case TileTypes.Smokey:
                        tilesOnSmoke.Add(tile);
                        break;
                    case TileTypes.HasVictim:
                        tilesWithVictim.Add(tile);
                        break;
                    case TileTypes.HasHazard:
                        tilesWithHazard.Add(tile);
                        break;
                    case TileTypes.Wall:
                        tilesWithWall.Add(tile);
                        break;
                }
            }
            Debug.WriteLine($"Tiles on fire: {tilesOnFire.Count}");
            foreach (TileViewModel tile in tilesOnFire)
            {
                Debug.WriteLine($"R:{tile.Col} K:{tile.Row}");
            }
            Debug.WriteLine($"Tiles on smoke: {tilesOnSmoke.Count}");
            foreach (TileViewModel tile in tilesOnSmoke)
            {
                Debug.WriteLine($"R:{tile.Col} K:{tile.Row}");
            }
            Debug.WriteLine($"Tiles with victim: {tilesWithVictim.Count}");
            foreach (TileViewModel tile in tilesWithVictim)
            {
                Debug.WriteLine($"R:{tile.Col} K:{tile.Row}");
            }
            Debug.WriteLine($"Tiles with hazard: {tilesWithHazard.Count}");
            foreach (TileViewModel tile in tilesWithHazard)
            {
                Debug.WriteLine($"R:{tile.Col} K:{tile.Row}");
            }
            Debug.WriteLine($"Tiles with wall: {tilesWithWall.Count}");
            foreach (TileViewModel tile in tilesWithWall)
            {
                Debug.WriteLine($"R:{tile.Col} K:{tile.Row}");
            }

            
        }

        public void SaveMap(String saveLocation)
        {
            var mapData = new EntireMapData
            {
                Rows = Rows,
                Columns = Columns,
                Tiles = Map.Where(tile => tile.Type != TileTypes.Path)
                .Select(tile => new TileData
                {
                    Row = tile.Row,
                    Col = tile.Col,
                    Type = tile.Type.ToString()
                })
                .ToList()
            };

            string json = JsonSerializer.Serialize(mapData, new JsonSerializerOptions
            {
                //WriteIndented = true // Pretty-print the JSON
            });

            Debug.WriteLine(json);
            File.WriteAllText(saveLocation, json);
        }

/*        public String jsonMap ()
        {
            var mapData = new MapSizeData
            {
                Rows = Rows,
                Columns = Columns
            };
            string json = JsonSerializer.Serialize(mapData, new JsonSerializerOptions
            {
                WriteIndented = false 
            });
            return json;
        }



        public String jsonTile(int row, int col)
        {
            var tileData = new TileData
            {
                Row = Rows,
                Col = Columns,
                Type = tiles[row, col].Type.ToString()
            };
            string json = JsonSerializer.Serialize(tileData, new JsonSerializerOptions
            {
                WriteIndented = false
            });
            return json;
        }

        */
        private void cycleTileType()
        {
            switch (TileTypeToAdd)
            {
                case nameof(TileTypes.Path):
                    TileTypeToAdd = nameof(TileTypes.Wall);
                    HeldtileType = TileTypes.Wall;
                    break;
                case nameof(TileTypes.Wall):
                    TileTypeToAdd = nameof(TileTypes.Smokey);
                    HeldtileType = TileTypes.Smokey;
                    break;
                case nameof(TileTypes.Smokey):
                    TileTypeToAdd = nameof(TileTypes.Fire);
                    HeldtileType = TileTypes.Fire;
                    break;
                case nameof(TileTypes.Fire):
                    TileTypeToAdd = nameof(TileTypes.HasVictim);
                    HeldtileType = TileTypes.HasVictim;
                    break;
                case nameof(TileTypes.HasVictim):
                    TileTypeToAdd = nameof(TileTypes.HasHazard);
                    HeldtileType = TileTypes.HasHazard;
                    break;
                case nameof(TileTypes.HasHazard):
                    TileTypeToAdd = nameof(TileTypes.Path);
                    HeldtileType = TileTypes.Path;
                    break;
            }
        }

        public void ToServerChangeTileType(TileViewModel tileViewModel)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
            if (tiles[tileViewModel.Row,tileViewModel.Col].Type != TileTypes.FireFighter)
            {
                clearElementsTile(tileViewModel);
            switch (HeldtileType) 
            {
                case TileTypes.Path:
                    tileViewModel.Type = TileTypes.Path;
                    break;
                case TileTypes.Wall:
                tileViewModel.Type = TileTypes.Wall;
                    break;
                case TileTypes.Smokey:
                    tileViewModel.Type = TileTypes.Smokey;
                    addElementOnTile(tileViewModel, ElementTypes.Smoke, 50,50);
                    break;
                case TileTypes.Fire:
                    tileViewModel.Type = TileTypes.Fire;
                    addElementOnTile(tileViewModel, ElementTypes.Fire, 50, 50);
                    break;
                case TileTypes.HasVictim:
                    tileViewModel.Type = TileTypes.HasVictim;
                    addElementOnTile(tileViewModel, ElementTypes.Victim, 50, 50);
                    break;
                case TileTypes.HasHazard:
                    tileViewModel.Type = TileTypes.HasHazard;
                    addElementOnTile(tileViewModel, ElementTypes.Hazard, 50, 50);
                    break;
            }
                    var tileMessage = new TileUpdate
                    {
                        Command = "Tile",
                        Row = tileViewModel.Row,
                        Column = tileViewModel.Col,
                        Type = tileViewModel.Type.ToString()
                    };
                    String tileJson = JsonSerializer.Serialize(tileMessage, new JsonSerializerOptions
                    {
                        WriteIndented = false
                    });
                    mainViewModel.serialViewModel.OutputString(tileJson);
                    //mainViewModel.await Task.Delay(50);
                }
            });
        }

        public void FromSververChangeTileType(TileViewModel tileViewModel, TileTypes tileType)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                clearElementsTile(tileViewModel);
            switch (tileType)
            {
                case TileTypes.Path:
                    tileViewModel.Type = TileTypes.Path;
                    break;
                case TileTypes.Wall:
                    tileViewModel.Type = TileTypes.Wall;
                    break;
                case TileTypes.Smokey:
                    tileViewModel.Type = TileTypes.Smokey;
                    addElementOnTile(tileViewModel, ElementTypes.Smoke, 50, 50);
                    break;
                case TileTypes.Fire:
                    tileViewModel.Type = TileTypes.Fire;
                    addElementOnTile(tileViewModel, ElementTypes.Fire, 50, 50);
                    break;
                case TileTypes.HasVictim:
                    tileViewModel.Type = TileTypes.HasVictim;
                    addElementOnTile(tileViewModel, ElementTypes.Victim, 50, 50);
                    break;
                case TileTypes.HasHazard:
                    tileViewModel.Type = TileTypes.HasHazard;
                    addElementOnTile(tileViewModel, ElementTypes.Hazard, 50, 50);
                    break;
                case TileTypes.FireFighter:
                    tileViewModel.Type = TileTypes.FireFighter;
                    addElementOnTile(tileViewModel, ElementTypes.FireFighter, 50, 50);
                    break;
                }
            });
        }

        public void addElementOnTile(TileViewModel tileViewModel, ElementTypes elementType, int height, int width)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ElementViewModel elementViewModel = new ElementViewModel(elementType, tileViewModel.X, tileViewModel.Y);
            ElementOnTile.Add(tileViewModel, elementViewModel);
            Elements.Add(elementViewModel);
            });
        }
        public void clearElementsTile(TileViewModel tileViewModel)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (ElementOnTile.ContainsKey(tileViewModel))
            {
                ElementViewModel elementViewModel = ElementOnTile[tileViewModel];
                ElementOnTile.Remove(tileViewModel);
                Elements.Remove(elementViewModel);
            }
            });
        }

        public void moveElementTo(TileViewModel from, TileViewModel to)
        {
            if (ElementOnTile.ContainsKey(from))
            {
                // Retrieve the element to move
                ElementViewModel element = ElementOnTile[from];

                // Update the element's position
                element.changePosition(to.X, to.Y);

                // Reassign the element to the new tile in the dictionary
                ElementOnTile[to] = element;
                ElementOnTile.Remove(from);

                // Update the tile types if needed (optional, based on your requirements)
                to.Type = from.Type; // Example: update the `to` tile type
                from.Type = TileTypes.Path; //
            }
        }
    }
    public class EntireMapData
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<TileData> Tiles { get; set; }
    }


        public class MapSizeData
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
    }

    public class TileData
    {
        public string Command = "Tile";
        public string Type { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
