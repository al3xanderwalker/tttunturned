using SDG.Unturned;
using UnityEngine;
using Steamworks;
using TTTUnturned.Utils;
using System.Collections;
using TTTUnturned.API.Commands;
using TTTUnturned.API.Core;

namespace TTTUnturned.API.Lobby
{
    public class LobbyManager : MonoBehaviour, IObjectComponent
    {
        public static LobbySession Lobby;

        public void Awake()
        {
            CommandWindow.Log("LobbyManager loaded");

            Lobby = CreateLobbyInitial();

            Commander.register(new CommandPos());
            Commander.register(new CommandDiscord());
        }

        private LobbySession CreateLobbyInitial()
        {
            LobbySession createdSession = new LobbySession(LobbyState.SETUP);
            return createdSession;
        }

        public static LobbySession GetLobby()
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
    }
}
