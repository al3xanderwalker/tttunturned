using SDG.Unturned;
using Steamworks;
using TTTUnturned.API.Core;
using UnityEngine;

namespace TTTUnturned.API.Items.C4.C4Manager
{
    public class C4Manager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.Log("C4Manager loaded");

            BarricadeManager.onBarricadeSpawned += OnBarricadeSpawned;
            BarricadeManager.onSalvageBarricadeRequested += OnSalvageBarricadeRequested;
        }

        public void OnSalvageBarricadeRequested(CSteamID steamID, byte x, byte y, ushort plant, ushort index, ref bool shouldAllow)
        {
            shouldAllow = false;
        }

        public void OnBarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
        {
            if (drop.asset.id != 1241) return;

            SpawnC4Barricade(region, drop, 10);
        }

        static public void SpawnC4Barricade(BarricadeRegion region, BarricadeDrop drop, int time) => new C4(region, drop, time);
    }
}
