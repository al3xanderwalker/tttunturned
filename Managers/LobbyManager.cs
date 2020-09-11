using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;
using TTTUnturned.Models;
using Steamworks;
using TTTUnturned.Commands;
using TTTUnturned.Utils;
using TTTUnturned.Managers;
using System.Collections;

namespace TTTUnturned.Managers
{
    public class LobbyManager : MonoBehaviour
    {
        public static Lobby Lobby;

        public void Awake()
        {
            CommandWindow.Log("LobbyManager loaded");

            Lobby = CreateLobbyInitial();

            Provider.onEnemyConnected += PlayersManager.OnEnemyConnected;
            Provider.onEnemyDisconnected += PlayersManager.OnEnemyDisconnected;

            Commander.register(new CommandPos());
            Commander.register(new CommandDiscord());
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
            UnityThread.executeCoroutine(SendMessageAsync(message, target));
        }

        private static IEnumerator SendMessageAsync(string message, SteamPlayer target = null)
        {
            ChatManager.serverSendMessage(message, Color.white, null, target, EChatMode.GLOBAL, "https://i.imgur.com/UUwQfvY.png", true);
            yield return null;
        }
        public static void CheckStart(SteamPlayer player)
        {
            if (Lobby.State == LobbyState.SETUP)
            {
                if (Provider.clients.Count == Main.Config.MinimumPlayers)
                {
                    AsyncHelper.RunAsync("LobbyStart", Lobby.Start);
                    return;
                }
                else if (Provider.clients.Count < Main.Config.MinimumPlayers)
                {
                    Message($"<color=red>{Main.Config.MinimumPlayers - Provider.clients.Count}</color> more players needed to start game.");
                    return;
                }
            }

            if (Lobby.State == LobbyState.LIVE)
            {
                Message($"Game is currently in progress, time left: <color=red>{RoundManager.ParseTime(Lobby.RoundTime - Main.Config.RoundLength)}</color>", player);
                return;
            }
        }
    }
}
