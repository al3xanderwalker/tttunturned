using Newtonsoft.Json;

namespace TTTUnturned.API.Level
{
    public class Spawn
    {
        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        [JsonProperty("z")]
        public float Z { get; set; }

        public Spawn(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
