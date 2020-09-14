using Steamworks;
using TTTUnturned.API.Players;
using TTTUnturned.API.Roles;
using TTTUnturned.API.Round;

namespace TTTUnturned.API.Lobby
{
    public class TTTPlayer
    {
        public CSteamID SteamID { get; set; }
        public Status Status { get; set; }
        public Role Role { get; set; }
        public Rank Rank { get; set; }
        public bool UIToggled { get; set; }
        public int Credits { get; set; }
        public bool Armor { get; set; }

        public TTTPlayer(CSteamID steamID, Role role, Rank rank, Status status)
        {
            SteamID = steamID;
            Role = role;
            Rank = rank;
            Status = status;
            Credits = 0;
            UIToggled = false;
            Armor = false;
        }
    }
}
