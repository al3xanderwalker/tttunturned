using System;
using SDG.Unturned;
namespace TTTUnturned.API.Items.SilencedPistol
{
    public class SilencedPistol
    {
        public static SDG.Unturned.Item Create()
        {
            SDG.Unturned.Item gun = new SDG.Unturned.Item(1021, true);
            Asset SelectAsset = Assets.find(EAssetType.ITEM, 7);
            ItemAsset Item = (ItemAsset)SelectAsset;
            byte[] ID = BitConverter.GetBytes(Item.id);
            Array.Copy(ID, 0, gun.state, 6, 2);
            return gun;
        }
    }
}
