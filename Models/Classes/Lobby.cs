using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using TTTUnturned.Managers;

namespace TTTUnturned.Models
{
    public class Lobby
    {
        public LobbyState State { get;  set; }
        public List<LobbyPlayer> Players { get; set; }

        public Lobby(LobbyState state)
        {
            State = state;
            Players = new List<LobbyPlayer>();
        }

        public Lobby(LobbyState state, List<LobbyPlayer> players)
        {
            State = state;
            Players = players;
        }

        public void Start()
        {
            Players = RoleManager.ShufflePlayerRoles(this);
            // Spawn items
            // Teleport all players to spawn point
            // Assign all players a role
        }
    }
}
