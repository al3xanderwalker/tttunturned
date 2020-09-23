using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.Utils;
using UnityEngine;
using TTTUnturned.API.Interface;

namespace TTTUnturned.API.Items.C4
{
    public class C4 : BarricadeItem
    {
        public bool Defused { get; set; }
        public int TimeLeft { get; set; }
        public int Length { get; set; }

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
            Length = time;
            TimeLeft = time;
            AsyncHelper.Schedule("C4Tick", C4Tick, 500);
        }

        #region Threading
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task C4Tick()
        {
            if (!Defused)
            {
                TimeLeft -= 500;

                beepCheck();

                if (TimeLeft == 0) UnityThread.executeCoroutine(C4DetonateCoroutine());
            }
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        #endregion

        #region Coroutines

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

            byte x;
            byte y;
            ushort plant;
            ushort index;
            BarricadeRegion region;

            if (!BarricadeManager.tryGetInfo(Drop.model.transform, out x, out y, out plant, out index, out region)) yield return null;

            BarricadeManager.destroyBarricade(region, x, y, plant, index);
            Defused = true; // quick fix to prevent time ticking
            C4Manager.ActiveC4.Remove(this);

            yield return null;
        }
        #endregion

        #region API
        public void beepCheck()
        {
            if (TimeLeft > Length / 2 && TimeLeft % 4000 == 0) Beep();

            if (TimeLeft < Length / 2 && TimeLeft > Length / 4 && TimeLeft % 2000 == 0) Beep();

            if (TimeLeft < Length / 4 && TimeLeft > Length / 8 && TimeLeft % 1000 == 0) Beep();

            if (TimeLeft < Length / 8 && TimeLeft % 500 == 0) Beep();
        }

        private void Beep()
        {
            InterfaceManager.SendEffectLocation(56, Drop.model.position);
        }
        #endregion
    }
}
