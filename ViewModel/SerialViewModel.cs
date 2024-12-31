

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using GridSim.Model;

namespace GridSim.ViewModel
{
    public class SerialViewModel : INotifyPropertyChanged
    {
        private SerialModel serialModel;

        private string portName = string.Empty;
        public string PortName
        {
            get { return portName; }
            set
            {
                portName = value;
                OnPropertyChanged();
            }
        }
        private string textToSend = string.Empty;
        public string TextToSend
        {
            get { return textToSend; }
            set
            {
                textToSend = value;
                OnPropertyChanged();
            }
        }



        public ObservableCollection<string> Messages { get; set; }
        //public ObservableCollection<MapTileModel> Map { get; set; }
        public RelayCommand ConnectCommand { get; }
        public RelayCommand SendCommand { get; }
        public RelayCommand DisconnectCommand { get; }
        private MainViewModel mainViewModel;

        public SerialViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            serialModel = new SerialModel();
            serialModel.DataReceived += OnDataReceived;

            Messages = new ObservableCollection<string>();

            ConnectCommand = new RelayCommand(execute => ConnectSerial()
            , CanExecuteConnect => { return !string.IsNullOrEmpty(portName); });

            SendCommand = new RelayCommand(Send => OutputSerial()
            , canExecute => { return serialModel.IsConnected; });

            DisconnectCommand = new RelayCommand(Disconnect => { serialModel.Disconnect(); informUser("Disconnected"); }
            , canExecute => { return serialModel.IsConnected; });

            PortName = "COM12";
        }

        private void OutputSerial()
        {
                serialModel.Send(TextToSend);
                informUser($"Skickat: {TextToSend}");
                TextToSend = string.Empty;
        }

        public void informUser(string stringToDisplay)
        {
            Messages.Add(stringToDisplay);
        }

        public void OutputString(string text)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                serialModel.Send(text);
            });
        }
        public void ConnectSerial()
        {
                if (serialModel.Connect(portName, 115200))
                {
                    informUser($"Ansluten till {PortName}");
                    var resetMessage = new MessagesToSend
                    {
                        Command = "Reset"
                    };
                    string resetJson = JsonSerializer.Serialize(resetMessage, new JsonSerializerOptions
                    {
                        WriteIndented = false
                    });
                    OutputString(resetJson);

                }
                else
                {
                    informUser($"Kunde inte ansluta till, Försök igen {PortName}");
                }
            
        }
        private void OnDataReceived(string data)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                
                try
                {
                    var message = JsonSerializer.Deserialize<MessagesToSend>(data);
                    switch (message.Command)
                    {
                        case "Tile":
                            TileUpdate tile = JsonSerializer.Deserialize<TileUpdate>(data);
                            mainViewModel.changeTile(tile.Row, tile.Column, tile.Type);
                            break;
                        case "Reset":
                            mainViewModel.resetFromServer();
                            break;
                        case "MoveTile":
                            MoveTile moveTile = JsonSerializer.Deserialize<MoveTile>(data);
                            mainViewModel.moveTile(moveTile.OldRow, moveTile.OldColumn, moveTile.Row, moveTile.Column);
                            break;
                    }
                }
                catch (Exception e)
                {
                    informUser("RECEIVED: " + data);
                }

                //informUser("GUI received: " + data);



                //mainViewModel.EvaluateReceived(data);
            });
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal bool isConnected()
        {
            return serialModel.IsConnected;
        }

        /*public MessagesToReceiveTypes StringToEnum(string command)
        {
            switch(command)
            {
                case "TileUpdateCommand":
                    return MessagesToReceiveTypes.TileUpdateCommand;
                case "NewMap":
                    return MessagesToReceiveTypes.NewMap;
                default:
                    throw new Exception("not a command");
            }

        }*/

    }
    



    }
