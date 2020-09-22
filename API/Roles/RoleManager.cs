using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Steamworks;
using TTTUnturned.API.Players;
using TTTUnturned.API.Round;
using SDG.Unturned;
using TTTUnturned.API.Core;
using PlayerManager = TTTUnturned.API.Players.PlayerManager;
using Random = System.Random;
using System.Threading.Tasks;
using TTTUnturned.API.Interface;
using System.Collections;
using TTTUnturned.Utils;

namespace TTTUnturned.API.Roles
{
    public class RoleManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.Log("RoleManager loaded");
        }

        #region API
        public static void GeneratePlayerRoles()
        {
            if (RoundManager.GetAllAlivePlayers().Count < Main.Config.MinimumPlayers)
            {
                RoundManager.StopRound();
                return;
            }

            Random rng = new Random();
            List<CSteamID> tickets = new List<CSteamID>();

            RoundManager.GetAllPlayers().ForEach(p =>
            {
                tickets.Add(p.SteamID);
            });

            int traitorCount = (int)Math.Floor(RoundManager.GetAllPlayers().Count / 4.0);
            int detectiveCount = (int)Math.Floor(RoundManager.GetAllPlayers().Count / 8.0);
            if (traitorCount == 0) traitorCount = 1;
            if (detectiveCount == 0) detectiveCount = 1;

            GroupInfo oldTraitorGroup = GroupManager.getGroupInfo((CSteamID)1);
            if (!(oldTraitorGroup is null))
            {
                UnityThread.executeCoroutine(DeleteTraitorGroup());
            }

            UnityThread.executeCoroutine(CreateTraitorGroup());

            for (var i = 0; i < traitorCount; i++)
            {
                CSteamID selectedTraitor = tickets[rng.Next(tickets.Count)];

                TTTPlayer tttPlayer = PlayerManager.GetTTTPlayer(selectedTraitor);
                if (tttPlayer is null) return;

                tttPlayer.SetRole(PlayerRole.TRAITOR);
                tttPlayer.SendMessageUnsafe("You are a traitor");

                UnityThread.executeCoroutine(SetSteamGroupEnumerator(tttPlayer.SteamID));

                InterfaceManager.SendUIEffectUnsafe(8499, 8490, tttPlayer.SteamID, true);
                InterfaceManager.SendUIEffectTextUnsafe(8490, tttPlayer.SteamID, true, "RoleValue", "TRAITOR");

                tickets.RemoveAll(x => x == selectedTraitor);
            }

            for (var i = 0; i < detectiveCount; i++)
            {
                CSteamID selectedDetective = tickets[rng.Next(tickets.Count)];

                TTTPlayer tttPlayer = PlayerManager.GetTTTPlayer(selectedDetective);
                tttPlayer.SetRole(PlayerRole.DETECTIVE);
                tttPlayer.SendMessageUnsafe("You are a detective");
                tttPlayer.AddItemUnsafe(10);

                InterfaceManager.SendUIEffectUnsafe(8496, 8490, tttPlayer.SteamID, true);
                InterfaceManager.SendUIEffectTextUnsafe(8490, tttPlayer.SteamID, true, "RoleValue", "DETECTIVE");

                tickets.RemoveAll(x => x == selectedDetective);
            }

            tickets.ToList().ForEach(steamID =>
            {
                TTTPlayer tttPlayer = PlayerManager.GetTTTPlayer(steamID);
                tttPlayer.SetRole(PlayerRole.INNOCENT);
                tttPlayer.SendMessageUnsafe("You are a innocent");

                InterfaceManager.SendUIEffectUnsafe(8497, 8490, tttPlayer.SteamID, true);
                InterfaceManager.SendUIEffectTextUnsafe(8490, tttPlayer.SteamID, true, "RoleValue", "INNOCENT");

                tickets.RemoveAll(x => x == steamID);
            });
        }
        #endregion

        #region Enumerators
        private static IEnumerator CreateTraitorGroup()
        {
            GroupManager.addGroup((CSteamID)1, "Traitors");
            yield return null;
        }

        private static IEnumerator DeleteTraitorGroup()
        {
            GroupManager.deleteGroup((CSteamID) 1);
            yield return null;
        }

        private static IEnumerator SetSteamGroupEnumerator(CSteamID steamID)
        {
            Player ply = PlayerTool.getPlayer(steamID);
            if (ply is null) yield return null;

            ply.quests.channel.send("tellSetGroup", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
            {
                    (CSteamID) 1,
                    (byte) EPlayerGroupRank.MEMBER
            });
            yield return null;
        }
        #endregion
    }
}