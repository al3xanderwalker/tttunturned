using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Interface;
using TTTUnturned.API.Players;
using TTTUnturned.API.Roles;
using TTTUnturned.API.Items.C4;
using LevelManager = TTTUnturned.API.Level.LevelManager;
using TTTUnturned.API.Items.TrackerGun;

namespace TTTUnturned.API.Round
{
    public class RoundSession
    {
        public RoundState State;
        public int RoundTime;
        public List<TTTPlayer> Players;

        public RoundSession()
        {
            State = RoundState.SETUP;
            RoundTime = 600;
            Players = new List<TTTPlayer>();
        }

        #region API
        public void AddPlayer(TTTPlayer player) => Players.Add(player);

        public void RemovePlayer(TTTPlayer player) => Players.Remove(player);

        public async Task Start()
        {
            try
            {
                CommandWindow.Log("Warmup Starting");
                RoundManager.Broadcast("Warmup Starting");
                State = RoundState.WARMUP;

                LevelManager.RespawnItems();
                Players.ToList().ForEach(p => InterfaceManager.ClearAllUI(p.SteamID));

                await Task.Delay(6000);

                Players.ToList().ForEach(p =>
                {
                    p.SetStatus(PlayerStatus.ALIVE);
                    p.TeleportToMapUnsafe();
                    TTTPlayer.ClearInventoryUnsafe(PlayerTool.getSteamPlayer(p.SteamID));
                });

                await Task.Delay(15000);

                CommandWindow.Log("Round is live");
                RoundManager.Broadcast("The round has started.");

                RoleManager.GeneratePlayerRoles();

                Players.ToList().ForEach(p =>
                {
                    if (p.GetRole() == PlayerRole.TRAITOR || p.GetRole() == PlayerRole.DETECTIVE)
                    {
                        p.SetCredits(2);
                    }
                });

                State = RoundState.LIVE;
            } catch (Exception ex)
            {
                CommandWindow.Log(ex);
            }
        }

        public async Task Stop()
        {
            CommandWindow.Log("Stopping round");
            RoundManager.Broadcast("Round has ended.");
            RoundTime = 600;

            State = RoundState.INTERMISSION;

            await Task.Delay(6000);

            Players.ToList().ForEach(p =>
            {
                if (p.Status == PlayerStatus.ALIVE)
                {
                    p.ReviveUnsafe();
                    TTTPlayer.ClearInventoryUnsafe(PlayerTool.getSteamPlayer(p.SteamID));
                }
            });

            await Task.Delay(6000);

            State = RoundState.SETUP;

            LevelManager.ClearBarricadesUnsafe();
            C4Manager.ClearC4();
            TrackerGunManager.ClearTrackedPlayers();
        }
        #endregion
    }
}
