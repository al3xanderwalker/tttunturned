using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Players;
using TTTUnturned.API.Roles;
using TTTUnturned.API.Interface;
using PlayerManager = TTTUnturned.API.Players.PlayerManager;

namespace TTTUnturned.API.Commands
{
    public class CommandMenu : Command
    {
        protected override void execute(CSteamID executor, string parameter)
        {
            SteamPlayer ply = PlayerTool.getSteamPlayer(executor);
            if (ply is null) return;
            TTTPlayer tttplayer = PlayerManager.GetTTTPlayer(executor);
            switch (parameter) {
                case "t":
                    tttplayer.Role = PlayerRole.TRAITOR;
                    tttplayer.SendMessage("Role set to traitor");
                    InterfaceManager.ToggleShop(ply.player, tttplayer, 8501);
                    break;
                case "d":
                    tttplayer.Role = PlayerRole.DETECTIVE;
                    tttplayer.SendMessage("Role set to detective");
                    InterfaceManager.ToggleShop(ply.player, tttplayer, 8502);
                    break;
            }

            tttplayer.SendMessage(parameter);

        }

        public CommandMenu()
        {
            this.localization = new Local();
            this._command = "menu";
            this._info = "menu";
            this._help = "open menu";
        }
    }
}
