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

        public void Awake()
        {
            CommandWindow.Log("TrackerGunManager loaded");
            
            DamageTool.damagePlayerRequested += OnDamagePlayerRequested;
        }
        
        private void OnDamagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (parameters.player is null) return;
            if (parameters.player.equipment.asset is null) return;
            if (parameters.player.equipment.asset.id != 1447) return;
            SpawnTrackerGun(parameters.player.channel.owner);
        }


        //ItemJar item = base.player.inventory.getItem(b, 0);

        static public TrackerGun SpawnTrackerGun(SteamPlayer player) => new TrackerGun(player);
    }
}
