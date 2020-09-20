using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using TTTUnturned.API.Roles;
using TTTUnturned.API.Round;
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
            // Gets a list of all players in the round that arent traitors
            List<TTTPlayer> players = RoundManager.GetAlivePlayersWithRole(PlayerRole.INNOCENT);
            RoundManager.GetAlivePlayersWithRole(PlayerRole.DETECTIVE).ForEach(p => players.Add(p));
            List<TTTPlayer> alive = players.FindAll(p => p.Status == PlayerStatus.ALIVE);

            CalculateKillCredit(players, alive, 0.75);
            CalculateKillCredit(players, alive, 0.50);
            CalculateKillCredit(players, alive, 0.25);

        }
        #endregion
        #region Functions
        private void CalculateKillCredit(List<TTTPlayer> players, List<TTTPlayer> alive, double percent)
        {
            // Check if the percentage of people alive before a player dies is above 
            if ((alive.Count + 1) / players.Count > percent || alive.Count / players.Count <= percent) { 
                alive.ForEach(p =>
                {
                    if (p.Role == PlayerRole.TRAITOR)
                    { 
                        p.Credits += 1;
                        p.SendMessage("You have gained 1 credit");
                    }
                });
            }
        }
        #endregion
    }
}
