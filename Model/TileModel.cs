using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GridSim.Model
{
    public class TileModel
    {
        public TileTypes Type;
        

        public TileModel(TileTypes type)
        {
            this.Type = type;
        }

        public static TileTypes stringToEnum(string type)
        {
            switch (type)
            {
                case "Path":
                    return TileTypes.Path;
                case "Wall":
                    return TileTypes.Wall;
                case "Smokey":
                    return TileTypes.Smokey;
                case "Fire":
                    return TileTypes.Fire;
                case "HasVictim":
                    return TileTypes.HasVictim;
                case "HasHazard":
                    return TileTypes.HasHazard;
                case "FireFighter":
                    return TileTypes.FireFighter;
                default:
                    throw new ArgumentOutOfRangeException();
                        }
        }

    }
        public enum TileTypes
        {
            Path,
            Wall,
            Smokey,
            Fire,
            HasVictim,
            HasHazard,
            FireFighter
        }

    }
