using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTTUnturned.Models;
using TTTUnturned.Utils;
using UnityEngine;

namespace TTTUnturned.Managers
{
    public class RoundManager : MonoBehaviour
    {
        public static Lobby Lobby;

        public void Awake()
        {
            Lobby = LobbyManager.GetLobby();

            CommandWindow.Log("RoundManager loaded");

            AsyncHelper.Schedule("RoundTick", RoundTick, 1000);
        }

        public static void CheckWin()
        {
            if (Lobby.GetAlive(PlayerRole.TERRORIST).Count == 0)
            {
                CommandWindow.Log("Innocents win");
                LobbyManager.Message("<color=lime>Innocents</color> Win!");

                AsyncHelper.RunAsync("LobbyStopCheckWinT", Lobby.Stop);
                // Innocents win
            }
            if (Lobby.GetAlive(PlayerRole.DETECTIVE).Count == 0 && Lobby.GetAlive(PlayerRole.INNOCENT).Count == 0)
            {
                CommandWindow.Log("Terrorist win");
                LobbyManager.Message("<color=red>Terroists</color> Win!");
                AsyncHelper.RunAsync("LobbyStopCheckWinInnocent", Lobby.Stop);
                // Terrorist win
            }
        }

        private async Task RoundTick()
        {
            Lobby lobby = LobbyManager.GetLobby();

            if (lobby.State != LobbyState.LIVE) return;

            lobby.RoundTime--;
            if(Main.Config.DebugMode) CommandWindow.Log(lobby.RoundTime);

            lobby.Players.ForEach(async player =>
            {
                await SendUIEffectTextAsync(8490, player.SteamID, true, "TimeValue", ParseTime(lobby.RoundTime));
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

                await Lobby.Stop();
                //AsyncHelper.RunAsync("LobbyStopRoundTime", Lobby.Stop);
                return;
            }
        }

        private static async Task SendUIEffectTextAsync(short key, CSteamID steamID, bool reliable, string component, string text)
        {
            UnityThread.executeCoroutine(SendUIEffectTextCoroutine(key, steamID, reliable, component, text));
        }

        private static IEnumerator SendUIEffectTextCoroutine(short key, CSteamID steamID, bool reliable, string component, string text)
        {
            EffectManager.sendUIEffectText(key, steamID, reliable, component, text);
            yield return null;
        }


        public static string ParseTime(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            return t.ToString(@"mm\:ss");
        }
    }
}
