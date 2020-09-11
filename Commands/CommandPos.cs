using SDG.Unturned;
using Steamworks;
using TTTUnturned.Managers;

namespace TTTUnturned.Commands
{
    public class CommandPos : Command
    {
        protected override void execute(CSteamID executor, string parameter)
        {
            SteamPlayer ply = PlayerTool.getSteamPlayer(executor);
            if (ply is null) return;

            CommandWindow.Log(ply.player.transform.position.ToString());
            LobbyManager.Message(ply.player.transform.position.ToString(), ply);
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