﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Navigation;
using GridSim.Model;


namespace GridSim.ViewModel
{
    public class MainViewModel
    {
        private string lastLoadedMapSizeData = string.Empty;
        public ViewModel.MapViewModel mapViewModel { get; set; }
        public SerialViewModel serialViewModel { get; set; }
        public RelayCommand SendScenario { get; set; }

        public RelayCommand GeneralDebugbutton { get; set; }
        public MainViewModel()
        {
            mapViewModel = new MapViewModel(this, 20, 25, 600, 800);
            serialViewModel = new SerialViewModel(this);
            SendScenario = new RelayCommand(execute => sendNewMap(), canExecute => serialViewModel.isConnected());
            GeneralDebugbutton = new RelayCommand(execute => mydebugfunc(), canExecute => { return true; });
        }

        private void sendRresetToServer()
        {
            var resetMessage = new MessagesToSend
            {
                Command = "Reset"
            };
            string resetJson = JsonSerializer.Serialize(resetMessage, new JsonSerializerOptions
            {
                WriteIndented = false
            });
            serialViewModel.OutputString(resetJson);


        }

        private void sendNewMapDimensionsToServer()
        {
            var newMap = new NewMap
            {
                Command = "NewMap",
                Rows = mapViewModel.Rows,
                Columns = mapViewModel.Columns,
            };
            string newMapJson = JsonSerializer.Serialize(newMap, new JsonSerializerOptions
            {
                WriteIndented = false
            });
            serialViewModel.OutputString(newMapJson);

        }

        private void sendGoToServer()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var GoMessage = new MessagesToSend
                {
                    Command = "Go"
                };
                string goJson = JsonSerializer.Serialize(GoMessage, new JsonSerializerOptions
                {
                    WriteIndented = false
                });
                serialViewModel.OutputString(goJson);
            });
        }

        public void resetFromServer()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                mapViewModel.resetMap();
            });
        }

        public void mydebugfunc()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var moveTileMessage = new MoveTile
                {
                    Command = "MoveTile",
                    OldRow = 0,
                    OldColumn = 0,
                    Row = 1,
                    Column = 1,
                };
                String moveTileJson = JsonSerializer.Serialize(moveTileMessage, new JsonSerializerOptions
                {
                    WriteIndented = false
                });
                serialViewModel.OutputString(moveTileJson);
            });
            
        }

        private void sendNewMap()
        {
            App.Current.Dispatcher.Invoke(async () =>
            {
                //sendRresetToServer();
                //await Task.Delay(50);
                sendNewMapDimensionsToServer();
                await Task.Delay(50);

                foreach (TileViewModel tile in mapViewModel.tiles)
                {
                    if (tile.Type != TileTypes.Path)
                    {
                        var tileMessage = new TileUpdate
                        {
                            Command = "Tile",
                            Row = tile.Row,
                            Column = tile.Col,
                            Type = tile.Type.ToString()
                        };
                        String tileJson = JsonSerializer.Serialize(tileMessage, new JsonSerializerOptions
                        {
                            WriteIndented = false
                        });
                        serialViewModel.OutputString(tileJson);
                        await Task.Delay(50);
                    }
                }
                sendGoToServer();
                await Task.Delay(50);
            });
        }
        public void changeTile(int row, int col, String tileType)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                //för att uppdatera lokalt
                TileViewModel tileToChange = mapViewModel.getTile(row, col);
                mapViewModel.FromSververChangeTileType(tileToChange, TileModel.stringToEnum(tileType));

            });
        }

        public void moveTile(int row, int col, int newRow, int newCol)
        {
            App.Current.Dispatcher.Invoke(() =>
         {
             TileViewModel tileToMove = mapViewModel.getTile(row, col);
             TileViewModel newTile = mapViewModel.getTile(newRow, newCol);
             mapViewModel.moveElementTo(tileToMove, newTile);
         });

        }
    }

}
