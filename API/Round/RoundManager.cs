using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
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

        public static List<TTTPlayer> GetAlivePlayers(PlayerRole role) => Round.Players.FindAll(p => p.Status == PlayerStatus.ALIVE && p.Role == role);

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
        }

        private void OnDamagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (GetState() != RoundState.LIVE)
            {
                shouldAllow = false;
            }
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
                Round.RoundTime--;
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        #endregion
        private static IEnumerator BroadcastCoroutine(string message)
        {
            ChatManager.serverSendMessage(message, Color.white, null, null, EChatMode.GLOBAL, "https://i.imgur.com/UUwQfvY.png", true);
            yield return null;
        }
    }
}
