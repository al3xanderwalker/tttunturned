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

namespace TTTUnturned.API.Items.HealhStation
{
    public class HealthStation : BarricadeItem
    {
        public int TimeLeft { get; set; }
        public bool Expired { get; set; }

        public HealthStation(BarricadeRegion region, BarricadeDrop drop, int time)
        {
            Id = 2;
            Name = "item_barricade_healthstation";
            DisplayName = "HealthStation";
            ItemType = ItemType.BARRICADE;
            UnturnedItem = new SDG.Unturned.Item(1050, true);
            Asset = (ItemAsset)Assets.find(EAssetType.ITEM, 1050);
            Region = region;
            Drop = drop;
            Expired = false;
            TimeLeft = time;
            AsyncHelper.Schedule("HealthStationTick", HealthStationTick, 500);
        }

        #region Threading
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task HealthStationTick()
        {
            if (!Expired)
            {
                TimeLeft -= 500;

                HealPlayers();

                if (TimeLeft == 0) UnityThread.executeCoroutine(HealthStationExpiredCoroutine());
            }
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        #endregion

        #region Coroutines
        public static IEnumerator SendEffectLocation(ushort id, Vector3 position)
        {
            EffectManager.sendEffect(id, byte.MaxValue, byte.MaxValue, byte.MaxValue, position);

            yield return null;
        }

        private IEnumerator HealPlayer(Player player)
        {
            player.life.askHeal(5, false, false);
            yield return null;
        }

        private IEnumerator HealthStationExpiredCoroutine()
        {
            byte x;
            byte y;
            ushort plant;
            ushort index;
            BarricadeRegion region;

            if (!BarricadeManager.tryGetInfo(Drop.model.transform, out x, out y, out plant, out index, out region)) yield return null;

            BarricadeManager.destroyBarricade(region, x, y, plant, index);
            Expired = true;
            C4Manager.ActiveHealthStations.Remove(this);

            yield return null;
        }
        private async Task HealPlayers()
        {
            List<Player> result = new List<Player>();
            PlayerTool.getPlayersInRadius(Drop.model.position, 10f, result);
            result.ForEach(p =>
            {
                UnityThread.executeCoroutine(HealPlayer(p));
            });
        }
        #endregion
    }
}
