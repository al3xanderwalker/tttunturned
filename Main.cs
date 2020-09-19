using Nito.AsyncEx;
using SDG.Framework.Modules;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TTTUnturned.API.Core;
using TTTUnturned.Utils;
using UnityEngine;

namespace TTTUnturned
{
    public class Main : MonoBehaviour, IModuleNexus
    {
        private GameObject TTTUnturnedObject;
        public static Config Config;

        public void initialize()
        {
            CommandWindow.Log("TTTUnturned loaded");

            UnityThread.initUnityThread();

            Patcher patch = new Patcher();
            Patcher.DoPatching();

            ConfigHelper.EnsureConfig($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}config.json");
            Config = ConfigHelper.ReadConfig($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}config.json");

            TTTUnturnedObject = new GameObject();
            var componentManagers = Assembly.GetExecutingAssembly().DefinedTypes
                .Where(type => type.ImplementedInterfaces.Any(inter => inter == typeof(IObjectComponent))).ToList();
            componentManagers.ForEach(c =>
            {
                MethodInfo methodInfo = typeof(GameObject).GetMethods()
                    .Where(x => x.IsGenericMethod)
                    .Where(x => x.Name == "AddComponent").Single();
                MethodInfo addComponentRef = methodInfo.MakeGenericMethod(c);
                addComponentRef.Invoke(TTTUnturnedObject, null);
            });
        }

        public void shutdown()
        {

        }
    }
}
