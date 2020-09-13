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
        private static Dictionary<CSteamID, long> keyCooldowns;

        public void Awake()
        {
            CommandWindow.Log("PlayerManager loaded");

            keyCooldowns = new Dictionary<CSteamID, long>();

            Provider.onEnemyConnected += OnEnemyConnected;
            Provider.onEnemyDisconnected += OnEnemyDisconnected;
            DamageTool.damagePlayerRequested += OnDamageRequested;
            PlayerLife.onPlayerDied += OnPlayerDied;
            PlayerInput.onPluginKeyTick += OnPluginKeyTick;
            // UseableThrowable.onThrowableSpawned += OnThrowableSpawned;
        }
        /* Me experimenting with an idea but i still dont have a fucking clue how raycasting works lmao
        private void OnThrowableSpawned(UseableThrowable useable, GameObject throwable)
        {
            CommandWindow.Log("Throwable lobbed");
            RaycastInfo raycast = DamageTool.raycast(new Ray(useable.player.look.aim.position, useable.player.look.aim.forward), 500f, RayMasks.GROUND);
            if (raycast.transform is null) return;
            Transform target = raycast.collider?.transform;
            if (target is null) return;
            CommandWindow.Log(target);
            CommandWindow.Log($"name({target.name.ToUpper()})");
            if(target.name.ToUpper() == "GROUND")
            {
                CommandWindow.Log("teleported");
                EffectManager.sendEffect(120, 30, target.position);
                useable.player.teleportToLocation(target.position,0f);
                
            }
            if (raycast.vehicle != null)
            {
                CommandWindow.Log($"vehicle id: {raycast.vehicle.name}");
            }
            else
            {
                CommandWindow.Log(raycast.transform.position);
                
            }
        }
        */
        private void OnPluginKeyTick(Player player, uint simulation, byte key, bool state)
        {
            if (!state || key != 0) return;

            LobbyPlayer lobbyPlayer = LobbyManager.GetLobbyPlayer(player.channel.owner.playerID.steamID);
            if (lobbyPlayer is null) return;

            if (lobbyPlayer.Status == PlayerStatus.DEAD) return;
            if (LobbyManager.GetLobby().State != LobbyState.LIVE) return;


            if (keyCooldowns.ContainsKey(player.channel.owner.playerID.steamID))
            {
                long lastPressed = keyCooldowns[player.channel.owner.playerID.steamID];
                // 1 second key cooldown on menu
                if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastPressed < 1000) return;

                keyCooldowns.Remove(player.channel.owner.playerID.steamID);
            }

            keyCooldowns.Add(player.channel.owner.playerID.steamID, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            if (lobbyPlayer.Role == PlayerRole.TERRORIST )
            {
                if (lobbyPlayer.UIToggled)
                {
                    lobbyPlayer.UIToggled = false;
                    UIManager.ClearUIEffectAsync(8501, lobbyPlayer.SteamID);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, false);
                }
                else
                {
                    lobbyPlayer.UIToggled = true;
                    UIManager.SendUIEffectAsync(8501, 8470, lobbyPlayer.SteamID, true);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, true);
                }
            }
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
            Lobby Lobby = LobbyManager.GetLobby();
            if (Lobby.State != LobbyState.LIVE)
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
