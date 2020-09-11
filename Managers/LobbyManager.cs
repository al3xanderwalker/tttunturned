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
using System.Collections;

namespace TTTUnturned.Managers
{
    public class LobbyManager : MonoBehaviour
    {
        public static Lobby Lobby;
        public static int PlayersRequired;

        public void Awake()
        {
            CommandWindow.Log("LobbyManager loaded");

            Lobby = CreateLobbyInitial();
            if (Main.Config.DebugMode) PlayersRequired = 2;
            else PlayersRequired = 5;

            Provider.onEnemyConnected += OnEnemyConnected;
            Provider.onEnemyDisconnected += OnEnemyDisconnected;

            Commander.register(new CommandPos());
            Commander.register(new CommandDiscord());
        }

        private void OnEnemyDisconnected(SteamPlayer steamPlayer)
        {
            LobbyPlayer lPlayer = GetLobbyPlayer(steamPlayer.playerID.steamID);
            if (lPlayer is null) return;

            Lobby.Players.Remove(lPlayer);
            RoundManager.CheckWin();
        }

        private void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            steamPlayer.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
            steamPlayer.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
            steamPlayer.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
            steamPlayer.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);

            ClearInventory(steamPlayer.playerID.steamID);
            System.Random rng = new System.Random();
            List<Spawn> spawns = Main.Config.LobbySpawns;

            int t = rng.Next(spawns.Count);
            Vector3 spawn = new Vector3(spawns[t].X, spawns[t].Y, spawns[t].Z);
            Lobby.TeleportToLocation(steamPlayer, spawn);

            if (Lobby.State == LobbyState.SETUP)
            {
                if (Provider.clients.Count == PlayersRequired)
                {
                    Message($"<color=red>{steamPlayer.playerID.playerName}</color> has joined!");
                    AsyncHelper.RunAsync("LobbyStart", Lobby.Start);
                    return;
                }
                else if (Provider.clients.Count < PlayersRequired)
                {
                    Message($"<color=red>{steamPlayer.playerID.playerName}</color> has joined, <color=red>{PlayersRequired - Provider.clients.Count}</color> more players needed to start game.");
                    return;
                }
            }

            if (Lobby.State == LobbyState.LIVE)
            {
                Message($"<color=red>{steamPlayer.playerID.playerName}</color> has joined!");
                Message($"Game is currently in progress, time left: <color=red>{RoundManager.ParseTime(Lobby.RoundTime - Main.Config.RoundLength)}</color>", steamPlayer);
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
            UnityThread.executeCoroutine(SendMessageAsync(message, target));
        }

        private static IEnumerator SendMessageAsync(string message, SteamPlayer target = null)
        {
            ChatManager.serverSendMessage(message, Color.white, null, target, EChatMode.GLOBAL, "https://i.imgur.com/UUwQfvY.png", true);
            yield return null;
        }

        public void ClearInventory(CSteamID steamID)
        {
            UnityThread.executeCoroutine(ClearInventoryAsync(steamID));
        }

        private IEnumerator ClearInventoryAsync(CSteamID steamID)
        {
            Player ply = PlayerTool.getPlayer(steamID);
            for (byte page = 0; page < 6; page++)
            {
                for (byte i = 0; i < ply.inventory.items[page].getItemCount(); i++)
                {
                    if (ply.inventory.items[page].getItem(i) != null)
                    {
                        ItemJar item = ply.inventory.items[page].getItem(i);
                        ply.inventory.removeItem(page, ply.inventory.getIndex(page, item.x, item.y));
                    }
                }
            }
            yield return null;
        }
    }
}
