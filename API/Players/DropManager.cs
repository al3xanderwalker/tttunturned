using SDG.Unturned;
using Steamworks;
using TTTUnturned.API.Lobby;
using UnityEngine;

namespace TTTUnturned.API.Players
{
    public class DropManager : MonoBehaviour
    {
        public void Awake()
        {
            CommandWindow.Log("DropManager loaded");

            PlayerLife.onPlayerDied += OnPlayerDied;
        }

        private void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            LobbySession lobby = LobbyManager.GetLobby();
        }
    }
}
