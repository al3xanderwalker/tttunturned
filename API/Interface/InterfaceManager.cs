using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using TTTUnturned.API.Lobby;
using TTTUnturned.API.Round;
using TTTUnturned.Utils;
using UnityEngine;
using PlayerManager = TTTUnturned.API.Players.PlayerManager;
using System.Collections.Generic;
using TTTUnturned.API.Players;
using TTTUnturned.API.Roles;
using TTTUnturned.API.Items.SilencedPistol;
namespace TTTUnturned.API.Interface
{
    public class InterfaceManager : MonoBehaviour, IObjectComponent
    {
        private static Dictionary<CSteamID, long> keyCooldowns;

        public void Awake()
        {
            CommandWindow.Log("InterfaceManager loaded");

            keyCooldowns = new Dictionary<CSteamID, long>();

            EffectManager.onEffectButtonClicked += OnEffectButtonClicked;
            PlayerInput.onPluginKeyTick += OnPluginKeyTick;
            Provider.onEnemyConnected += OnEnemyConnected;
        }

        public void SendEffectAsync(ushort id, byte x, byte y, byte z, Vector3 position)
        {
            UnityThread.executeCoroutine(SendEffectCoroutine(id, x, y, z, position));
        }

        private void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            DisableExtraHUD(steamPlayer.playerID.steamID);
            SendUIEffectAsync(8498, 8490, steamPlayer.playerID.steamID, true);
            SendUIEffectTextAsync(8490, steamPlayer.playerID.steamID, true, "RoleValue", "WAITING");
            SendUIEffectTextAsync(8490, steamPlayer.playerID.steamID, true, "TimerValue", "00:00");
        }

        public void OnEffectButtonClicked(Player player, string buttonName)
        {
            if (buttonName.Substring(0, 2) == "T_")
            {
                switch (buttonName.Remove(0, 2))
                {
                    case "ChargeButton":
                        player.inventory.forceAddItem(new Item(1241, true), true);
                        RoundManager.Broadcast("You redeemed C4", player.channel.owner);
                        break;
                    case "CoughSyrupButton":
                        player.inventory.forceAddItem(new Item(404, true), true);
                        RoundManager.Broadcast("You redeemed Cough Syrup", player.channel.owner);
                        break;
                    case "KnifeButton":
                        player.inventory.forceAddItem(new Item(140, true), true);
                        RoundManager.Broadcast("You redeemed Knife", player.channel.owner);
                        break;
                    case "LMGButton":
                        player.inventory.forceAddItem(new Item(126, true), true);
                        RoundManager.Broadcast("You redeemed LMG", player.channel.owner);
                        break;
                    case "SupressedPistol":
                        player.inventory.forceAddItem(SilencedPistol.Create(), true);
                        RoundManager.Broadcast("You redeemed Suppresed Pistol", player.channel.owner);
                        break;
                    case "BombVestButton":
                        player.inventory.forceAddItem(new Item(1013, true), true);
                        RoundManager.Broadcast("You redeemed Bomb Vest", player.channel.owner);
                        break;
                    case "BodyArmourButton":
                        PlayerManager.GetTTTPlayer(player.channel.owner.playerID.steamID).Armor = true;
                        RoundManager.Broadcast("You redeemed Armor Vest", player.channel.owner);
                        break;
                }
            }
        }
        private void OnPluginKeyTick(Player player, uint simulation, byte key, bool state)
        {
            if (!state || key != 0) return;
            TTTPlayer tttPlayer = PlayerManager.GetTTTPlayer(player.channel.owner.playerID.steamID);
            if (tttPlayer is null) return;

            if (tttPlayer.Status == Status.DEAD) return;
            if (RoundManager.GetRoundSessionState() != RoundState.LIVE) return;


            if (keyCooldowns.ContainsKey(player.channel.owner.playerID.steamID))
            {
                long lastPressed = keyCooldowns[player.channel.owner.playerID.steamID];
                // 1 second key cooldown on menu
                if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastPressed < 1000) return;

                keyCooldowns.Remove(player.channel.owner.playerID.steamID);
            }

            keyCooldowns.Add(player.channel.owner.playerID.steamID, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            if (tttPlayer.Role == Role.TRAITOR)
            {
                if (tttPlayer.UIToggled)
                {
                    tttPlayer.UIToggled = false;
                    ClearUIEffectAsync(8501, tttPlayer.SteamID);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, false);
                }
                else
                {
                    tttPlayer.UIToggled = true;
                    SendUIEffectAsync(8501, 8470, tttPlayer.SteamID, true);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, true);
                }

            }
        }
        #region Functions
        public static async Task SendLobbyBannerMessage(ushort id, string message, int duration, bool reliable)
        {
            Provider.clients.ToList().ForEach(async player =>
            {
                Task.Run(async () => { await SendBannerMessage(player.playerID.steamID, id, message, duration, reliable); });
            });
        }

        public static async Task ClearStatusUIAsync(TTTPlayer player)
        {
            ClearUIEffectAsync(8497, player.SteamID);
            ClearUIEffectAsync(8499, player.SteamID);
            ClearUIEffectAsync(8496, player.SteamID);
            ClearUIEffectAsync(8498, player.SteamID);
        }

        public static async Task SendBannerMessage(CSteamID steamId, ushort id, string message, int duration, bool reliable)
        {
            await SendUIEffectAsync(id, 8480, steamId, reliable);
            await SendUIEffectTextAsync(8480, steamId, true, "RoleValue", message);
            await Task.Delay(duration);
            await ClearUIEffectAsync(id, steamId);
        }

        public static async Task ClearUIEffectAsync(ushort id, CSteamID steamID)
        {
            UnityThread.executeCoroutine(ClearUIEffectCoroutine(id, steamID));
        }

        public static async Task SendUIEffectAsync(ushort id, short key, CSteamID steamID, bool reliable)
        {
            UnityThread.executeCoroutine(SendUIEffectCoroutine(id, key, steamID, reliable));
        }

        public static async Task SendUIEffectTextAsync(short key, CSteamID steamID, bool reliable, string component, string text)
        {
            UnityThread.executeCoroutine(SendUIEffectTextCoroutine(key, steamID, reliable, component, text));
        }

        public static void DisableExtraHUD(CSteamID steamID)
        {
            UnityThread.executeCoroutine(DisableExtraHUDCoroutine(steamID));
        }
        #endregion
        #region Coroutines
        private static IEnumerator SendEffectCoroutine(ushort id, byte x, byte y, byte z, Vector3 position)
        {
            EffectManager.sendEffect(id, x, y, z, position);
            yield return null;
        }
        private static IEnumerator SendUIEffectTextCoroutine(short key, CSteamID steamID, bool reliable, string component, string text)
        {
            EffectManager.sendUIEffectText(key, steamID, reliable, component, text);
            yield return null;
        }
        private static IEnumerator ClearUIEffectCoroutine(ushort id, CSteamID steamID)
        {
            EffectManager.askEffectClearByID(id, steamID);
            yield return null;
        }
        private static IEnumerator SendUIEffectCoroutine(ushort id, short key, CSteamID steamID, bool reliable)
        {
            EffectManager.sendUIEffect(id, key, steamID, reliable);
            yield return null;
        }
        private static IEnumerator DisableExtraHUDCoroutine(CSteamID steamID)
        {
            Player ply = PlayerTool.getPlayer(steamID);
            if (ply is null) yield return null;

            ply.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
            ply.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
            ply.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
            ply.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);
            ply.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
            yield return null;
        }
        #endregion
    }
}
