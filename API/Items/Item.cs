using SDG.Unturned;

namespace TTTUnturned.API.Items
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ItemType ItemType { get; set; }
        public SDG.Unturned.Item UnturnedItem { get; set; }
        public ItemAsset Asset { get; set; }
    }
}
