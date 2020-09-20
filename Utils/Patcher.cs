using HarmonyLib;

namespace TTTUnturned.Utils
{
    public class Patcher
    {
        public static void DoPatching()
        {
            var harmony = new Harmony("com.tttunturned.patch");
            harmony.PatchAll();
        }
    }
}