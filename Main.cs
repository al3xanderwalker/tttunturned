using TTTUnturned.Utils;
using SDG.Framework.Modules;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TTT.Utils;
using TTTUnturned.Managers;

namespace TTTUnturned
{
    public class Main : MonoBehaviour, IModuleNexus
    {
        private static GameObject TTTUnturnedObject;
        public static Main Instance;
        public static Config Config;

        public void initialize()
        {
            Instance = this;

            Patcher patch = new Patcher(); // Create patcher object and call PatchAll
            Patcher.DoPatching();

            UnityThread.initUnityThread(); // Init UnityThread helper

            // Generate config
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ConfigHelper.EnsureConfig($"{path}{Path.DirectorySeparatorChar}config.json");
            Config = ConfigHelper.ReadConfig($"{path}{Path.DirectorySeparatorChar}config.json");
            CommandWindow.Log($"DEBUG MODE: {Config.DebugMode}");
            // Load our module as a gameObject
            TTTUnturnedObject = new GameObject("TTTUnturned");
            DontDestroyOnLoad(TTTUnturnedObject);

            TTTUnturnedObject.AddComponent<LobbyManager>();
            TTTUnturnedObject.AddComponent<RoleManager>();

            CommandWindow.Log("TTTUnturned by Corbyn & Alex loaded");
        }

        public void shutdown()
        {
            Instance = null;
        }
    }
}
