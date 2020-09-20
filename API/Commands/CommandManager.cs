using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using UnityEngine;

namespace TTTUnturned.API.Commands
{
    public class CommandManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.Log("CommandManager loaded");
            ChatManager.onCheckPermissions += OnCheckPermissions;
        }

        #region Events
        private void OnCheckPermissions(SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList)
        {
            if (text.StartsWith("/discord"))
            {
                shouldExecuteCommand = true;
            }
        }
        #endregion
    }
}
