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
    public class PlayerManager : MonoBehaviour
    {
        public void Awake()
        {
            CommandWindow.Log("PlayerManager loaded");

            Provider.onEnemyConnected += OnEnemyConnected;
            Provider.onEnemyDisconnected += OnEnemyDisconnected;
            DamageTool.damagePlayerRequested += OnDamageRequested;
            PlayerLife.onPlayerDied += OnPlayerDied;
        }

        #region Events
        private void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            Lobby Lobby = LobbyManager.GetLobby();
            LobbyManager.Message($"<color=red>{steamPlayer.playerID.playerName}</color> has joined!");

            // Manually call Task.Run since we have to pass parameters
            Task.Run(() =>
            {
                UIManager.SendBannerMessage(steamPlayer.playerID.steamID, 8494, $"Welcome {steamPlayer.playerID.playerName} to <color=red>TTT</color>", 10000, true);
                
                UIManager.SendUIEffectAsync(8498, 8490, steamPlayer.playerID.steamID, true);
                UIManager.SendUIEffectTextAsync(8490, steamPlayer.playerID.steamID, true, "RoleValue", "WAITING");
                UIManager.SendUIEffectTextAsync(8490, steamPlayer.playerID.steamID, true, "TimerValue", "00:00");

                DisableHUDAsync(steamPlayer);
                ClearInventoryAsync(steamPlayer);
                TeleportToLocationAsync(steamPlayer, GetRandomSpawn(Main.Config.LobbySpawns));
                Lobby.Start();
            });
        }

        public static void OnEnemyDisconnected(SteamPlayer steamPlayer)
        {
            Lobby Lobby = LobbyManager.GetLobby();
            LobbyPlayer lobbyPlayer = LobbyManager.GetLobbyPlayer(steamPlayer.playerID.steamID);
            if (lobbyPlayer is null) return;

            Lobby.Players.Remove(lobbyPlayer);
            RoundManager.CheckWin();
        }

        private void OnDamageRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            Lobby Lobby = LobbyManager.GetLobby();
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
            RoundManager.CheckWin();
        }
        #endregion

        public static async Task ClearInventoryAsync(SteamPlayer player)
        {
            UnityThread.executeCoroutine(ClearInventoryCoroutine(player));
        }

        private static IEnumerator ClearInventoryCoroutine(SteamPlayer player)
        {
            for (byte page = 0; page < 6; page++)
            {
                for (byte i = 0; i < player.player.inventory.items[page].getItemCount(); i++)
                {
                    ItemJar item = player.player.inventory.items[page].getItem(i);
                    player.player.inventory.removeItem(page, player.player.inventory.getIndex(page, item.x, item.y));
                    /*
                    if (player.player.inventory.items[page].getItem(i) != null)
                    {

                    }
                    */
                }
            }
            yield return null;
        }

        public static async Task TeleportToLocationAsync(SteamPlayer steamPlayer, Vector3 location)
        {
            UnityThread.executeCoroutine(TeleportToLocationCoroutine(steamPlayer, location));
        }

        private static IEnumerator TeleportToLocationCoroutine(SteamPlayer steamPlayer, Vector3 location)
        {
            steamPlayer.player.teleportToLocation(location, 0f);

            yield return null;
        }

        public static async Task DisableHUDAsync(SteamPlayer player)
        {
            UnityThread.executeCoroutine(DisableHUDCoroutine(player));
        }

        private static IEnumerator DisableHUDCoroutine(SteamPlayer player)
        {
            player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
            player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
            player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
            player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);
            player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
            yield return null;
        }

        public static Vector3 GetRandomSpawn(List<Spawn> spawns)
        {
            System.Random rng = new System.Random();
            int t = rng.Next(spawns.Count);
            return new Vector3(spawns[t].X, spawns[t].Y, spawns[t].Z);
        }
    }
}
