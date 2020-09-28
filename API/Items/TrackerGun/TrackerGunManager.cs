using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using PlayerManager = TTTUnturned.API.Players.PlayerManager;
using TTTUnturned.API.Players;
using TTTUnturned.Utils;
using TTTUnturned.API.Interface;
using UnityEngine;

namespace TTTUnturned.API.Items.TrackerGun
{
    public class TrackerGunManager : MonoBehaviour, IObjectComponent
    {
        private static Dictionary<CSteamID, long> TrackedPlayers;

        public void Awake()
        {
            CommandWindow.Log("TrackerGunManager loaded");

            TrackedPlayers = new Dictionary<CSteamID, long>();

            DamageTool.damagePlayerRequested += OnDamagePlayerRequested;
            AsyncHelper.Schedule("TrackerGunTick", TrackerGunTick, 500);
        }

        private void OnDamagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            Player killerPlayer = PlayerTool.getPlayer(parameters.killer);
            if (killerPlayer is null) return;
            if (killerPlayer.equipment.asset is null) return;
            if (killerPlayer.equipment.asset.id != 1447) return;

            CSteamID playerId = parameters.player.channel.owner.playerID.steamID;

            parameters.damage = 0;

            if (!TrackedPlayers.ContainsKey(playerId))
            {
                AddTrackedPlayer(playerId);
            }
            else
            {
                TrackedPlayers[playerId] = 30000;
            }
        }

        #region Threading
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task TrackerGunTick()
        {
            foreach (KeyValuePair<CSteamID, long> pair in TrackedPlayers.Keys.ToDictionary(_ => _, _ => TrackedPlayers[_]))
            {
                TrackedPlayers[pair.Key] -= 500;
                if (pair.Value % 5000 == 0)
                {
                    Player ply = PlayerTool.getPlayer(pair.Key);
                    InterfaceManager.SendEffectLocationUnsafe(107, ply.transform.position);
                }
                if(pair.Value == 0)
                {
                    RemoveTrackedPlayer(pair.Key);
                }
            }
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        #endregion


        //ItemJar item = base.player.inventory.getItem(b, 0);

        public static void AddTrackedPlayer(CSteamID steamID) => TrackedPlayers.Add(steamID, 30000);

        public static void RemoveTrackedPlayer(CSteamID steamID) => TrackedPlayers.Remove(steamID);

        public static void ClearTrackedPlayers() => TrackedPlayers.Clear();
    }
}
