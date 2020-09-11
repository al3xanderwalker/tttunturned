using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SDG.Unturned;
using TTTUnturned.Managers;
using TTTUnturned.Utils;
using UnityEngine;
using System.Collections;

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

        public async Task Start()
        {
            // CHECK IF WE CAN START THE GAME
            int playersRequired;
            if (Main.Config.DebugMode) playersRequired = 2;
            else playersRequired = 5;

            if (State != LobbyState.SETUP)
            {
                return;
            }

            if (Provider.clients.ToList().Count >= playersRequired)
            {

                LobbyManager.Message("Round starting in <color=red>15</color> seconds");
                await Task.Delay(15000);

                Players = RoleManager.GeneratePlayerRoles(); // Assign all players a role
                ItemsManager.RespawnItems(); // Spawn items

                // Teleport all players to spawn point
                System.Random rng = new System.Random();
                List<Spawn> spawns = Main.Config.Maps[rng.Next(Main.Config.Maps.Count)].Spawns;
                Players.ForEach(player =>
                {
                    SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(player.SteamID);
                    PlayersManager.TeleportToLocation(steamPlayer, RandomSpawn(spawns));
                });

                // Wait 30 seconds before displaying roles and allowing damage
                State = LobbyState.WAITING;
                LobbyManager.Message("Roles will be assigned in <color=red>30</color> seconds!");
                await Task.Delay(30000);

                RoleManager.TellRoles(this);
                State = LobbyState.LIVE;
                return;
            }
            else
            {
                LobbyManager.Message($"<color=red>{playersRequired - Provider.clients.Count}</color> more players needed to start game.");
                return;
            }
        }

        public async Task Stop()
        {
            State = LobbyState.SETUP; // Set game state to setup
            RoundTime = Main.Config.RoundLength; // Reset round timer

            await Task.Delay(10000);
            //Teleport players NOT kill them 
            Players.ForEach(player => 
            {
                if (player.Status == PlayerStatus.ALIVE)
                {
                    SteamPlayer ply = PlayerTool.getSteamPlayer(player.SteamID);
                    if (ply is null) return;
                    PlayersManager.ClearInventory(ply);
                    PlayersManager.TeleportToLocation(ply, RandomSpawn(Main.Config.LobbySpawns));
                }
            });

            await Start();
            //AsyncHelper.RunAsync("RestartLobby", Start);
            // Track stats in database
        }

        public Vector3 RandomSpawn(List<Spawn> spawns)
        {
            System.Random rng = new System.Random();
            int t = rng.Next(spawns.Count);
            return new Vector3(spawns[t].X, spawns[t].Y, spawns[t].Z);
        }

        public List<LobbyPlayer> GetAlive(PlayerRole role)
        {
            return Players.FindAll(player => player.Role == role && player.Status == PlayerStatus.ALIVE);
        }
    }
}
