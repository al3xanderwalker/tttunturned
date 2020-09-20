using SDG.Unturned;
using Steamworks;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TTTUnturned.API.Level;
using TTTUnturned.API.Roles;
using TTTUnturned.Utils;
using UnityEngine;

namespace TTTUnturned.API.Players
{
    public class TTTPlayer
    {
        public CSteamID SteamID;
        public PlayerRole Role;
        public PlayerStatus Status;
        public bool Armor;
        public int Credits;

        public TTTPlayer(CSteamID steamID, PlayerRole role, PlayerStatus status)
        {
            SteamID = steamID;
            Role = role;
            Status = status;
            Armor = false;
            Credits = 0;
        }

        #region API
        public void SetRole(PlayerRole role) => Role = role;

        public PlayerRole GetRole() => Role;

        public PlayerStatus GetStatus() => Status;

        public void SetStatus(PlayerStatus status) => Status = status;

        public int GetCredits() => Credits;

        public void AddCredits(int credits) => Credits += credits;

        public void RemoveCredits(int credits) => Credits -= credits;

        public void SetCredits(int credits) => Credits = credits;

        public void Revive()
        {
            Player ply = PlayerTool.getPlayer(SteamID);
            Spawn spawn = Main.Config.LobbySpawns.First();
            ply.life.channel.send("tellRevive", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
            {
                new Vector3(spawn.X, spawn.Y, spawn.Z),  // Location  
                (byte) 0 // yaw?
            });
            ply.life.sendRevive();
        }

        public void ReviveUnsafe()
        {
            UnityThread.executeCoroutine(ReviveEnumerator());
        }

        public void TeleportToLobby()
        {
            Player ply = PlayerTool.getPlayer(SteamID);
            Spawn spawn = Main.Config.LobbySpawns.First();
            ply.teleportToLocation(new Vector3(spawn.X, spawn.Y, spawn.Z), 0.0f);
        }

        public void TeleportToMap()
        {
            Player ply = PlayerTool.getPlayer(SteamID);
            Spawn spawn = Main.Config.Maps.First().Spawns.First();
            ply.teleportToLocation(new Vector3(spawn.X, spawn.Y, spawn.Z), 0.0f);
        }

        public void TeleportToMapUnsafe()
        {
            Spawn spawn = Main.Config.Maps.First().Spawns.First();
            UnityThread.executeCoroutine(TeleportToLocationEnumerator(new Vector3(spawn.X, spawn.Y, spawn.Z)));
        }

        public void SendMessage(string message)
        {
            ChatManager.serverSendMessage(message, Color.white, null, PlayerTool.getSteamPlayer(SteamID), EChatMode.SAY, null, true);
        }
        public void SendMessageUnsafe(string message)
        {
            UnityThread.executeCoroutine(SendMessageEnumerator(message));
        }

        public static void DisableHUDUnsafe(SteamPlayer player)
        {
            UnityThread.executeCoroutine(DisableHUDCoroutine(player));
        }

        public static void ClearInventoryUnsafe(SteamPlayer player)
        {
            UnityThread.executeCoroutine(ClearInventoryCoroutine(player));
        }

        public void AddItemUnsafe(ushort id)
        {
            UnityThread.executeCoroutine(AddItemCoroutine(id));
        }
        #endregion

        #region Enumerators
        private IEnumerator AddItemCoroutine(ushort id)
        {
            PlayerTool.getPlayer(SteamID).inventory.forceAddItem(new Item(id, true), true);

            yield return null;
        }

        private static IEnumerator ClearInventoryCoroutine(SteamPlayer player)
        {
            for (byte page = 0; page < 6; page++)
            {
                for (byte i = 0; i < player.player.inventory.items[page].getItemCount(); i++)
                {
                    ItemJar item = player.player.inventory.items[page].getItem(i);
                    player.player.inventory.removeItem(page, player.player.inventory.getIndex(page, item.x, item.y));
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

        private IEnumerator TeleportToLocationEnumerator(Vector3 location)
        {
            Player ply = PlayerTool.getPlayer(SteamID);
            ply.teleportToLocation(location, 0.0f);
            yield return null;
        }

        private IEnumerator SendMessageEnumerator(string message)
        {
            ChatManager.serverSendMessage(message, Color.white, null, PlayerTool.getSteamPlayer(SteamID), EChatMode.SAY, null, false);
            yield return null;
        }

        private IEnumerator ReviveEnumerator()
        {
            Player ply = PlayerTool.getPlayer(SteamID);
            Spawn spawn = Main.Config.LobbySpawns.First();
            ply.life.channel.send("tellRevive", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] 
            {
                new Vector3(spawn.X, spawn.Y, spawn.Z),  // Location  
                (byte) 0 // yaw?
            });
            ply.life.sendRevive();
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
