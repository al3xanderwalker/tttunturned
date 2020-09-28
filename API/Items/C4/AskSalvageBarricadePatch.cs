using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Interface;
using TTTUnturned.Utils;

namespace TTTUnturned.API.Items.C4
{
    [HarmonyPatch(typeof(BarricadeManager))]
    [HarmonyPatch("askSalvageBarricade")]
    class AskSalvageBarricadePatch
    {
        public static bool Prefix(CSteamID steamID, byte x, byte y, ushort plant, ushort index, BarricadeManager __instance)
        {
            BarricadeRegion region;
            BarricadeManager.tryGetRegion(x, y, plant, out region);
            uint UID = region.barricades[index].instanceID;
            C4Manager.ActiveC4.ToList().ForEach(c4 =>
            {
                if(c4.Drop.instanceID == UID)
                {
                    c4.Defused = true;
                    InterfaceManager.SendEffectLocationUnsafe(61, c4.Drop.model.position);
                    BarricadeManager.destroyBarricade(region, x, y, plant, index);
                    C4Manager.ActiveC4.Remove(c4);
                }
            });
            return false;
        }
    }
}
