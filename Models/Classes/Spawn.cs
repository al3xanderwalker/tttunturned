using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTUnturned.Models
{
    public class Spawn
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Spawn(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
