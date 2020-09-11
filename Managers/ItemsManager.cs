using System.Collections;
using SDG.Unturned;
using TTTUnturned.Utils;

namespace TTTUnturned.Managers
{
    public class ItemsManager
    {
        public static void RespawnItems()
        {
            UnityThread.executeCoroutine(RespawnItemsAsync());
        }

        private static IEnumerator RespawnItemsAsync()
        {
            ItemManager.askClearAllItems();
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

            yield return null;
        }
    }
}
