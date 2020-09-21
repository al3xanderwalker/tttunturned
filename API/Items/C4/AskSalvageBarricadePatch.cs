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
            C4Manager.ActiveC4.ToList().ForEach(c4 =>
            {
                CommandWindow.Log(c4.TimeLeft);
                byte itemX;
                byte itemY;
                ushort itemPlant;
                ushort itemIndex;
                BarricadeRegion region;

                if (!BarricadeManager.tryGetInfo(c4.Drop.model.transform, out itemX, out itemY, out itemPlant, out itemIndex, out region)) return;

                if (itemX == x && itemY == y)
                {
                    c4.Defused = true;
                    UnityThread.executeCoroutine(C4.SendEffectLocation(61, c4.Drop.model.position));
                    BarricadeManager.destroyBarricade(region, itemX, itemY, itemPlant, itemIndex);
                    C4Manager.ActiveC4.Remove(c4);
                }
            });

            return false;
        }
    }
}
