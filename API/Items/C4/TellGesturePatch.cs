using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TTTUnturned.API.Items.C4
{
    [HarmonyPatch(typeof(PlayerAnimator))]
    [HarmonyPatch("tellGesture")]
    class TellGesturePatch
    {
        public static void Prefix(CSteamID steamID, byte id, PlayerAnimator __instance)
        {
            Player ply = __instance.channel.owner.player;
            if (ply is null) return;

            RaycastInfo traceResult = TraceRay(ply, 10f, RayMasks.BARRICADE_INTERACT);
            if (traceResult is null)
            {
                CommandWindow.Log("Result is nulll");
                return;
            }

            InteractableCharge charge = traceResult.transform.gameObject.GetComponent<InteractableCharge>();
            if (charge is null)
            {
                CommandWindow.Log("Charge is null");
                return;
            }

            byte x;
            byte y;
            ushort plant;
            ushort index;
            BarricadeRegion region;

            if (!BarricadeManager.tryGetInfo(charge.transform, out x, out y, out plant, out index, out region))
            {
                CommandWindow.Log("Couldnt get charge transform thing");
                return;
            }

            BarricadeManager.instance.channel.send("tellBarricadeOwnerAndGroup", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] 
            {
                x,
                y,
                plant,
                index,
                (ulong) ply.channel.owner.playerID.steamID,
                (ulong) 0
            });
            //CSteamID steamID, byte x, byte y, ushort plant, ushort index, ulong newOwner, ulong newGroup

        }
        public static RaycastInfo TraceRay(Player player, float distance, int masks)
        {
            return DamageTool.raycast(new Ray(player.look.aim.position, player.look.aim.forward), distance, masks);
        }
    }
}
