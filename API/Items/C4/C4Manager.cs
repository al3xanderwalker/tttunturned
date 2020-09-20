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
                CommandWindow.Log(c4.TimeLeft);
                byte itemX;
                byte itemY;
                ushort itemPlant;
                ushort itemIndex;
                BarricadeRegion region;

                BarricadeManager.tryGetInfo(c4.Drop.model.transform, out itemX, out itemY, out itemPlant, out itemIndex, out region);

                if (itemX == x && itemY == y)
                {
                    c4.Defused = true;
                    UnityThread.executeCoroutine(C4.SendEffectLocation(61, c4.Drop.model.position));
                    InterfaceManager.SendBannerMessage(steamID, 8494, "Bomb Defused", 3000, true);
                }

            });

            shouldAllow = false;
        }

        public void OnBarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
        {
            if (drop.asset.id != 1241) return;

            ActiveC4.Add(SpawnC4Barricade(region, drop, 20000));
        }

        static public C4 SpawnC4Barricade(BarricadeRegion region, BarricadeDrop drop, int time) => new C4(region, drop, time);
    }
}
