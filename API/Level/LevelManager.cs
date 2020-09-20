using System.Collections;
using SDG.Unturned;
using TTTUnturned.Utils;
using System.Threading.Tasks;

namespace TTTUnturned.API.Level
{
    public class LevelManager
    {
        public static void RespawnItems()
        {
            UnityThread.executeCoroutine(RespawnItemsAsync());
        }

        public static void ClearBarricadesUnsafe()
        {
            UnityThread.executeCoroutine(ClearBarricadesEnumerator());   
        }
        
        private static IEnumerator ClearBarricadesEnumerator()
        {
            BarricadeManager.askClearAllBarricades();
            yield return null;
        }

        private static IEnumerator RespawnItemsAsync()
        {
            SDG.Unturned.ItemManager.askClearAllItems();
            for (byte x = 0; x < Regions.WORLD_SIZE; x++) // Straight from uEssentials, dont judge me lmao
            {
                for (byte y = 0; y < Regions.WORLD_SIZE; y++)
                {

                    var itemsCount = SDG.Unturned.LevelItems.spawns[x, y].Count;
                    if (itemsCount <= 0) continue;

                    for (var i = 0; i < itemsCount; i++)
                    {
                        var itemSpawnpoint = SDG.Unturned.LevelItems.spawns[x, y][i];
                        var itemId = SDG.Unturned.LevelItems.getItem(itemSpawnpoint);

                        if (itemId == 0) continue;

                        var item = new Item(itemId, true);
                        SDG.Unturned.ItemManager.dropItem(item, itemSpawnpoint.point, false, false, false);
                    }
                }
            }

            yield return null;
        }
    }
}
