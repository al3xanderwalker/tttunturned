using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTUnturned.API.Items
{
    public class BarricadeItem : Item
    {
        public BarricadeRegion Region { get; set; }
        public BarricadeDrop Drop { get; set; }
    }
}
