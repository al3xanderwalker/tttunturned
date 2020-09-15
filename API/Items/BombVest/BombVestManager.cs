using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using TTTUnturned.API.Core;
using UnityEngine;

namespace TTTUnturned.API.Items.BombVest
{
    public class BombVestManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.Log("BombVest Manager loaded");

            DamageTool.damagePlayerRequested += OnDamageRequested;
            PlayerInput.onPluginKeyTick += OnPluginKeyTick;
        }

        private void OnDamageRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (parameters.damage >= parameters.player.life.health)
            {
                if (parameters.player.clothing.vest.ToString() == "1013")
                {
                    parameters.player.clothing.askWearVest(0, 0, new byte[0], true); // Doesnt work and needs fixing
                    /*
                    parameters.player.inventory.items[2].items.ForEach(item =>
                    {
                        if (item.item.id == 1013) ply.inventory.items[2].removeItem(ply.inventory.items[2].getIndex(item.x, item.y));
                    });
                    */
                    ExplosionParameters explodParams = new ExplosionParameters(parameters.player.transform.position, 10f, EDeathCause.KILL, CSteamID.Nil);
                    explodParams.penetrateBuildables = true;
                    explodParams.playerDamage = 150;
                    explodParams.damageRadius = 32;
                    explodParams.barricadeDamage = 1000;
                    List<EPlayerKill> deadPlayers = new List<EPlayerKill>();
                    EffectManager.sendEffect(45, byte.MaxValue, byte.MaxValue, byte.MaxValue, parameters.player.transform.position);
                    DamageTool.explode(explodParams, out deadPlayers);
                }
            }
        }
        private void OnPluginKeyTick(Player player, uint simulation, byte key, bool state)
        {
            if (!state || key != 1) return;
            if (player.clothing.vest.ToString() == "1013")
            {
                player.clothing.askWearVest(0, 0, new byte[0], true); // Doesnt work and needs fixing
                /*
                parameters.player.inventory.items[2].items.ForEach(item =>
                {
                    if (item.item.id == 1013) ply.inventory.items[2].removeItem(ply.inventory.items[2].getIndex(item.x, item.y));
                });
                */
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
