using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using GridSim.Model;

namespace GridSim.ViewModel
{
    public class TileViewModel : ViewModel.BaseViewModel
    {
        private Model.TileModel tileModel;
        private TileTypes type => tileModel.Type;
        public TileTypes Type
        {
            get { return type; }
            set
            {
                tileModel.Type = value;
                getTileAppearance();
                OnPropertyChanged();
            }
        }

        private Brush tileColor;
        public Brush TileColor
        {
            get { return tileColor; }
            set
            {
                tileColor = value;
                OnPropertyChanged();
            }
        }


        private int tileWidth;
        public int TileWidth
        {
            get { return tileWidth; }
            set
            {
                tileWidth = value;
                OnPropertyChanged();
            }
        }

        private int tileHeight;
        public int TileHeight
        {
            get { return tileHeight; }
            set
            {
                tileHeight = value;
                OnPropertyChanged();
            }
        }

        private int y;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        private int x;

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public RelayCommand ClickCommand { get; set; }
        
        private string stringColRow;
        public string StringColRow { get { return stringColRow; } set 
            {
                stringColRow = value;
                OnPropertyChanged();
            } }
        private int row;
        public int Row { get { return row; } set 
            {
                row = value;
                OnPropertyChanged();
            } }
        private int col;
        public int Col
        {
            get { return col; }
            set
            {
                col = value;
                OnPropertyChanged();
            }
        }
        private MapViewModel myMap;

        private string tileImage;

        public string TileImage
        {
            get { return tileImage; }
            set 
            { 
                tileImage = value;
                OnPropertyChanged();
            }
        }

        public TileViewModel(MapViewModel mapViewModel, int tileWidth, int tileHeight, int canvasX, int canvasY, int row, int col)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                this.myMap = mapViewModel;
                this.row = row;
                this.col = col;
                this.x = canvasX;
                this.y = canvasY;
                this.tileWidth = tileWidth;
                this.tileHeight = tileHeight;
                this.tileModel = new TileModel(TileTypes.Path);
                getTileAppearance();
                StringColRow = $"{col},{row}";
                ClickCommand = new RelayCommand(async ClickCommand
                    =>
                {
                    /* if (Type != TileTypes.Path)
                     {
                         Debug.WriteLine("Changing to path\n");
                         myMap.removeElementOnTile(this, ElementTypes.Fire);
                         Type = TileTypes.Path;
                         OnPropertyChanged(nameof(Color));
                     }
                     else
                     {
                         Debug.WriteLine("Changing to wall\n");
                         myMap.addElementOnTile(this, ElementTypes.Fire);
                         Type = TileTypes.Wall;
                         OnPropertyChanged(nameof(Color));
                     }
                 */
                    Debug.WriteLine($"Tile at \nXpos: {X}, Column: {col}\n Ypos: {Y}, row: {row}\n"); // Check coordinates
                    myMap.ToServerChangeTileType(this);
                    
                });
            });
        }

        private void getTileAppearance()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                switch (Type)
                {
                    case TileTypes.Path:
                        TileColor = Brushes.Transparent;
                        TileImage = String.Empty;
                        break;
                    case TileTypes.Wall:
                        TileColor = Brushes.Black;
                        TileImage = "Resources/wall.png";
                        break;
                    case TileTypes.Fire:
                        TileColor = Brushes.Red;
                        TileImage = String.Empty;
                        break;
                    case TileTypes.HasVictim:
                        TileColor = Brushes.Purple;
                        TileImage = String.Empty;
                        break;
                    case TileTypes.HasHazard:
                        TileColor = Brushes.ForestGreen;
                        TileImage = String.Empty;
                        break;
                    case TileTypes.Smokey:
                        TileColor = Brushes.Gray;
                        TileImage = String.Empty;
                        break;
                    case TileTypes.FireFighter:
                        TileColor = Brushes.Blue;
                        TileImage = String.Empty;
                        break;
                    default:
                        return;//throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

            });

        }
    }
}
