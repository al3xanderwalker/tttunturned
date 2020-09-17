using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using TTTUnturned.API.Interface;
using TTTUnturned.API.Level;
using TTTUnturned.API.Lobby;
using TTTUnturned.API.Roles;
using TTTUnturned.API.Round;
using TTTUnturned.Utils;
using UnityEngine;

namespace TTTUnturned.API.Players
{
    public class PlayerManager : MonoBehaviour, IObjectComponent
    {

        public void Awake()
        {
            CommandWindow.Log("PlayerManager loaded");

            DamageTool.damagePlayerRequested += OnDamageRequested;
            PlayerLife.onPlayerDied += OnPlayerDied;
            Provider.onEnemyConnected += OnEnemyConnected;
        }
        #region Events
        private void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            ClearInventoryAsync(steamPlayer);
        }

        private void OnDamageRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            // 30% damage reduction
            if (GetTTTPlayer(parameters.player.channel.owner.playerID.steamID).Armor) parameters.damage = parameters.damage * 0.70f;

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

            if (player.Role == Role.TRAITOR) RoundManager.GetAlive(Role.DETECTIVE).ForEach(detective =>
            {
                detective.Credits += 1;
                RoundManager.Broadcast("You have gained 1 credit", PlayerTool.getSteamPlayer(detective.SteamID));
            });

            TTTPlayer killer = GetTTTPlayer(instigator);
            if(killer != null)
            {
                if (player.Role == Role.DETECTIVE && killer.Role == Role.TRAITOR) 
                { 
                    killer.Credits += 1;
                    RoundManager.Broadcast("You have gained 1 credit", PlayerTool.getSteamPlayer(killer.SteamID));
                }
                if(player.Role != Role.TRAITOR && killer.Role == Role.TRAITOR)
                {
                    int totalCount = RoundManager.GetPlayers().Count;
                    int alive = RoundManager.GetPlayers().FindAll(t => t.Status == Status.ALIVE).Count;
                    if ( totalCount / (alive + 1) > 75 && totalCount / alive <= 75 || totalCount / (alive + 1) > 50 && totalCount / alive <= 50 || totalCount / (alive + 1) > 25 && totalCount / alive <= 25)
                    {
                        RoundManager.GetAlive(Role.TRAITOR).ForEach(traitor =>
                        {
                            traitor.Credits += 1;
                            RoundManager.Broadcast("You have gained 1 credit", PlayerTool.getSteamPlayer(traitor.SteamID));
                        });
                    }
                }
            }
            RoundManager.CheckWin();
        }
        #endregion
        #region Functions
        public static TTTPlayer GetTTTPlayer(CSteamID steamID) => RoundManager.GetPlayers().FirstOrDefault(p => p.SteamID == steamID);

        public static async Task ClearInventoryAsync(SteamPlayer player)
        {
            UnityThread.executeCoroutine(ClearInventoryCoroutine(player));
        }

        public static async Task TeleportToLocationAsync(SteamPlayer steamPlayer, Vector3 location)
        {
            UnityThread.executeCoroutine(TeleportToLocationCoroutine(steamPlayer, location));
        }

        public static async Task DisableHUDAsync(SteamPlayer player)
        {
            UnityThread.executeCoroutine(DisableHUDCoroutine(player));
        }

        public static Vector3 GetRandomSpawn(List<Spawn> spawns)
        {
            System.Random rng = new System.Random();
            int t = rng.Next(spawns.Count);
            return new Vector3(spawns[t].X, spawns[t].Y, spawns[t].Z);
        }
        #endregion
        #region Couroutines
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
            System.Action removeUnequipped = () =>
            {
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

        private static IEnumerator TeleportToLocationCoroutine(SteamPlayer steamPlayer, Vector3 location)
        {
            steamPlayer.player.teleportToLocation(location, 0f);

            yield return null;
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
        #endregion
    }
}
