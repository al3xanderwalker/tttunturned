using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TTTUnturned.Models;
using Steamworks;

namespace TTTUnturned.Managers
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

            int terroistCount = (int)Math.Floor(Provider.clients.Count / 8.0);
            int detectiveCount = (int)Math.Floor(Provider.clients.Count / 12.0);
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

        public static void tellRoles(Lobby lobby)
        {
            lobby.Players.ForEach(player =>
            {
                SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(player.SteamID);
                switch (player.Role) {
                    case PlayerRole.INNOCENT:
                        LobbyManager.Message($"You are a <color=green>Innocent</color>", steamPlayer);
                        break;
                    case PlayerRole.DETECTIVE:
                        LobbyManager.Message($"You are a <color=blue>Detective</color>", steamPlayer);
                        break;
                    case PlayerRole.TERRORIST:
                        LobbyManager.Message($"You are a <color=red>Terroist</color>", steamPlayer);
                        break;
                }
            });
        }
    }
}
