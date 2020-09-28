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
        public TrackerGun()
        {
            Id = 3;
            Name = "item_weapon_prototype";
            DisplayName = "Prototype";
            ItemType = ItemType.WEAPON;
            UnturnedItem = new SDG.Unturned.Item(1447, true);
            Asset = (ItemAsset)Assets.find(EAssetType.ITEM, 1447);
        }
    }
}
