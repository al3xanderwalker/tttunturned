using Steamworks;
using TTTUnturned.API.Players;
using TTTUnturned.API.Roles;
using TTTUnturned.API.Round;

namespace TTTUnturned.API.Lobby
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
