using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using UnityEngine;

namespace TTTUnturned.API.Items.C4
{
    public class C4Manager : MonoBehaviour, IObjectComponent
    {
        private List<C4> ActiveC4;

        public void Awake()
        {
            CommandWindow.Log("C4Manager loaded");

            ActiveC4 = new List<C4>();

            BarricadeManager.onBarricadeSpawned += OnBarricadeSpawned;
            BarricadeManager.onSalvageBarricadeRequested += OnSalvageBarricadeRequested;
        }

        public void OnSalvageBarricadeRequested(CSteamID steamID, byte x, byte y, ushort plant, ushort index, ref bool shouldAllow)
        {
            ActiveC4.ForEach(c4 =>
            {
                byte itemX;
                byte itemY;
                ushort itemPlant;
                ushort itemIndex;
                BarricadeRegion region;

                if (!BarricadeManager.tryGetInfo(c4.Drop.model.transform, out itemX, out itemY, out itemPlant, out itemIndex, out region)) return;

                if (itemX == x && itemY == y)
                {
                    CommandWindow.Log("C4 Defused");
                    c4.Defused = true;
                }

            });

            shouldAllow = false;
        }

        public void OnBarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
        {
            if (drop.asset.id != 1241) return;

            ActiveC4.Add(SpawnC4Barricade(region, drop, 10));
        }

        static public C4 SpawnC4Barricade(BarricadeRegion region, BarricadeDrop drop, int time) => new C4(region, drop, time);
    }
}
