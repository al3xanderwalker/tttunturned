using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.Models;
using TTTUnturned.Managers;
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

            PlayerLife.onPlayerDied += OnPlayerDied;

            DamageTool.damagePlayerRequested += OnDamageRequested;
        }

        private void OnDamageRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (Lobby.State != LobbyState.LIVE)
            {
                shouldAllow = false;
            }
        }

        private void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            System.Random rng = new System.Random();
            List<Spawn> spawns = Main.Config.LobbySpawns;
            int t = rng.Next(spawns.Count);
            Vector3 spawn = new Vector3(spawns[t].X, spawns[t].Y, spawns[t].Z);

            sender.channel.send("tellRevive", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] // thanks alien man
            {
                spawn,
                (byte) 0
            });
            sender.sendRevive();

            LobbyPlayer player = LobbyManager.GetLobbyPlayer(sender.channel.owner.playerID.steamID);
            if (player is null || player.Status == PlayerStatus.DEAD) return;

            player.Status = PlayerStatus.DEAD;
            CheckWin();
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

            if (lobby.RoundTime == 60)
            {
                // TODO: Make this execute async so it doesnt block the thread
                UnityThread.executeInUpdate(() => { LobbyManager.Message("1 minute remaining"); });
            }

            if (lobby.RoundTime == 0)
            {
                UnityThread.executeInUpdate(() =>
                {
                    CommandWindow.Log("Innocents win");
                    LobbyManager.Message("<color=lime>Innocents</color> Win!");

                    AsyncHelper.RunAsync("LobbyStopRoundTime", Lobby.Stop);
                });
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
