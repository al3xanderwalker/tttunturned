using SDG.Unturned;
using Steamworks;
using TTTUnturned.Models;
using UnityEngine;

namespace TTTUnturned.Managers
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
            Lobby lobby = LobbyManager.GetLobby();
        }
    }
}
