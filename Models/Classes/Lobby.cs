﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using Steamworks;
using TTTUnturned.Managers;
using TTTUnturned.Utils;
using TTTUnturned.Models;
using UnityEngine;
using System.Collections;

namespace TTTUnturned.Models
{
    public class Lobby
    {
        public LobbyState State { get;  set; }
        public int RoundTime { get; set; }
        public List<LobbyPlayer> Players { get; set; }

        public Lobby(LobbyState state)
        {
            State = state;
            RoundTime = Main.Config.RoundLength;
            Players = new List<LobbyPlayer>();
        }

        public Lobby(LobbyState state, List<LobbyPlayer> players)
        {
            State = state;
            RoundTime = Main.Config.RoundLength;
            Players = players;
        }

        public async Task Start()
        {
            // CHECK IF WE CAN START THE GAME
            int playersRequired;
            if (Main.Config.DebugMode) playersRequired = 2;
            else playersRequired = 5;
            if (State == LobbyState.SETUP)
            {
                if (Provider.clients.ToList().Count >= playersRequired)
                {
                    LobbyManager.Message("Starting game session...");
                    // Assign all players a role
                    Players = RoleManager.GeneratePlayerRoles();
                    // Spawn items
                    ItemsManager.RespawnItems();
                    // Teleport all players to spawn point
                    System.Random rng = new System.Random();
                    List<Spawn> spawns = Main.Config.Maps[rng.Next(Main.Config.Maps.Count)].Spawns;
                    Players.ForEach(player =>
                    {
                        int t = rng.Next(spawns.Count);
                        Vector3 spawn = new Vector3(spawns[t].X, spawns[t].Y, spawns[t].Z);
                        SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(player.SteamID);
                        TeleportToLocation(steamPlayer, spawn);
                    });
                    LobbyManager.Message("Game is live, roles will be assigned in 30 seconds!");
                    // Wait 30 seconds before displaying roles and allowing damage
                    State = LobbyState.WAITING;
                    await Task.Delay(10000); // CHANGE TO 30 LATER TODO: Add this value to config
                                             // Display roles
                    RoleManager.tellRoles(this);
                    State = LobbyState.LIVE;
                    return;
                }
                else
                {
                    LobbyManager.Message($"<color=red>{playersRequired - Provider.clients.Count}</color> more players needed to start game.");
                    return;
                }
            }
        }

        public async Task Stop()
        {
            State = LobbyState.SETUP; // Set game state to setup
            RoundTime = Main.Config.RoundLength; // Reset round timer

            LobbyManager.Message("Round ending in 10 seconds"); // Fix hardcoded time value

            await Task.Delay(10000);
            //Teleport players NOT kill them 
            Players.ForEach(player => 
            {
                if(player.Status == PlayerStatus.ALIVE) UnityThread.executeCoroutine(ResetPlayer(player));
            });
            LobbyManager.Message("New round starting in 20 seconds"); // Fix hardcoded time value
            await Task.Delay(20000);
            AsyncHelper.RunAsync("RestartLobby", Start);
            // Track stats in database
        }

        public IEnumerator ResetPlayer(LobbyPlayer player)
        {
            Player ply = PlayerTool.getPlayer(player.SteamID);
            if (ply is null) yield return null;
            ply.life.sendRevive();
            for (byte page = 0; page < 6; page++)
            {
                for (byte i = 0; i < ply.inventory.items[page].getItemCount(); i++)
                {
                    if (ply.inventory.items[page].getItem(i) != null) 
                    {
                        ItemJar item = ply.inventory.items[page].getItem(i);
                        ply.inventory.removeItem(page, ply.inventory.getIndex(page, item.x, item.y));
                    }
                }
            }

            System.Random rng = new System.Random();
            List<Spawn> spawns = Main.Config.LobbySpawns;

            int t = rng.Next(spawns.Count);
            Vector3 spawn = new Vector3(spawns[t].X, spawns[t].Y, spawns[t].Z);
            ply.teleportToLocation(spawn, 0f);
            
            yield return null;
        }

        public void TeleportToLocation(SteamPlayer steamPlayer, Vector3 location)
        {
            UnityThread.executeCoroutine(TeleportToLocationAsync(steamPlayer, location));
        }

        private IEnumerator TeleportToLocationAsync(SteamPlayer steamPlayer, Vector3 location)
        {
            steamPlayer.player.teleportToLocation(location, 0f);

            yield return null;
        }

        public List<LobbyPlayer> GetAlive(PlayerRole role)
        {
            return Players.FindAll(player => player.Role == role && player.Status == PlayerStatus.ALIVE);
        }
    }
}
