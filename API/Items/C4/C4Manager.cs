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
        public static List<C4> ActiveC4;

        public void Awake()
        {
            CommandWindow.Log("C4Manager loaded");

            ActiveC4 = new List<C4>();


            BarricadeManager.onBarricadeSpawned += OnBarricadeSpawned;
            BarricadeManager.onDamageBarricadeRequested += OnDamageBarricadeRequest;
        }

        public void OnBarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
        {
            if (drop.asset.id != 1241) return;

            ActiveC4.Add(SpawnC4Barricade(region, drop, 30000));
        }

        private void OnDamageBarricadeRequest(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            shouldAllow = false;
        }

        static public C4 SpawnC4Barricade(BarricadeRegion region, BarricadeDrop drop, int time) => new C4(region, drop, time);
    }
}
