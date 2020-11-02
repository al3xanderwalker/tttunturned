using SDG.Unturned;
using Steamworks;
using PlayerManager = TTTUnturned.API.Players.PlayerManager;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTUnturned.API.Commands
{
    public class CommandPos : Command
    {
        protected override void execute(CSteamID executor, string parameter)
        {
            SteamPlayer ply = PlayerTool.getSteamPlayer(executor);
            if (ply is null) return;

            Vector3 position = ply.player.transform.position;
            PlayerManager.GetTTTPlayer(executor).SendMessageUnsafe($"X: {Math.Round(position.x, 1)} Y: {Math.Round(position.y, 1)} Z: {Math.Round(position.z, 1)}");

        }

        public CommandPos()
        {
            this.localization = new Local();
            this._command = "pos";
            this._info = "pos";
            this._help = "pos";
        }
    }
}
