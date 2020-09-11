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
            if (State != LobbyState.SETUP) return;

            CommandWindow.Log(Provider.clients.Count);
            CommandWindow.Log(Main.Config.MinimumPlayers);

            if (Provider.clients.ToList().Count >= Main.Config.MinimumPlayers)
            {
                LobbyManager.Message("Round starting in <color=red>15</color> seconds");
                await Task.Delay(15000);

                Players = RoleManager.GeneratePlayerRoles(); // Assign all players a role
                Managers.ItemManager.RespawnItems(); // Spawn items

                // Teleport all players to spawn point
                System.Random rng = new System.Random();
                List<Spawn> spawns = Main.Config.Maps[rng.Next(Main.Config.Maps.Count)].Spawns;
                Players.ForEach(async player =>
                {
                    SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(player.SteamID);
                    await Managers.PlayerManager.TeleportToLocationAsync(steamPlayer, Managers.PlayerManager.GetRandomSpawn(spawns));
                });

                // Wait 30 seconds before displaying roles and allowing damage
                State = LobbyState.WAITING;
                LobbyManager.Message("Roles will be assigned in <color=red>30</color> seconds!");
                await Task.Delay(30000);

                RoleManager.TellRoles(this);
                State = LobbyState.LIVE;
            }
            else
            {
                LobbyManager.Message($"<color=red>{Main.Config.MinimumPlayers - Provider.clients.Count}</color> more players needed to start game.");
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
                    Managers.PlayerManager.ClearInventoryAsync(ply);
                    Managers.PlayerManager.TeleportToLocationAsync(ply, Managers.PlayerManager.GetRandomSpawn(Main.Config.LobbySpawns));
                }
            });

            await Start();
            //AsyncHelper.RunAsync("RestartLobby", Start);
            // Track stats in database
        }

        public List<LobbyPlayer> GetAlive(PlayerRole role)
        {
            return Players.FindAll(player => player.Role == role && player.Status == PlayerStatus.ALIVE);
        }
    }
}
