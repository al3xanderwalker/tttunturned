using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using TTTUnturned.Utils;
using Steamworks;
using System.Collections;
using UnityEngine;
using TTTUnturned.Models;

namespace TTTUnturned.Managers
{
    public class UIManager : MonoBehaviour
    {
        public void Awake()
        {
            CommandWindow.Log("UIManager loaded");

            EffectManager.onEffectButtonClicked += OnEffectButtonClicked;
            BarricadeManager.onBarricadeSpawned += OnBarricadeSpawned;
        }
        public void OnBarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
        {
            if (drop.asset.id != 1241) return;
            CommandWindow.Log(drop.asset.id);
            
            Task.Run(() =>
            {
                C4(region,drop);
            });
        }
        public async Task C4(BarricadeRegion region,BarricadeDrop drop)
        {
            await Task.Delay(10000);
            SendEffectAsync(45, byte.MaxValue, byte.MaxValue, byte.MaxValue, drop.model.position);
            ExplosionParameters parameters = new ExplosionParameters(drop.model.position,10f, EDeathCause.KILL, CSteamID.Nil);
            parameters.penetrateBuildables = true;
            parameters.playerDamage = 10;
            parameters.damageRadius = 32;
            ExplodeAsync(parameters);
            DestroyAsync(region);
        }
        public void DestroyAsync(BarricadeRegion region)
        {
            UnityThread.executeCoroutine(DestroyCoroutine(region));
        }
        private static IEnumerator DestroyCoroutine(BarricadeRegion region)
        {
            region.destroy(); // DESTROYS IT BUT doesnt delete the in game model

            yield return null;
        }

        public void ExplodeAsync(ExplosionParameters parameters)
        {
            UnityThread.executeCoroutine(ExplodeCoroutine(parameters));
        }
        private static IEnumerator ExplodeCoroutine(ExplosionParameters parameters)
        {
            List<EPlayerKill> deadPlayers = new List<EPlayerKill>();
            DamageTool.explode(parameters, out deadPlayers);

            yield return null;
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
                        player.inventory.forceAddItem(new Item(1240, true), true);
                        SteamPlayer ply = Provider.clients.ToList().Find(x => x.player == player);
                        LobbyManager.Message("You redeemed C4", ply);
                        break;
                    case "CoughSyrupButton":
                        player.inventory.forceAddItem(new Item(404, true), true);
                        SteamPlayer ply1 = Provider.clients.ToList().Find(x => x.player == player);
                        LobbyManager.Message("You redeemed Cough Syrup", ply1);
                        break;
                    case "KnifeButton":
                        player.inventory.forceAddItem(new Item(140, true), true);
                        SteamPlayer ply2 = Provider.clients.ToList().Find(x => x.player == player);
                        LobbyManager.Message("You redeemed Knife", ply2);
                        break;
                    case "LMGButton":
                        player.inventory.forceAddItem(new Item(126, true), true);
                        SteamPlayer ply3 = Provider.clients.ToList().Find(x => x.player == player);
                        LobbyManager.Message("You redeemed LMG", ply3);
                        break;
                    case "SupressedPistol":
                        player.inventory.forceAddItem(new Item(126, true), true);
                        SteamPlayer ply4 = Provider.clients.ToList().Find(x => x.player == player);
                        LobbyManager.Message("You redeemed LMG", ply4);
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

        public static async Task ClearStatusUIAsync(LobbyPlayer player)
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
    }
}
