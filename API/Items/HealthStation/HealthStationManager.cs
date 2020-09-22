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

namespace TTTUnturned.API.Items.HealhStation
{
    public class C4Manager : MonoBehaviour, IObjectComponent
    {
        public static List<HealthStation> ActiveHealthStations;

        public void Awake()
        {
            CommandWindow.Log("HealthStation Manager loaded");

            ActiveHealthStations = new List<HealthStation>();


            BarricadeManager.onBarricadeSpawned += OnBarricadeSpawned;
            BarricadeManager.onDamageBarricadeRequested += OnDamageBarricadeRequest;
        }

        public void OnBarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
        {
            if (drop.asset.id != 1050) return;
            
            ActiveHealthStations.Add(SpawnHealthStationBarricade(region, drop, 20000));
        }

        private void OnDamageBarricadeRequest(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            shouldAllow = false;
        }

        static public HealthStation SpawnHealthStationBarricade(BarricadeRegion region, BarricadeDrop drop, int time) => new HealthStation(region, drop, time);
    }
}
