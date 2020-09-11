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
using TTTUnturned.Managers;
using System.Collections;

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

            Provider.hasCheats = true;
            Provider.configData.Normal.Items.Gun_Bullets_Full_Chance = 1;
            Provider.configData.Normal.Items.Quality_Full_Chance = 1;
            Provider.configData.Normal.Gameplay.Can_Suicide = false;
            Provider.configData.Normal.Players.Can_Start_Bleeding = false;
            Provider.configData.Normal.Players.Food_Use_Ticks = uint.MaxValue;
            Provider.configData.Normal.Players.Water_Use_Ticks = uint.MaxValue;
            Provider.configData.Normal.Players.Allow_Per_Character_Saves = false;

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
