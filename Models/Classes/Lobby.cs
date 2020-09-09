using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using Steamworks;
using TTTUnturned.Managers;
using TTTUnturned.Utils;
using UnityEngine;

namespace TTTUnturned.Models
{
    public class Lobby
    {
        public LobbyState State { get;  set; }
        public int RoundTime { get; set; }
        public List<LobbyPlayer> Players { get; set; }

        public Lobby(LobbyState state)
        {
            State = state;
            RoundTime = Main.Config.RoundLength;
            Players = new List<LobbyPlayer>();
        }

        public Lobby(LobbyState state, List<LobbyPlayer> players)
        {
            State = state;
            RoundTime = Main.Config.RoundLength;
            Players = players;
        }

        public void Start()
        {
            // Assign all players a role
            Players = RoleManager.GeneratePlayerRoles();
            // Spawn items
            ItemsManager.respawnItems();
            // Teleport all players to spawn point
            System.Random rng = new System.Random();
            List<Spawn> spawns = Main.Config.Maps[rng.Next(Main.Config.Maps.Count)].Spawns;
            Players.ForEach(player =>
            {
                int t = rng.Next(spawns.Count);
                Vector3 spawn = new Vector3(spawns[t].X, spawns[t].Y, spawns[t].Z);
                SteamPlayer steamPlayer = Provider.clients.Find(x => x.playerID.steamID == player.SteamID);
                steamPlayer.player.teleportToLocation(spawn, 0f);
            });
            // Wait 30 seconds before displaying roles and allowing damage
            // Display roles
            RoleManager.tellRoles(this);
        }
    }
}
