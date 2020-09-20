using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using UnityEngine;

namespace TTTUnturned.API.Players
{
    public class DeathboxManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.Log("Deathbox loaded");
        }
    }

    [HarmonyPatch(typeof(PlayerLife))]
    [HarmonyPatch("doDamage")]
    class DoDamagePatch
    {
        public static void Prefix(byte amount, Vector3 newRagdoll, EDeathCause newCause, ELimb newLimb, CSteamID newKiller, ref EPlayerKill kill, bool trackKill, ERagdollEffect newRagdollEffect, bool canCauseBleeding, PlayerLife __instance)
        {
            Player ply = __instance.channel.owner.player;
            if (ply is null) return;

            if (amount >= ply.life.health)
            {
                Transform deathboxTransform = BarricadeManager.dropBarricade(new Barricade(366),ply.transform, ply.transform.position, 0f, 0f, 0f, (ulong) ply.channel.owner.playerID.steamID, 0UL);
                byte x;
                byte y;
                ushort plant;
                ushort index;
                BarricadeRegion region;

                if (!BarricadeManager.tryGetInfo(deathboxTransform, out x, out y, out plant, out index, out region)) return;

                InteractableStorage storage = deathboxTransform.GetComponent<InteractableStorage>();
                storage.items.resize(10, 10);

                for (byte page = 0; page < 6; page++)
                {
                    for (byte i = 0; i < ply.inventory.items[page].getItemCount(); i++)
                    {
                        ItemJar item = ply.inventory.items[page].getItem(i);
                        storage.items.tryAddItem(item.item);
                    }
                }
            }
        }
    }
}
