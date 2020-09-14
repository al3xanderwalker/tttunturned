﻿using SDG.Unturned;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTTUnturned.Utils;
using Item = SDG.Unturned.Item;

namespace TTTUnturned.API.Items.C4
{

    public class C4 : BarricadeItem
    {
        public bool Defused { get; set; }
        public int TimeLeft { get; set; }

        public C4(BarricadeRegion region, BarricadeDrop drop, int time)
        {
            Id = 1;
            Name = "item_barricade_c4";
            DisplayName = "C4";
            ItemType = ItemType.BARRICADE;
            UnturnedItem = new SDG.Unturned.Item(1241, true);
            Asset = (ItemAsset)Assets.find(EAssetType.ITEM, 1241);
            Region = region;
            Drop = drop;
            Defused = false;
            TimeLeft = time;
            AsyncHelper.Schedule("C4Tick", C4Tick, 1000);
        }

        private async Task C4Tick()
        {
            TimeLeft--;
            if (TimeLeft == 0)
            {
                UnityThread.executeCoroutine(C4DetonateCoroutine());
            }
        }
        private IEnumerator C4DetonateCoroutine()
        {
            EffectManager.sendEffect(45, byte.MaxValue, byte.MaxValue, byte.MaxValue, Drop.model.position);
            ExplosionParameters parameters = new ExplosionParameters(Drop.model.position, 150f, EDeathCause.KILL, CSteamID.Nil);
            parameters.penetrateBuildables = true;
            parameters.playerDamage = 150;
            parameters.damageRadius = 64;
            parameters.barricadeDamage = 1000;
            List<EPlayerKill> deadPlayers = new List<EPlayerKill>();
            DamageTool.explode(parameters, out deadPlayers);
            // Region.destroy(); // DESTROYS IT BUT doesnt delete the in game model
            byte x;
            byte y;
            ushort plant;
            ushort index;
            BarricadeRegion region;

            if (!BarricadeManager.tryGetInfo(Drop.model.transform, out x, out y, out plant, out index, out region)) yield return null;

            BarricadeManager.destroyBarricade(region, x, y, plant, index);

            yield return null;
        }

    }
}
