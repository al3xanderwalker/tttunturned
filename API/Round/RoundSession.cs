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
using ItemManager = TTTUnturned.API.Level.LevelItems;
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
                State = RoundState.WARMUP;

                ItemManager.RespawnItems();

                await Task.Delay(6000);

                Players.ToList().ForEach(p =>
                {
                    p.SetStatus(PlayerStatus.ALIVE);
                    p.TeleportToMapUnsafe();
                    TTTPlayer.ClearInventoryUnsafe(PlayerTool.getSteamPlayer(p.SteamID));
                });

                await Task.Delay(15000);

                CommandWindow.Log("Round is live");

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
            RoundTime = 600;

            await Task.Delay(6000);

            Players.ToList().ForEach(p =>
            {
                if (p.Status == PlayerStatus.ALIVE)
                {
                    p.ReviveUnsafe();
                }
            });

            await Task.Delay(6000);

            State = RoundState.SETUP;
        }

        public async Task CheckWin()
        {
            CommandWindow.Log("Checking Win");
            if (RoundManager.GetAlivePlayersWithRole(PlayerRole.TRAITOR).Count == 0)
            {
                CommandWindow.Log("Innocents win");
                Players.ToList().ForEach(p => Task.Run(async () => await InterfaceManager.SendBannerMessage(p.SteamID, 8493, "Innocents win!", 6000, true)));
                await Stop();
                return;
            }

            if (RoundManager.GetAlivePlayersWithRole(PlayerRole.DETECTIVE).Count == 0 && RoundManager.GetAlivePlayersWithRole(PlayerRole.INNOCENT).Count == 0)
            {
                CommandWindow.Log("Traitors win");
                Players.ToList().ForEach(p => Task.Run(async () => await InterfaceManager.SendBannerMessage(p.SteamID, 8492, "Traitors win!", 6000, true)));
                await Stop();
                return;
            }
        }
        #endregion
    }
}
