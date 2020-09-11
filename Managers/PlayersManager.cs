using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using SDG.Unturned;
using TTTUnturned.Utils;
using System.Collections;
using TTTUnturned.Models;
using UnityEngine;

namespace TTTUnturned.Managers
{
    public class PlayersManager
    {
        public void Awake()
        {
            CommandWindow.Log("PlayersManager loaded");

            Provider.onEnemyConnected += OnEnemyConnected;
            Provider.onEnemyDisconnected += OnEnemyDisconnected;
            PlayerLife.onPlayerDied += OnPlayerDied;

            DamageTool.damagePlayerRequested += OnDamageRequested;
        }

        public static void ClearInventory(SteamPlayer player)
        {
            UnityThread.executeCoroutine(ClearInventoryAsync(player));
        }

        private static IEnumerator ClearInventoryAsync(SteamPlayer player)
        {
            for (byte page = 0; page < 6; page++)
            {
                for (byte i = 0; i < player.player.inventory.items[page].getItemCount(); i++)
                {
                    if (player.player.inventory.items[page].getItem(i) != null)
                    {
                        ItemJar item = player.player.inventory.items[page].getItem(i);
                        player.player.inventory.removeItem(page, player.player.inventory.getIndex(page, item.x, item.y));
                    }
                }
            }
            yield return null;
        }

        public static void TeleportToLocation(SteamPlayer steamPlayer, Vector3 location)
        {
            UnityThread.executeCoroutine(TeleportToLocationAsync(steamPlayer, location));
        }

        private static IEnumerator TeleportToLocationAsync(SteamPlayer steamPlayer, Vector3 location)
        {
            steamPlayer.player.teleportToLocation(location, 0f);

            yield return null;
        }

        public static void SetFlags(SteamPlayer player)
        {
            UnityThread.executeCoroutine(SetFlagsAsync(player));
        }

        private static IEnumerator SetFlagsAsync(SteamPlayer player)
        {
            player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
            player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
            player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
            player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);

            yield return null;
        }

        public static void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            Lobby Lobby = LobbyManager.GetLobby();
            SetFlags(steamPlayer);
            ClearInventory(steamPlayer);

            TeleportToLocation(steamPlayer, PlayersManager.RandomSpawn(Main.Config.LobbySpawns));
            LobbyManager.Message($"<color=red>{steamPlayer.playerID.playerName}</color> has joined!");
            LobbyManager.CheckStart(steamPlayer);
        }

        public static void OnDamageRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            Lobby Lobby = LobbyManager.GetLobby();
            if (Lobby.State != LobbyState.LIVE)
            {
                shouldAllow = false;
            }
        }

        public static void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
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
            RoundManager.CheckWin();
        }

        public static void OnEnemyDisconnected(SteamPlayer steamPlayer)
        {
            Lobby Lobby = LobbyManager.GetLobby();
            LobbyPlayer lPlayer = LobbyManager.GetLobbyPlayer(steamPlayer.playerID.steamID);
            if (lPlayer is null) return;

            Lobby.Players.Remove(lPlayer);
            RoundManager.CheckWin();
        }

        public static Vector3 RandomSpawn(List<Spawn> spawns)
        {
            System.Random rng = new System.Random();
            int t = rng.Next(spawns.Count);
            return new Vector3(spawns[t].X, spawns[t].Y, spawns[t].Z);
        }
    }
}
