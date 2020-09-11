using Newtonsoft.Json;
using System.Collections.Generic;

namespace TTTUnturned.Models
{
    public class SpawnsConfig
    {
        [JsonProperty("debugMode")]
        public bool DebugMode { get; set; }
        [JsonProperty("lobbySpawns")]
        public List<Spawn> Spawns { get; set; }
        [JsonProperty("maps")]
        public List <Map> Maps { get; set; }
        [JsonProperty("roundLength")]
        public int RoundLength { get; set; }
        [JsonProperty("minimumPlayers")]
        public int MinimumPlayers { get; set; }

    }
}
