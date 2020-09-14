using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using TTTUnturned.API.Interface;
using TTTUnturned.API.Lobby;
using TTTUnturned.API.Players;
using TTTUnturned.API.Roles;
using TTTUnturned.Utils;
using UnityEngine;
using PlayerManager = TTTUnturned.API.Players.PlayerManager;

namespace TTTUnturned.API.Round
{
    public class RoundManager : MonoBehaviour, IObjectComponent
    {
        private static RoundSession RoundSession;

        public void Awake()
        {
            RoundSession = CreateRoundSessionInitial();
            CommandWindow.Log("RoundManager loaded");

            AsyncHelper.Schedule("RoundTick", RoundTick, 1000);

            Provider.onEnemyConnected += OnEnemyConnected;
            Provider.onEnemyDisconnected += OnEnemyDisconnected;
        }

        private void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            if (RoundSession.State == RoundState.LIVE || RoundSession.State == RoundState.WARMUP)
            {
                PlayerManager.TeleportToLocationAsync(steamPlayer, PlayerManager.GetRandomSpawn(Main.Config.LobbySpawns));
                return;
            }

            if (Provider.clients.ToList().Count >= Main.Config.MinimumPlayers)
            {
                AsyncHelper.RunAsync("RoundStart", RoundSession.Start);
            }
            else
            {
                Broadcast($"<color=red>{Main.Config.MinimumPlayers - Provider.clients.Count}</color> more players needed to start game.");
                InterfaceManager.SendLobbyBannerMessage(8494, $"<size=20><color=red>{Main.Config.MinimumPlayers - Provider.clients.Count}</color> more players needed to start game.</size>", 5000, true);
            }
        }

        public static void OnEnemyDisconnected(SteamPlayer steamPlayer)
        {
            TTTPlayer tttPlayer = PlayerManager.GetTTTPlayer(steamPlayer.playerID.steamID);
            if (tttPlayer is null) return;

            RoundSession.Players.Remove(tttPlayer);
            RoundManager.CheckWin();
        }

        public static List<TTTPlayer> GetPlayers() => RoundSession.Players;

        public static void Broadcast(string message, SteamPlayer toPlayer = null) => UnityThread.executeCoroutine(BroadcastCoroutine(message, toPlayer));

        private static IEnumerator BroadcastCoroutine(string message, SteamPlayer toPlayer = null)
        {
            ChatManager.serverSendMessage(message, Color.white, null, toPlayer, EChatMode.GLOBAL, "https://i.imgur.com/UUwQfvY.png", true);
            yield return null;
        } 

        public static RoundState GetRoundSessionState()
        {
            return RoundSession.State;
        }

        public static List<TTTPlayer> GetAlive(Role role) => RoundSession.Players.FindAll(p => p.Role == role && p.Status == Status.ALIVE);

        public static void CheckWin()
        {
            Task.Run(async () =>
            {
                if (GetAlive(Role.TRAITOR).Count == 0)
                {
                    Broadcast("<color=lime>Innocents</color> Win!");
                    await InterfaceManager.SendLobbyBannerMessage(8493, $"Innocents Win!", 10000, true);
                    await RoundSession.Stop();
                    // Innocents win
                }
                if (GetAlive(Role.DETECTIVE).Count == 0 && GetAlive(Role.INNOCENT).Count == 0)
                {
                    Broadcast("<color=red>Terroists</color> Win!");
                    await InterfaceManager.SendLobbyBannerMessage(8492, $"Terroists Win!", 10000, true);
                    await RoundSession.Stop();
                    // Terrorist win
                }
            });
        }

        private RoundSession CreateRoundSessionInitial() => new RoundSession();

        private async Task RoundTick()
        {
            if (RoundSession.State != RoundState.LIVE) return;

            RoundSession.RoundTime--;

            RoundSession.Players.ForEach(async player =>
            {
                await InterfaceManager.SendUIEffectTextAsync(8490, player.SteamID, true, "TimerValue", ParseTime(RoundSession.RoundTime));
               // await SendUIEffectAsync();
            });

            if (RoundSession.RoundTime == 60) // Convert this to updating the time displayed on the ui once added.
            {
                Broadcast("1 minute remaining");
            }

            if (RoundSession.RoundTime == 0)
            {
                Broadcast("<color=lime>Innocents</color> Win!");

                await RoundSession.Stop();
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
