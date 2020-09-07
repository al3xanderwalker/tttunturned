using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TTTUnturned.Models;

namespace TTTUnturned.Managers
{
    public class LobbyManager : MonoBehaviour
    {
        public static Lobby Lobby;

        public void Awake()
        {
            CommandWindow.Log("LobbyManager loaded");

            Lobby = CreateLobbyInitial();

            Provider.onEnemyConnected += OnEnemyConnected;
        }

        private void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            if (Lobby.State == LobbyState.SETUP)
            {
                if(Lobby.Players.Count == 5)
                {
                    //Lobby.start()
                }
                // if lobby players > 5 call the start function

                // Add player to lobby
            }

            if (Lobby.State == LobbyState.LIVE)
            {
                // Send player to waiting area
            }
        }

        private Lobby CreateLobbyInitial()
        {
            Lobby createdSession = new Lobby(LobbyState.SETUP);
            return createdSession;
        }

        private void StartGameSession(Lobby lobby)
        {

        }

        /*
        public static LobbyState GetLobbyState(Lobby lobby)
        {
            
        }
        */
    }
}
