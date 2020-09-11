using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;

namespace TTTUnturned.Managers
{
    public class ConfigManager
    {
        public static void SetConfig()
        {
            Provider.hasCheats = true;
            Provider.mode = EGameMode.NORMAL;
            Provider.configData.Normal.Items.Gun_Bullets_Full_Chance = 1;
            Provider.configData.Normal.Items.Quality_Full_Chance = 1;
            Provider.configData.Normal.Gameplay.Can_Suicide = false;
            Provider.configData.Normal.Players.Can_Start_Bleeding = false;
            Provider.configData.Normal.Players.Food_Use_Ticks = uint.MaxValue;
            Provider.configData.Normal.Players.Water_Use_Ticks = uint.MaxValue;
            Provider.configData.Normal.Players.Allow_Per_Character_Saves = false;
        }
    }
}
