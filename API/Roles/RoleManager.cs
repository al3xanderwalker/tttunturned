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

            for (var i = 0; i < traitorCount; i++)
            {
                CSteamID selectedTraitor = tickets[rng.Next(tickets.Count)];

                TTTPlayer tttPlayer = PlayerManager.GetTTTPlayer(selectedTraitor);
                tttPlayer.SetRole(PlayerRole.TRAITOR);
                tttPlayer.SendMessageUnsafe("You are a traitor");

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
    }
}