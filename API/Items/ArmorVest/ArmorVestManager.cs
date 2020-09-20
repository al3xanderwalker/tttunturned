using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerManager = TTTUnturned.API.Players.PlayerManager;
using TTTUnturned.API.Players;
using UnityEngine;

namespace TTTUnturned.API.Items.ArmorVest
{
    [HarmonyPatch(typeof(PlayerLife))]
    [HarmonyPatch("doDamage")]
    class DoDamagePatch
    {
        public static void Prefix(byte amount, Vector3 newRagdoll, EDeathCause newCause, ELimb newLimb, CSteamID newKiller, ref EPlayerKill kill, bool trackKill, ERagdollEffect newRagdollEffect, bool canCauseBleeding, PlayerLife __instance)
        {
            Player ply = __instance.channel.owner.player;
            if (ply is null) return;

            TTTPlayer player = PlayerManager.GetTTTPlayer(ply.channel.owner.playerID.steamID);
            if (player is null) return;

            if (player.Armor) amount = (byte) Math.Floor(amount * 0.70f);
        }
    }
}
