using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using Steamworks;
using TTTUnturned.Managers;
using TTTUnturned.Utils;
using TTTUnturned.Models;
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
            LobbyManager.message("Starting game session...");
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
                SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(player.SteamID);
                steamPlayer.player.teleportToLocation(spawn, 0f);
            });
            // Wait 30 seconds before displaying roles and allowing damage
            // Display roles
            LobbyManager.message("Game is live!");
            RoleManager.tellRoles(this);
            State = LobbyState.LIVE;
        }

        public void Stop()
        {
            State = LobbyState.SETUP; // Set game state to setup
            RoundTime = Main.Config.RoundLength; // Reset round timer
            Players.ForEach(player => 
            {
                Player ply = PlayerTool.getPlayer(player.SteamID);
                EPlayerKill kill; // Useless var we have to pass
                ply.life.askDamage(200, Vector3.up, EDeathCause.SUICIDE, ELimb.SKULL, CSteamID.Nil, out kill);
            });
        }

        public List<LobbyPlayer> getAlive(PlayerRole role)
        {
            return Players.FindAll(player => player.Role == role && player.Status == PlayerStatus.ALIVE);
        }
    }
}
