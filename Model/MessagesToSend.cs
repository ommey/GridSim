﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridSim.Model
{
    public enum MessagesToReceiveTypes // medelanden från server
    {
        Tile,
    }
    public class MessagesToSend //meddelanden till server
    {
        public string Command { get; set; }
    }

    public class NewMap : MessagesToSend 
    {
        public NewMap()
        {
            Command = "NewMap";
        }
        public int Rows { get; set; }
        public int Columns { get; set; }
    };

    public class MoveTile : MessagesToSend 
    {
        public MoveTile()
        {
            Command = "MoveTile";
        }
        public int OldRow { get; set; }
        public int OldColumn { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Type { get; set; }
    };

    public class TileUpdate : MessagesToSend 
    {
        public TileUpdate()
        {
            Command = "Tile";
        }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Type { get; set; }
    };

}
