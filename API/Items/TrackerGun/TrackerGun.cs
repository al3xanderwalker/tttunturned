using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.Utils;
using TTTUnturned.API.Interface;
using UnityEngine;

namespace TTTUnturned.API.Items.TrackerGun
{
    public class TrackerGun : WeaponItem
    {
        public int TimeLeft { get; set; }
        public bool Expired { get; set; }
        public SteamPlayer SPlayer { get; set; }

        public TrackerGun(SteamPlayer player)
        {
            Id = 3;
            Name = "item_weapon_prototype";
            DisplayName = "Prototype";
            ItemType = ItemType.WEAPON;
            UnturnedItem = new SDG.Unturned.Item(1447, true);
            Asset = (ItemAsset)Assets.find(EAssetType.ITEM, 1447);
            SPlayer = player;
            TimeLeft = 30;
            AsyncHelper.Schedule("TrackerGunTick", TrackerGunTick, 1000);
            InterfaceManager.SendEffectLocation(107, player.player.transform.position);
        }
        #region Threading
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task TrackerGunTick()
        {
            if (!Expired)
            {
                TimeLeft -= 1;

                if (TimeLeft % 5 == 0) { 
                    InterfaceManager.SendEffectLocation(107, SPlayer.player.transform.position);
                    CommandWindow.Log(TimeLeft);
                }
                if (TimeLeft == 0)
                {
                    Expired = true;
                }
            }
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        #endregion
    }
}
