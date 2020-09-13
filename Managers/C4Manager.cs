using SDG.Unturned;
using Steamworks;
using TTTUnturned.Models;
using UnityEngine;
using TTTUnturned.Utils;
using System.Threading.Tasks;

namespace TTTUnturned.Managers
{
    public class C4Manager : MonoBehaviour
    {
        public void Awake()
        {
            CommandWindow.Log("C4Manager loaded");

            BarricadeManager.onBarricadeSpawned += OnBarricadeSpawned;
            BarricadeManager.onDamageBarricadeRequested += OnDamageBarricadeRequested;
        }
        public void OnDamageBarricadeRequested(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            CommandWindow.Log($"Damange origin: {damageOrigin}");
            // if (barricadeTransform.name.ToString() == "1241") shouldAllow = false;
        }
        public void OnBarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
        {
            if (drop.asset.id != 1241) return;
            CommandWindow.Log(drop.asset.id);

            Task.Run(() =>
            {
                C4 C4 = new C4(region, drop, 10);
            });
        }
    }
}
