using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTUnturned.API.Players
{
    [HarmonyPatch(typeof(Provider))]
    [HarmonyPatch("handleValidateAuthTicketResponse")]
    class HandleValidateAuthTicketResponsePatch
    {
        public static void Prefix(ValidateAuthTicketResponse_t callback)
        {
            foreach(SteamPending pendingPlayer in Provider.pending)
            {
                pendingPlayer.skinItems = new int[0];
                pendingPlayer.packageSkins = new ulong[0];

                pendingPlayer.packageHat = 0UL;
                pendingPlayer.hatItem = 0;

                pendingPlayer.maskItem = 0;
                pendingPlayer.packageMask = 0UL;

                pendingPlayer.packageGlasses = 0UL;
                pendingPlayer.glassesItem = 0;

                pendingPlayer.shirtItem = 0;
                pendingPlayer.packageShirt = 0UL;

                pendingPlayer.vestItem = 0;
                pendingPlayer.packageVest = 0UL;

                pendingPlayer.packageBackpack = 0UL;
                pendingPlayer.backpackItem = 0;

                pendingPlayer.pantsItem = 0;
                pendingPlayer.packagePants = 0UL;
            }
        }
    }
}
