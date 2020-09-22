using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using TTTUnturned.API.Interface;
using TTTUnturned.API.Players;
using TTTUnturned.API.Roles;
using TTTUnturned.Utils;
using UnityEngine;
using PlayerManager = TTTUnturned.API.Players.PlayerManager;

namespace TTTUnturned.API.Round
{
    public class RoundManager : MonoBehaviour, IObjectComponent
    {
        private static RoundSession Round;

        public void Awake()
        {
            CommandWindow.Log("RoundManager loaded");

            Round = CreateSession();
            AsyncHelper.Schedule("RoundTimeThread", RoundTimeTick, 1000);

            Provider.onEnemyConnected += OnEnemyConnected;
            Provider.onEnemyDisconnected += OnEnemyDisconnected;

            PlayerLife.onPlayerDied += OnPlayerDied;
            DamageTool.damagePlayerRequested += OnDamagePlayerRequested;

            ChatManager.onChatted += OnChatted;
        }

        #region API
        private RoundSession CreateSession() => new RoundSession();

        public static RoundSession GetSession() => Round;

        private static void StartRound() => Task.Run(async () => await Round.Start());

        public static void StopRound() => Task.Run(async () => await Round.Stop());

        public static RoundState GetState() => Round.State;

        public static void SetState(RoundState state) => Round.State = state;

        public static List<TTTPlayer> GetAllPlayers() => Round.Players;

        public static List<TTTPlayer> GetAllAlivePlayers() => Round.Players.FindAll(p => p.Status == PlayerStatus.ALIVE);

        public static List<TTTPlayer> GetAlivePlayersWithRole(PlayerRole role) => Round.Players.FindAll(p => p.Status == PlayerStatus.ALIVE && p.Role == role);

        public static int GetTimeRemaining() => Round.RoundTime;

        public static void SetTimeRemaining(int time) => Round.RoundTime = time;

        public static void CheckWin() => Task.Run(async () => await Round.CheckWin());

        public static void Broadcast(string message) => UnityThread.executeCoroutine(BroadcastCoroutine(message));
        #endregion

        #region Events
        private void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            TTTPlayer createdPlayer = PlayerManager.CreateTTTPlayer(steamPlayer.playerID.steamID, PlayerRole.NONE, PlayerStatus.DEAD);
            Round.AddPlayer(createdPlayer);

            createdPlayer.Revive();

            Task.Run(async () => await InterfaceManager.SendBannerMessage(createdPlayer.SteamID, 8494, $"Welcome {steamPlayer.playerID.characterName}", 5000, true));
        }

        private void OnEnemyDisconnected(SteamPlayer steamPlayer)
        {
            Round.RemovePlayer(PlayerManager.GetTTTPlayer(steamPlayer.playerID.steamID));

            if (GetState() == RoundState.LIVE)
            {
                CheckWin();
            }
        }

        private void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            TTTPlayer tttPly = PlayerManager.GetTTTPlayer(sender.channel.owner.playerID.steamID);

            if (GetState() == RoundState.LIVE && tttPly.GetStatus() == PlayerStatus.ALIVE)
            {
                CheckWin();
            }

            tttPly.SetStatus(PlayerStatus.DEAD);
            tttPly.Revive();

            InterfaceManager.SendUIEffectTextUnsafe(8490, tttPly.SteamID, true, "RoleValue", "WAITING");
            InterfaceManager.SendUIEffectTextUnsafe(8490, tttPly.SteamID, true, "TimerValue", "00:00");
        }

        private void OnDamagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (GetState() != RoundState.LIVE)
            {
                shouldAllow = false;
            }
        }

        private void OnChatted(SteamPlayer player, EChatMode mode, ref Color chatted, ref bool isRich, string text, ref bool isVisible)
        {
            TTTPlayer tttPlayer = PlayerManager.GetTTTPlayer(player.playerID.steamID);
            if (tttPlayer is null) return;

            if (mode == EChatMode.GLOBAL && GetState() != RoundState.SETUP && !player.isAdmin)
            {
                tttPlayer.SendMessage("Global chat is disabled during rounds, press K for area");
                isVisible = false;
            }
        }
        
        public static string ParseTime(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            return t.ToString(@"mm\:ss");
        }
        #endregion

        #region Threading
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task RoundTimeTick()

        {
            if (GetState() == RoundState.SETUP && GetAllPlayers().Count >= Main.Config.MinimumPlayers)
            {
                CommandWindow.Log("Enough players, attempting start");
                StartRound();
            }

            if (GetState() == RoundState.LIVE)
            {
                Round.Players.ToList().ForEach(p => InterfaceManager.SendUIEffectTextUnsafe(8490, p.SteamID, true, "TimerValue", ParseTime(Round.RoundTime)));
                
                Round.RoundTime--;
            }
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        #endregion

        #region Coroutines
        private static IEnumerator BroadcastCoroutine(string message)
        {
            ChatManager.serverSendMessage(message, Color.white, null, null, EChatMode.GLOBAL, "https://i.imgur.com/UUwQfvY.png", true);
            yield return null;
        }
        #endregion
    }
}
