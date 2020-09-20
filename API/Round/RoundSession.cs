using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Players;
using TTTUnturned.API.Roles;
using ItemManager = TTTUnturned.API.Level.ItemManager;
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

                ItemManager.RespawnItems();

                Players.ToList().ForEach(p =>
                {
                    p.SetStatus(PlayerStatus.ALIVE);
                    p.TeleportToMapUnsafe();
                    TTTPlayer.ClearInventoryUnsafe(PlayerTool.getSteamPlayer(p.SteamID));
                });

                State = RoundState.WARMUP;

                await Task.Delay(15000);

                CommandWindow.Log("Round is live");

                RoleManager.GeneratePlayerRoles();

                State = RoundState.LIVE;
            } catch (Exception ex)
            {
                CommandWindow.Log(ex);
            }
        }

        public async Task Stop()
        {
            CommandWindow.Log("Stopping round");
            State = RoundState.SETUP;
            RoundTime = 600;

            await Task.Delay(6000);

            Players.ToList().ForEach(p =>
            {
                if (p.Status == PlayerStatus.ALIVE)
                {
                    p.TeleportToLobby();
                }
            });
        }

        public async Task CheckWin()
        {
            CommandWindow.Log("Checking Win");
            if (RoundManager.GetAlivePlayers(PlayerRole.TRAITOR).Count == 0)
            {
                CommandWindow.Log("Innocents win");
                await Stop();
                return;
            }

            if (RoundManager.GetAlivePlayers(PlayerRole.DETECTIVE).Count == 0 && RoundManager.GetAlivePlayers(PlayerRole.INNOCENT).Count == 0)
            {
                CommandWindow.Log("Traitors win");
                await Stop();
                return;
            }
        }
        #endregion
    }
}
