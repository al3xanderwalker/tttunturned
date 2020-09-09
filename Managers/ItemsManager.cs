using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;

namespace TTTUnturned.Managers
{
    public class ItemsManager
    {
        public static void respawnItems()
        {
            for (byte x = 0; x < Regions.WORLD_SIZE; x++) // Straight from uEssentials, dont judge me lmao
            {
                for (byte y = 0; y < Regions.WORLD_SIZE; y++)
                {

                    var itemsCount = LevelItems.spawns[x, y].Count;
                    if (itemsCount <= 0) continue;

                    for (var i = 0; i < itemsCount; i++)
                    {
                        var itemSpawnpoint = LevelItems.spawns[x, y][i];
                        var itemId = LevelItems.getItem(itemSpawnpoint);

                        if (itemId == 0) continue;

                        var item = new Item(itemId, true);
                        ItemManager.dropItem(item, itemSpawnpoint.point, false, false, false);
                    }
                }
            }
        }
    }
}
