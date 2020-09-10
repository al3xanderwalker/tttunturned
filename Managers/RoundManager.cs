using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTT.Utils;
using TTTUnturned.Models;
using TTTUnturned.Utils;
using UnityEngine;

namespace TTTUnturned.Managers
{
    public class RoundManager : MonoBehaviour
    {
        public void Awake()
        {
            CommandWindow.Log("RoundManager loaded");

            AsyncHelper.Schedule("RoundTick", RoundTick, 1000);
        }

        private async Task RoundTick()
        {
            Lobby lobby = LobbyManager.GetLobby();

            if (lobby.State != LobbyState.LIVE) return;

            lobby.RoundTime--;
            CommandWindow.Log(lobby.RoundTime);

            if (lobby.RoundTime == 60)
            {
                // TODO: Make this execute async so it doesnt block the thread
                UnityThread.executeInUpdate(() => { LobbyManager.Message("1 minute remaining"); });
            }

            if (lobby.RoundTime == 0)
            {
                UnityThread.executeInUpdate(() =>
                {
                    lobby.Stop();
                    LobbyManager.Message("Timer expired! Innocents win.");
                });
                return;
            }
        }

        public static string ParseTime(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            return t.ToString(@"mm\:ss");
        }
    }
}
