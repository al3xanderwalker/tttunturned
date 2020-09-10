using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TTTUnturned.Models;
using TTTUnturned.Managers;

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
        }

        private void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            if (Lobby.State == LobbyState.SETUP)
            {
                if (Provider.clients.Count == playersRequired)
                {
                    message($"<color=red>{steamPlayer.playerID.playerName}</color> has joined!");
                    Lobby.Start();
                    return;
                }
                else if (Provider.clients.Count < playersRequired)
                {
                    message($"<color=red>{steamPlayer.playerID.playerName}</color> has joined, <color=red>{playersRequired - Provider.clients.Count}</color> more players needed to start game.");
                    return;
                }
            }

            if (Lobby.State == LobbyState.LIVE)
            {
                message($"<color=red>{steamPlayer.playerID.playerName}</color> has joined!");
                message($"Game is currently in progress, time left: <color=red>{RoundManager.ParseTime(Main.Config.RoundLength - Lobby.RoundTime)}</color>", steamPlayer);
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

        public static void message(string message, SteamPlayer target = null)
        {
            ChatManager.serverSendMessage(message, Color.white, null, target, EChatMode.GLOBAL, "https://image.winudf.com/v2/image/Y29tLmlvbmljZnJhbWV3b3JrLnR0dDMxOTQ5OV9pY29uXzBfYjAxN2RkMGE/icon.png?w=170&fakeurl=1", true);
        }
    }
}
