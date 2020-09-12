using System.Collections;
using SDG.Unturned;
using TTTUnturned.Utils;
using System.Threading.Tasks;
using UnityEngine;

namespace TTTUnturned.Managers
{
    public class ItemManager
    {
        public static void RespawnItems()
        {
            UnityThread.executeCoroutine(RespawnItemsAsync());
        }

        private static IEnumerator RespawnItemsAsync()
        {
            SDG.Unturned.ItemManager.askClearAllItems();
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
                        SDG.Unturned.ItemManager.dropItem(item, itemSpawnpoint.point, false, false, false);
                    }
                }
            }

            yield return null;
        }
        public static async Task AddItemAync(SteamPlayer steamPlayer, ushort id)
        {
            UnityThread.executeCoroutine(AddItemCoroutine(steamPlayer, id));
        }

        private static IEnumerator AddItemCoroutine(SteamPlayer steamPlayer, ushort id)
        {
            steamPlayer.player.inventory.forceAddItem(new Item(id, true), true);

            yield return null;
        }
    }
}
