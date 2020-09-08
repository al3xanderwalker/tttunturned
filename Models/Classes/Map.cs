using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTUnturned.Models
{
    public class Map
    {
        public string Name { get; set; }
        public List<Spawn> Spawns { get; set; }

        public Map(string name, List<Spawn> spawns)
        {
            Name = name;
            Spawns = spawns;
        }
    }
}
