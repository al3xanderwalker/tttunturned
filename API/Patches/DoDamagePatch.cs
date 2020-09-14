using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace TTTUnturned.API.Patches
{

    [HarmonyPatch(typeof(PlayerLife))]
    [HarmonyPatch("doDamage")]
    class DoDamagePatch
    {
        public static void Postfix(byte amount, Vector3 newRagdoll, EDeathCause newCause, ELimb newLimb, CSteamID newKiller, ref EPlayerKill kill, bool trackKill = false, ERagdollEffect newRagdollEffect = ERagdollEffect.NONE, bool canCauseBleeding = true)
        {
            CommandWindow.Log(amount);
        }
    }
}
