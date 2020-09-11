using TTTUnturned.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDG.Unturned;
using System.Collections.Generic;
using System.IO;

namespace TTTUnturned.Utils
{
    public class Config
    {
        public int exampleValue { get; set; }

        public List<Spawn> LobbySpawns { get; set; }

        public List<Map> Maps { get; set; }

        public int RoundLength { get; set; }

        public bool DebugMode { get; set; }
    }

    public class ConfigHelper
    {
        public static void EnsureConfig(string path)
        {
            if (!File.Exists(path))
            {
                CommandWindow.Log("No TTTUnturned config found, generating...");

                JObject tttUnuturnedConfig = new JObject();
                tttUnuturnedConfig.Add("roundLength", 600);
                tttUnuturnedConfig.Add("debugMode", true);

                Spawn exampleSpawn = new Spawn(0, 0, 0);
                List<Spawn> Spawns = new List<Spawn>();
                Spawns.Add(exampleSpawn);
                tttUnuturnedConfig["lobbySpawns"] = JToken.FromObject(Spawns);

                Map exampleMap = new Map("Example", Spawns);
                List<Map> Maps = new List<Map>();
                Maps.Add(exampleMap);
                tttUnuturnedConfig["maps"] = JToken.FromObject(Maps);

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
