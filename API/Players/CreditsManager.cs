using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using TTTUnturned.API.Roles;
using UnityEngine;

namespace TTTUnturned.API.Players
{
    public class CreditsManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.Log("CreditsManager loaded");

            PlayerLife.onPlayerDied += OnPlayerDied;
        }

        #region Events
        private void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            TTTPlayer victim = PlayerManager.GetTTTPlayer(sender.channel.owner.playerID.steamID);

            if (victim.GetRole() == PlayerRole.TRAITOR || victim.GetRole() == PlayerRole.DETECTIVE)
            {
                victim.SetCredits(0);
                return;
            }

            TTTPlayer killer = PlayerManager.GetTTTPlayer(instigator);
            if (killer is null) return;

            if (killer.GetRole() == PlayerRole.DETECTIVE && victim.GetRole() == PlayerRole.TRAITOR)
            {
                killer.AddCredits(1);
                return;
            }

            if (killer.GetRole() == PlayerRole.TRAITOR && victim.GetRole() != PlayerRole.TRAITOR)
            {
                killer.AddCredits(1);
                return;
            }
        }
        #endregion
    }
}
