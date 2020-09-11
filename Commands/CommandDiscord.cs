using SDG.Unturned;
using Steamworks;

namespace TTTUnturned.Commands
{
    public class CommandDiscord : Command
    {
        protected override void execute(CSteamID executor, string parameter)
        {
            SteamPlayer ply = PlayerTool.getSteamPlayer(executor);
            if (ply is null) return;

            ply.player.sendBrowserRequest("Join our discord!", "https://discord.gg/CXNy9wt");

        }

        public CommandDiscord()
        {
            this.localization = new Local();
            this._command = "discord";
            this._info = "discord";
            this._help = "discord link";
        }
    }
}