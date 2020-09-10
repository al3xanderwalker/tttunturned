using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TTTUnturned.Models;
using TTTUnturned.Managers;
using Steamworks;
using TTTUnturned.Commands;
using TTTUnturned.Utils;

namespace TTTUnturned.Managers
{
    public class LobbyManager : MonoBehaviour
    {
        public static Lobby Lobby;
        public static int playersRequired;

        public void Awake()
        {
            CommandWindow.Log("LobbyManager loaded");

            Lobby = CreateLobbyInitial();
            if (Main.Config.DebugMode) playersRequired = 2;
            else playersRequired = 5;

            Provider.onEnemyConnected += OnEnemyConnected;

            Commander.register(new CommandPos());
        }

        private void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            if (Lobby.State == LobbyState.SETUP)
            {
                if (Provider.clients.Count == playersRequired)
                {
                    Message($"<color=red>{steamPlayer.playerID.playerName}</color> has joined!");
                    AsyncHelper.RunAsync("LobbyStart", Lobby.Start);
                    Lobby.Start();
                    return;
                }
                else if (Provider.clients.Count < playersRequired)
                {
                    Message($"<color=red>{steamPlayer.playerID.playerName}</color> has joined, <color=red>{playersRequired - Provider.clients.Count}</color> more players needed to start game.");
                    return;
                }
            }

            if (Lobby.State == LobbyState.LIVE)
            {
                Message($"<color=red>{steamPlayer.playerID.playerName}</color> has joined!");
                Message($"Game is currently in progress, time left: <color=red>{RoundManager.ParseTime(Main.Config.RoundLength - Lobby.RoundTime)}</color>", steamPlayer);
                return;
            }
        }

        private Lobby CreateLobbyInitial()
        {
            Lobby createdSession = new Lobby(LobbyState.SETUP);
            return createdSession;
        }

        public static Lobby GetLobby()
        {
            return Lobby;
        }

        public static LobbyPlayer GetLobbyPlayer(CSteamID steamID)
        {
            return Lobby.Players.Find(x => x.SteamID == steamID);
        }

        public static void Message(string message, SteamPlayer target = null)
        {
            ChatManager.serverSendMessage(message, Color.white, null, target, EChatMode.GLOBAL, "https://i.imgur.com/UUwQfvY.png", true);
        }

        IEnumerator SendMessageAsync(string message, SteamPlayer target = null)
        {
            ChatManager.serverSendMessage(message, Color.white, null, target, EChatMode.GLOBAL, "https://i.imgur.com/UUwQfvY.png", true);

            yield return null;
        }
    }
}
