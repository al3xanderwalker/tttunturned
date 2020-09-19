using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using UnityEngine;

namespace TTTUnturned.API.Items.BombVest
{
    public class BombVestManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.Log("BombVest Manager loaded");

            PlayerInput.onPluginKeyTick += OnPluginKeyTick;
        }

        private void OnPluginKeyTick(Player player, uint simulation, byte key, bool state)
        {
            if (!state || key != 1) return;

            if (player.clothing.vest == 1013)
            {
                // Refactor this into explode function
                player.clothing.askWearVest(0, 0, new byte[0], true);
                ExplosionParameters explodParams = new ExplosionParameters(player.transform.position, 10f, EDeathCause.KILL, CSteamID.Nil);
                explodParams.penetrateBuildables = true;
                explodParams.playerDamage = 150;
                explodParams.damageRadius = 32;
                explodParams.barricadeDamage = 1000;
                List<EPlayerKill> deadPlayers = new List<EPlayerKill>();
                EffectManager.sendEffect(45, byte.MaxValue, byte.MaxValue, byte.MaxValue, player.transform.position);
                DamageTool.explode(explodParams, out deadPlayers);
            }

        }
    }
}
