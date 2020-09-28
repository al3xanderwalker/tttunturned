using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTTUnturned.API.Players;
using System.Threading.Tasks;
using TTTUnturned.API.Roles;
using PlayerManager = TTTUnturned.API.Players.PlayerManager;
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
            TTTPlayer tttplayer = PlayerManager.GetTTTPlayer(ply.channel.owner.playerID.steamID);
            
            if (tttplayer is null) return;
            if (tttplayer.Role == PlayerRole.TRAITOR) return;

            RaycastInfo traceResult = TraceRay(ply, 10f, RayMasks.BARRICADE_INTERACT);
            if (traceResult is null) return;
            if (traceResult.transform is null)
            {
                return;
            }

            InteractableCharge charge = traceResult.transform.gameObject.GetComponent<InteractableCharge>();
            if (charge is null)
            {
                return;
            }

            byte x;
            byte y;
            ushort plant;
            ushort index;
            BarricadeRegion region;

            if (!BarricadeManager.tryGetInfo(charge.transform, out x, out y, out plant, out index, out region))
            {
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

            if(PlayerManager.GetTTTPlayer(__instance.channel.owner.playerID.steamID).Defuser)
            {
                __instance.channel.owner.player.interact.sendSalvageTimeOverride(5f);
            }
            else
            {
                __instance.channel.owner.player.interact.sendSalvageTimeOverride(10f);
            }
            //CSteamID steamID, byte x, byte y, ushort plant, ushort index, ulong newOwner, ulong newGroup

        }
        public static RaycastInfo TraceRay(Player player, float distance, int masks)
        {
            return DamageTool.raycast(new Ray(player.look.aim.position, player.look.aim.forward), distance, masks);
        }
    }
}
