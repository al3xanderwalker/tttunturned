using System.Linq;
using System.Threading.Tasks;
using SDG.Unturned;
using TTTUnturned.Utils;
using Steamworks;
using System.Collections;
using UnityEngine;
using TTTUnturned.API.Lobby;
using TTTUnturned.API.Core;
using TTTUnturned.API.Round;
using PlayerManager = TTTUnturned.API.Players.PlayerManager;
using System;
namespace TTTUnturned.API.Interface
{
    public class InterfaceManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.Log("InterfaceManager loaded");

            EffectManager.onEffectButtonClicked += OnEffectButtonClicked;
        }
        
        public void SendEffectAsync(ushort id, byte x, byte y, byte z, Vector3 position)
        {
            UnityThread.executeCoroutine(SendEffectCoroutine(id, x, y, z, position));
        }
        private static IEnumerator SendEffectCoroutine(ushort id, byte x, byte y, byte z, Vector3 position)
        {
            EffectManager.sendEffect(id,x,y,z,position);
            yield return null;
        }

        public void OnEffectButtonClicked(Player player, string buttonName)
        {
            if(buttonName.Substring(0,2) == "T_")
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
                        Item gun = new Item(1021,true);
                        Asset SelectAsset = Assets.find(EAssetType.ITEM, 7);
                        ItemAsset Item = (ItemAsset)SelectAsset;
                        byte[] ID = BitConverter.GetBytes(Item.id);
                        Array.Copy(ID, 0, gun.state, 6, 2);
                        player.inventory.forceAddItem(gun, true);
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

        public static async Task SendLobbyBannerMessage(ushort id, string message, int duration, bool reliable)
        {
            Provider.clients.ToList().ForEach(async player =>
            {
                Task.Run(async () => {  await SendBannerMessage(player.playerID.steamID, id, message, duration, reliable);  });
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
            UnityThread.executeCoroutine(ClearUIEffectCoroutine(id,steamID));
        }

        private static IEnumerator ClearUIEffectCoroutine(ushort id, CSteamID steamID)
        {
            EffectManager.askEffectClearByID(id, steamID);
            yield return null;
        }
        public static async Task SendUIEffectAsync(ushort id, short key, CSteamID steamID, bool reliable)
        {
            UnityThread.executeCoroutine(SendUIEffectCoroutine(id, key, steamID, reliable));
        }

        private static IEnumerator SendUIEffectCoroutine(ushort id, short key, CSteamID steamID, bool reliable)
        {
            EffectManager.sendUIEffect(id,key, steamID, reliable);
            yield return null;
        }

        public static async Task SendUIEffectTextAsync(short key, CSteamID steamID, bool reliable, string component, string text)
        {
            UnityThread.executeCoroutine(SendUIEffectTextCoroutine(key, steamID, reliable, component, text));
        }

        private static IEnumerator SendUIEffectTextCoroutine(short key, CSteamID steamID, bool reliable, string component, string text)
        {
            EffectManager.sendUIEffectText(key, steamID, reliable, component, text);
            yield return null;
        }

        public static void DisableExtraHUD(CSteamID steamID)
        {
            UnityThread.executeCoroutine(DisableExtraHUDCoroutine(steamID));
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
    }
}
