using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using UnityEngine;

namespace TTTUnturned.API.Players
{
    public class DeathboxManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.Log("DeathboxManager loaded");
            PlayerLife.onPlayerDied += OnPlayerDied;
        }

        private void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            BarricadeManager.dropBarricade(new Barricade(366), sender.channel.owner.player.transform, sender.channel.owner.player.transform.position, 0f, 0f, 0f, (ulong)sender.channel.owner.playerID.steamID, 0UL);
        }
    }
}
