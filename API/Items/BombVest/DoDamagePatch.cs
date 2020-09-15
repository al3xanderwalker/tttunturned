using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TTTUnturned.API.Items.BombVest
{
    [HarmonyPatch(typeof(PlayerLife))]
    [HarmonyPatch("doDamage")]
    class DoDamagePatch
    {
        public static void Prefix(byte amount, Vector3 newRagdoll, EDeathCause newCause, ELimb newLimb, CSteamID newKiller, ref EPlayerKill kill, bool trackKill, ERagdollEffect newRagdollEffect, bool canCauseBleeding, PlayerLife __instance)
        {
            Player ply = __instance.channel.owner.player;
            if (ply is null) return;

            if (amount >= ply.life.health && ply.clothing.vest == 1013)
            {
                ply.clothing.askWearVest(0, 0, new byte[0], true);

                ExplosionParameters explodParams = new ExplosionParameters(ply.transform.position, 10f, EDeathCause.KILL, CSteamID.Nil);
                explodParams.penetrateBuildables = true;
                explodParams.playerDamage = 150;
                explodParams.damageRadius = 32;
                explodParams.barricadeDamage = 1000;
                List<EPlayerKill> deadPlayers = new List<EPlayerKill>();
                EffectManager.sendEffect(45, byte.MaxValue, byte.MaxValue, byte.MaxValue, ply.transform.position);
                DamageTool.explode(explodParams, out deadPlayers);
            }
        }
    }
}
