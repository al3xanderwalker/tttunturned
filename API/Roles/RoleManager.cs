using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Steamworks;
using TTTUnturned.API.Lobby;
using TTTUnturned.API.Players;
using TTTUnturned.API.Round;
using TTTUnturned.API.Interface;
using SDG.Unturned;
using TTTUnturned.API.Core;

namespace TTTUnturned.API.Roles
{
    public class RoleManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.Log("RoleManager loaded");
        }

        public static List<TTTPlayer> GeneratePlayerRoles()
        {
            List<CSteamID> tickets = new List<CSteamID>();
            List<TTTPlayer> players = new List<TTTPlayer>();

            Provider.clients.ToList().ForEach(player => // SteamPlayer
            {
                tickets.Add(player.playerID.steamID);
                /*
                LobbyPlayer p = new LobbyPlayer(player.playerID.steamID, PlayerRole.NONE, PlayerRank.NONE, PlayerStatus.ALIVE);
                if (p.Rank == PlayerRank.NONE) tickets.Add(p.SteamID);
                if (p.Rank == PlayerRank.VIP)
                {
                    tickets.Add(p.SteamID);
                    tickets.Add(p.SteamID);
                }
                players.Add(p);
                */
            });

            System.Random rng = new System.Random();

            int terroistCount = (int)Math.Floor(Provider.clients.Count / 4.0);
            int detectiveCount = (int)Math.Floor(Provider.clients.Count / 8.0);
            if (terroistCount == 0) terroistCount = 1;
            if (detectiveCount == 0) detectiveCount = 1;

            for (var i = 0; i < terroistCount; i++)
            {
                CSteamID test = tickets[rng.Next(tickets.Count)];
                TTTPlayer terroist = new TTTPlayer(test, Role.TRAITOR, Rank.NONE, Status.ALIVE);
                players.Add(terroist);
                tickets.RemoveAll(x => x == test);
            }

            for (var i = 0; i < detectiveCount; i++)
            {
                CSteamID test = tickets[rng.Next(tickets.Count)];
                TTTPlayer detective = new TTTPlayer(test, Role.DETECTIVE, Rank.NONE, Status.ALIVE);
                players.Add(detective);
                tickets.RemoveAll(x => x == test);
            }

            tickets.ToList().ForEach(ticket =>
            {
                TTTPlayer innocent = new TTTPlayer(ticket, Role.INNOCENT, Rank.NONE, Status.ALIVE);
                players.Add(innocent);
                tickets.RemoveAll(x => x == ticket);
            });
            return players;
        }

        public static void TellRoles(List<TTTPlayer> players)
        {
            players.ForEach(async player =>
            {
                SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(player.SteamID);
                switch (player.Role) {
                    case Role.INNOCENT:
                        RoundManager.Broadcast($"You are a <color=lime>Innocent</color>", steamPlayer);
                        await InterfaceManager.SendUIEffectAsync(8497, 8490, player.SteamID, true);
                        await InterfaceManager.SendUIEffectTextAsync(8490, player.SteamID, true, "RoleValue", "INNOCENT");
                        break;
                    case Role.DETECTIVE:
                        RoundManager.Broadcast($"You are a <color=blue>Detective</color>", steamPlayer);
                        Level.ItemManager.AddItemAync(steamPlayer, 10);
                        await InterfaceManager.SendUIEffectAsync(8496, 8490, player.SteamID, true);
                        await InterfaceManager.SendUIEffectTextAsync(8490, player.SteamID, true, "RoleValue", "DETECTIVE");
                        break;
                    case Role.TRAITOR:
                        RoundManager.Broadcast($"You are a <color=red>Terrorist</color>", steamPlayer);
                        await InterfaceManager.SendUIEffectAsync(8499, 8490, player.SteamID, true);
                        await InterfaceManager.SendUIEffectTextAsync(8490, player.SteamID, true, "RoleValue", "TERRORIST");
                        break;
                }
            });
        }
    }
}
