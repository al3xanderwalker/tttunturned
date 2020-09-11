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
    }
}
