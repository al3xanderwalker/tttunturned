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
        /*
        public void Awake()
        {
            CommandWindow.Log("TrackerGunManager loaded");

            ChatManager.onChatted += OnChatted;
        }

        private void OnChatted(SteamPlayer player, EChatMode mode, ref Color chatted, ref bool isRich, string text, ref bool isVisible)
        {
            Player ply = PlayerTool.getPlayer(player.playerID.steamID);
            if (ply is null) return;
            CommandWindow.Log(ply.equipment.asset);
        }

        //ItemJar item = base.player.inventory.getItem(b, 0);
        */
    }
}
