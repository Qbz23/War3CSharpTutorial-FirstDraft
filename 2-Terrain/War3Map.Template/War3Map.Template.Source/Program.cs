using static War3Api.Common;

namespace War3Map.Template.Source
{
    internal class Helpers
    { 
        public static void DebugPrint(string s)
        {
            DisplayTextToPlayer(GetLocalPlayer(), 0, 0, s);
        }

        public static int GetId(string s)
        {
            return (s[0] << 24) | (s[1] << 16) | (s[2] << 8) | (s[3]);
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            FogEnable(false);
            FogMaskEnable(false);

            int grassId = Helpers.GetId("Lgrs");
            SetTerrainType(0, 0, grassId, 0, 3, 0);

            Helpers.DebugPrint("Hello War3 C#!");
        }
    }
}