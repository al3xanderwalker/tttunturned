using TTTUnturned.Utils;
using SDG.Framework.Modules;
using SDG.Unturned;
using System.IO;
using System.Reflection;
using UnityEngine;
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
            // Level Config
            ConfigManager.SetConfig();

            // Load our module as a gameObject
            TTTUnturnedObject = new GameObject("TTTUnturned");
            DontDestroyOnLoad(TTTUnturnedObject);

            // Add managers as gameObject components
            TTTUnturnedObject.AddComponent<LobbyManager>();
            TTTUnturnedObject.AddComponent<RoundManager>();
            TTTUnturnedObject.AddComponent<RoleManager>();

            CommandWindow.Log("TTTUnturned by Corbyn & Alex loaded");
        }

        public void shutdown()
        {
            Instance = null;
        }
    }
}
