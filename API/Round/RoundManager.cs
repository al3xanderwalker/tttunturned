using SDG.Unturned;
using System;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using TTTUnturned.API.Interface;
using TTTUnturned.API.Lobby;
using TTTUnturned.API.Roles;
using TTTUnturned.Utils;
using UnityEngine;

namespace TTTUnturned.API.Round
{
    public class RoundManager : MonoBehaviour, IObjectComponent
    {

        public void Awake()
        {
            CommandWindow.Log("RoundManager loaded");

            AsyncHelper.Schedule("RoundTick", RoundTick, 1000);
        }

        public static void CheckWin()
        {
            LobbySession lobby = LobbyManager.GetLobby();
            Task.Run(async () =>
            {
                if (lobby.GetAlive(PlayerRole.TERRORIST).Count == 0)
                {
                    CommandWindow.Log("Innocents win");
                    LobbyManager.Message("<color=lime>Innocents</color> Win!");
                    await InterfaceManager.SendLobbyBannerMessage(8493, $"Innocents Win!", 10000, true);
                    await lobby.Stop();

                    // Innocents win
                }
                if (lobby.GetAlive(PlayerRole.DETECTIVE).Count == 0 && lobby.GetAlive(PlayerRole.INNOCENT).Count == 0)
                {
                    CommandWindow.Log("Terrorist win");
                    LobbyManager.Message("<color=red>Terroists</color> Win!");
                    await InterfaceManager.SendLobbyBannerMessage(8492, $"Terroists Win!", 10000, true);
                    await lobby.Stop();
                    // Terrorist win
                }
            });
        }

        private async Task RoundTick()
        {
            LobbySession lobby = LobbyManager.GetLobby();

            if (lobby.State != LobbyState.LIVE) return;

            lobby.RoundTime--;
            if(Main.Config.DebugMode) CommandWindow.Log(lobby.RoundTime);

            lobby.Players.ForEach(async player =>
            {
                await InterfaceManager.SendUIEffectTextAsync(8490, player.SteamID, true, "TimerValue", ParseTime(lobby.RoundTime));
               // await SendUIEffectAsync();
            });

            if (lobby.RoundTime == 60) // Convert this to updating the time displayed on the ui once added.
            {
                LobbyManager.Message("1 minute remaining");
            }

            if (lobby.RoundTime == 0)
            {
                CommandWindow.Log("Innocents win");
                LobbyManager.Message("<color=lime>Innocents</color> Win!");

                await lobby.Stop();
                //AsyncHelper.RunAsync("LobbyStopRoundTime", Lobby.Stop);
                return;
            }
        }

        public static string ParseTime(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            return t.ToString(@"mm\:ss");
        }
    }
}
