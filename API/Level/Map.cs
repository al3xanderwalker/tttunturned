using Newtonsoft.Json;
using System.Collections.Generic;

namespace TTTUnturned.API.Level
{
    public class Map
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("spawns")]
        public List<Spawn> Spawns { get; set; }

        public Map(string name, List<Spawn> spawns)
        {
            Name = name;
            Spawns = spawns;
        }
    }
}
