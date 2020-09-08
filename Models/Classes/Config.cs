using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTUnturned.Models
{
    public class SpawnsConfig
    {
        [JsonProperty("lobbySpawns")]
        public List<Spawn> Spawns { get; set; }
        [JsonProperty("maps")]
        public List <Map> Maps { get; set; }

    }
}
