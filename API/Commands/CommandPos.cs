using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using TTTUnturned.API.Lobby;

namespace TTTUnturned.API.Commands
{
    public class CommandPos : Command
    {
        protected override void execute(CSteamID executor, string parameter)
        {
            SteamPlayer ply = PlayerTool.getSteamPlayer(executor);
            if (ply is null) return;

            CommandWindow.Log(ply.player.transform.position.ToString());
            LobbyManager.Message(ply.player.transform.position.ToString(), ply);

            List<EPlayerKill> deadPlayers = new List<EPlayerKill>();
            EffectManager.sendEffect(45, byte.MaxValue, byte.MaxValue, byte.MaxValue, ply.player.transform.position);
            DamageTool.explode(ply.player.transform.position, 10f, EDeathCause.KILL, CSteamID.Nil, 10, 200, 200, 200, 200, 200, 200, 200, out deadPlayers, EExplosionDamageType.CONVENTIONAL, 32, true, false, EDamageOrigin.Unknown);
        }

        public CommandPos()
        {
            this.localization = new Local();
            this._command = "pos";
            this._info = "pos";
            this._help = "log player position";
        }
    }
}