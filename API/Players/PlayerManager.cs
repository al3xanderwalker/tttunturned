using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using TTTUnturned.API.Roles;
using TTTUnturned.API.Round;
using UnityEngine;

namespace TTTUnturned.API.Players
{
    public class PlayerManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.Log("PlayerManager loaded");

            Provider.onEnemyConnected += OnEnemyConnected;
        }

        #region API
        public static TTTPlayer CreateTTTPlayer(CSteamID steamID, PlayerRole role, PlayerStatus status) => new TTTPlayer(steamID, role, status);

        public static TTTPlayer GetTTTPlayer(CSteamID steamID) => RoundManager.GetSession().Players.FirstOrDefault(p => p.SteamID == steamID);
        #endregion

        #region Events
        private void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            TTTPlayer.ClearInventoryUnsafe(steamPlayer);
            Player ply = steamPlayer.player;
            ply.life.serverModifyHealth(100.0f); // Change this
            ply.life.serverModifyStamina(100.0f);
        }
        #endregion
    }
}
