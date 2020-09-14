using SDG.Unturned;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTTUnturned.API.Interface;
using TTTUnturned.API.Level;
using TTTUnturned.API.Lobby;
using TTTUnturned.API.Players;
using TTTUnturned.API.Roles;
using PlayerManager = TTTUnturned.API.Players.PlayerManager;

namespace TTTUnturned.API.Round
{
    public class RoundSession
    {
        public RoundState State { get; set; }
        public int RoundTime { get; set; }
        public List<TTTPlayer> Players { get; set; }

        public RoundSession()
        {
            State = RoundState.WAITING;
            RoundTime = Main.Config.RoundLength;
        }

        public async Task Start()
        {
            if (State != RoundState.WAITING) return;

            State = RoundState.WARMUP;

            RoundManager.Broadcast("Round starting in <color=red>15</color> seconds");
            InterfaceManager.SendLobbyBannerMessage(8494, "Round starting in <color=red>15</color> seconds", 5000, true);
            await Task.Delay(15000);

            Players = RoleManager.GeneratePlayerRoles(); // Assign all players a role
            Level.ItemManager.RespawnItems(); // Spawn items

            // Teleport all players to spawn point
            System.Random rng = new System.Random();
            List<Spawn> spawns = Main.Config.Maps[rng.Next(Main.Config.Maps.Count)].Spawns;
            Players.ForEach(async player =>
            {

                InterfaceManager.ClearUIEffectAsync(8501, player.SteamID);
                SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(player.SteamID);
                PlayerManager.ClearInventoryAsync(steamPlayer);
                steamPlayer.player.life.tellHealth(player.SteamID, 100); // DOES NOT WORK
                await PlayerManager.TeleportToLocationAsync(steamPlayer, PlayerManager.GetRandomSpawn(spawns));
            });

            // Wait 30 seconds before displaying roles and allowing damage
            RoundManager.Broadcast("Roles will be assigned in <color=red>15</color> seconds!");
            InterfaceManager.SendLobbyBannerMessage(8494, "<size=25>Roles will be assigned in <color=red>15</color> seconds!</size>", 5000, true);
            await Task.Delay(15000);
            Players.ForEach(p => InterfaceManager.ClearStatusUIAsync(p));
            RoleManager.TellRoles(Players);
            State = RoundState.LIVE;
        }

        public async Task Stop()
        {
            State = RoundState.WAITING; // Set game state to setup
            RoundTime = Main.Config.RoundLength; // Reset round timer

            await Task.Delay(10000);
            //Teleport players NOT kill them 
            Players.ForEach(async player =>
            {
                if (player.Status == Status.ALIVE)
                {
                    player.Role = Roles.Role.NONE;
                    SteamPlayer ply = PlayerTool.getSteamPlayer(player.SteamID);
                    if (ply is null) return;
                    InterfaceManager.ClearUIEffectAsync(8501, player.SteamID);
                    PlayerManager.ClearInventoryAsync(ply);
                    PlayerManager.TeleportToLocationAsync(ply, PlayerManager.GetRandomSpawn(Main.Config.LobbySpawns));
                }
                await InterfaceManager.ClearStatusUIAsync(player);
                await InterfaceManager.SendUIEffectAsync(8498, 8490, player.SteamID, true);
                await InterfaceManager.SendUIEffectTextAsync(8490, player.SteamID, true, "RoleValue", "WAITING");
                await InterfaceManager.SendUIEffectTextAsync(8490, player.SteamID, true, "TimerValue", "00:00");
            });

            await Start();
        }

        public List<TTTPlayer> GetAlive(Roles.Role role)
        {
            return Players.FindAll(player => player.Role == role && player.Status == Status.ALIVE);
        }
    }
}
