using static War3Api.Common;

namespace War3Map.Template.Source
{
    internal class Helpers
    { 
        public static void DebugPrint(string s)
        {
            DisplayTextToPlayer(GetLocalPlayer(), 0, 0, s);
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            Helpers.DebugPrint("Hello War3 C#!");
        }
    }
}