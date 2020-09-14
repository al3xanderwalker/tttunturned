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

namespace TTTUnturned.API.Roles
{
    public class RoleManager : MonoBehaviour
    {
        public void Awake()
        {
            CommandWindow.Log("RoleManager loaded");
        }

        public static List<LobbyPlayer> GeneratePlayerRoles()
        {
            List<CSteamID> tickets = new List<CSteamID>();
            List<LobbyPlayer> players = new List<LobbyPlayer>();

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
                LobbyPlayer terroist = new LobbyPlayer(test, PlayerRole.TERRORIST, PlayerRank.NONE, PlayerStatus.ALIVE);
                players.Add(terroist);
                tickets.RemoveAll(x => x == test);
            }

            for (var i = 0; i < detectiveCount; i++)
            {
                CSteamID test = tickets[rng.Next(tickets.Count)];
                LobbyPlayer detective = new LobbyPlayer(test, PlayerRole.DETECTIVE, PlayerRank.NONE, PlayerStatus.ALIVE);
                players.Add(detective);
                tickets.RemoveAll(x => x == test);
            }

            tickets.ToList().ForEach(ticket =>
            {
                LobbyPlayer innocent = new LobbyPlayer(ticket, PlayerRole.INNOCENT, PlayerRank.NONE, PlayerStatus.ALIVE);
                players.Add(innocent);
                tickets.RemoveAll(x => x == ticket);
            });
            return players;
        }

        public static void TellRoles(LobbySession lobby)
        {
            lobby.Players.ForEach(async player =>
            {
                SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(player.SteamID);
                switch (player.Role) {
                    case PlayerRole.INNOCENT:
                        LobbyManager.Message($"You are a <color=lime>Innocent</color>", steamPlayer);
                        await InterfaceManager.SendUIEffectAsync(8497, 8490, player.SteamID, true);
                        await InterfaceManager.SendUIEffectTextAsync(8490, player.SteamID, true, "RoleValue", "INNOCENT");
                        break;
                    case PlayerRole.DETECTIVE:
                        LobbyManager.Message($"You are a <color=blue>Detective</color>", steamPlayer);
                        Level.ItemManager.AddItemAync(steamPlayer, 10);
                        CommandWindow.Log("Gave vest");
                        await InterfaceManager.SendUIEffectAsync(8496, 8490, player.SteamID, true);
                        await InterfaceManager.SendUIEffectTextAsync(8490, player.SteamID, true, "RoleValue", "DETECTIVE");
                        break;
                    case PlayerRole.TERRORIST:
                        LobbyManager.Message($"You are a <color=red>Terrorist</color>", steamPlayer);
                        await InterfaceManager.SendUIEffectAsync(8499, 8490, player.SteamID, true);
                        await InterfaceManager.SendUIEffectTextAsync(8490, player.SteamID, true, "RoleValue", "TERRORIST");
                        break;
                }
            });
        }
    }
}
