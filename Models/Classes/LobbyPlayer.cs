using TTTUnturned.Models;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using TTTUnturned.Utils;

namespace TTTUnturned.Models
{
    public class LobbyPlayer
    {
        public CSteamID SteamID { get; set; }
        public PlayerRole Role { get; set; }
        public PlayerRank Rank { get; set; }
        public PlayerStatus Status { get; set; }
        public bool UIToggled { get; set; }
        public int Credits { get; set; }

        public LobbyPlayer(CSteamID steamID, PlayerRole role, PlayerRank rank, PlayerStatus status)
        {
            SteamID = steamID;
            Role = role;
            Rank = rank;
            Status = status;
            Credits = 0;
            UIToggled = false;
        }
    }
}
