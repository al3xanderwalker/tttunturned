using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using SDG.Unturned;
using TTTUnturned.Utils;
using System.Collections;
using UnityEngine;
using TTTUnturned.API.Level;
using TTTUnturned.API.Lobby;
using TTTUnturned.API.Round;
using TTTUnturned.API.Interface;
using TTTUnturned.API.Roles;
using TTTUnturned.API.Core;
using System.Linq;

namespace TTTUnturned.API.Players
{
    public class PlayerManager : MonoBehaviour, IObjectComponent
    {
        private static Dictionary<CSteamID, long> keyCooldowns;

        public void Awake()
        {
            CommandWindow.Log("PlayerManager loaded");

            keyCooldowns = new Dictionary<CSteamID, long>();

            DamageTool.damagePlayerRequested += OnDamageRequested;
            PlayerLife.onPlayerDied += OnPlayerDied;
            PlayerInput.onPluginKeyTick += OnPluginKeyTick;
            Provider.onEnemyConnected += OnEnemyConnected;
            // UseableThrowable.onThrowableSpawned += OnThrowableSpawned;
        }

        private void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            InterfaceManager.DisableExtraHUD(steamPlayer.playerID.steamID);
        }

        public static TTTPlayer GetTTTPlayer(CSteamID steamID) => RoundManager.GetPlayers().FirstOrDefault(p => p.SteamID == steamID);

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

            if (tttPlayer.Role == Roles.Role.TRAITOR )
            {
                if (tttPlayer.UIToggled)
                {
                    tttPlayer.UIToggled = false;
                    InterfaceManager.ClearUIEffectAsync(8501, tttPlayer.SteamID);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, false);
                }
                else
                {
                    tttPlayer.UIToggled = true;
                    InterfaceManager.SendUIEffectAsync(8501, 8470, tttPlayer.SteamID, true);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, true);
                }
            }
        }

        #region Events
        private void OnDamageRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            parameters.respectArmor = true;
            parameters.applyGlobalArmorMultiplier = true;
            if (parameters.damage >= parameters.player.life.health)
            {
                if(parameters.player.clothing.vest.ToString() == "1013")
                {
                    parameters.player.clothing.askWearVest(0, 0, new byte[0], true); // Doesnt work and needs fixing
                    ExplosionParameters explodParams = new ExplosionParameters(parameters.player.transform.position, 10f, EDeathCause.KILL, CSteamID.Nil);
                    explodParams.penetrateBuildables = true;
                    explodParams.playerDamage = 10;
                    explodParams.damageRadius = 32;
                    explodParams.barricadeDamage = 1000;
                    List<EPlayerKill> deadPlayers = new List<EPlayerKill>();
                    EffectManager.sendEffect(45, byte.MaxValue, byte.MaxValue, byte.MaxValue, parameters.player.transform.position);
                    DamageTool.explode(explodParams, out deadPlayers);
                }
            }

            if (RoundManager.GetRoundSessionState() != RoundState.LIVE)
            {
                shouldAllow = false;
            }
        }

        private void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            sender.channel.send("tellRevive", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] // thanks alien man
            {
                GetRandomSpawn(Main.Config.LobbySpawns),
                (byte) 0
            });
            sender.sendRevive();

            TTTPlayer player = GetTTTPlayer(sender.channel.owner.playerID.steamID);
            if (player is null || player.Status == Status.DEAD) return;

            player.Status = Status.DEAD;
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
            System.Action removeUnequipped = () => {
                for (byte i = 0; i < player.player.inventory.getItemCount(2); i++)
                {
                    player.player.inventory.removeItem(2, 0);
                }
            };
            player.player.clothing.askWearBackpack(0, 0, new byte[0], true);
            removeUnequipped();
            player.player.clothing.askWearGlasses(0, 0, new byte[0], true);
            removeUnequipped();
            player.player.clothing.askWearHat(0, 0, new byte[0], true);
            removeUnequipped();
            player.player.clothing.askWearPants(0, 0, new byte[0], true);
            removeUnequipped();
            player.player.clothing.askWearMask(0, 0, new byte[0], true);
            removeUnequipped();
            player.player.clothing.askWearShirt(0, 0, new byte[0], true);
            removeUnequipped();
            player.player.clothing.askWearVest(0, 0, new byte[0], true);
            removeUnequipped();
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
