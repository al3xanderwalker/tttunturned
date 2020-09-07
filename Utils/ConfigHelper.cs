using TTTUnturned.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTUnturned.Utils
{
    public class Config
    {
        public int exampleValue { get; set; }

        //public List<ArenaConfig> arenas { get; set; }
    }

    public class ConfigHelper
    {
        public static void EnsureConfig(string path)
        {
            if (!File.Exists(path))
            {
                CommandWindow.Log("No TTTUnturned config found, generating...");

                JObject tttUnuturnedConfig = new JObject();
                tttUnuturnedConfig.Add("exampleValue", 10);


                //combatArenaConfig["arenas"] = JToken.FromObject(defaultArenas);

                using (StreamWriter file = File.CreateText(path))
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    tttUnuturnedConfig.WriteTo(writer);
                    CommandWindow.Log("Generated TTTUnturned config");
                }
            }
        }

        public static Config ReadConfig(string path)
        {
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                return JsonConvert.DeserializeObject<Config>(JToken.ReadFrom(reader).ToString());
            }
        }
    }
}
