using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.Utils;
using System.Collections;
using SDG.Unturned;
using Steamworks;

namespace TTTUnturned.Models
{
   
    public class C4
    {
        public bool Defused { get; set; }
        public int TimeLeft { get; set; }
        public BarricadeRegion Region { get; set; }
        public BarricadeDrop Drop { get; set; }

        public C4(BarricadeRegion region, BarricadeDrop drop, int time)
        {
            Defused = false;
            TimeLeft = time;
            Region = region;
            Drop = drop;
            CommandWindow.Log("C4 Planted");
            AsyncHelper.Schedule("RoundTick", C4Tick, 1000);
        }

        private async Task C4Tick()
        {
            TimeLeft--;
            if(TimeLeft == 0)
            {
                UnityThread.executeCoroutine(C4DetonateCoroutine());
            }
        }
        private IEnumerator C4DetonateCoroutine()
        {
            EffectManager.sendEffect(45, byte.MaxValue, byte.MaxValue, byte.MaxValue, Drop.model.position);
            ExplosionParameters parameters = new ExplosionParameters(Drop.model.position, 150f, EDeathCause.KILL, CSteamID.Nil);
            parameters.penetrateBuildables = true;
            parameters.playerDamage = 150;
            parameters.damageRadius = 64;
            parameters.barricadeDamage = 1000;
            List<EPlayerKill> deadPlayers = new List<EPlayerKill>();
            DamageTool.explode(parameters, out deadPlayers);
            // Region.destroy(); // DESTROYS IT BUT doesnt delete the in game model

            yield return null;
        }
    }
}
