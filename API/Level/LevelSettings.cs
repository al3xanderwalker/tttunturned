using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTUnturned.API.Level
{
    public class LevelSettings
    {
        public static void SetConfig()
        {
            if (Main.Config.DebugMode == true)
            {
                Provider.hasCheats = true;
            }

            Provider.mode = EGameMode.NORMAL;
            Provider.configData.Normal.Items.Gun_Bullets_Full_Chance = 1;
            Provider.configData.Normal.Items.Quality_Full_Chance = 1;
            Provider.configData.Normal.Items.Has_Durability = false;

            Provider.configData.Normal.Players.Can_Start_Bleeding = false;
            Provider.configData.Normal.Players.Can_Break_Legs = false;
            Provider.configData.Normal.Players.Food_Use_Ticks = uint.MaxValue;
            Provider.configData.Normal.Players.Water_Use_Ticks = uint.MaxValue;
            Provider.configData.Normal.Players.Allow_Per_Character_Saves = false;

            Provider.configData.Normal.Gameplay.Can_Suicide = false;
            Provider.configData.Normal.Gameplay.Allow_Static_Groups = false;
            Provider.configData.Normal.Gameplay.Allow_Dynamic_Groups = false;
            Provider.configData.Normal.Gameplay.Group_Player_List = false;

            Provider.configData.Server.Chat_Always_Use_Rich_Text = true;
        }
    }
}
