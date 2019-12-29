using static War3Api.Common;
using System;

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
        static bool spellCondition()
        {
            return GetSpellAbilityId() == Helpers.GetId("A000");
        }

        static void spellActions()
        {
            Helpers.DebugPrint("My spell was cast!");
        }

        private static void Main()
        {
            FogEnable(false);
            FogMaskEnable(false);

            location spawnLoc = Location(250, 250);                                 
            int customUnitId = Helpers.GetId("O000");                           
            var myUnit = CreateUnitAtLoc(GetLocalPlayer(), customUnitId, spawnLoc, 0.0f);    
            RemoveLocation(spawnLoc);
            int customSpellId = Helpers.GetId("A000");
            UnitAddAbility(myUnit, customSpellId);

            var spellTrigger = CreateTrigger();
            TriggerRegisterPlayerUnitEvent(spellTrigger, GetLocalPlayer(), EVENT_PLAYER_UNIT_SPELL_EFFECT, null);
            TriggerAddCondition(spellTrigger, Condition(spellCondition));
            TriggerAddAction(spellTrigger, spellActions);

            // The player that will own our spawned units, 26 is neutral hostile 
            player neutralHostile = Player(26);
            // How many wisps to spawn 
            int numWisps = 12;
            // The radius of the cirlce to spawn wisps in 
            float circleRadius = 500.0f;
            // the angle between each wisp 
            float angleDelta = (2.0f * 3.1415f) / numWisps;
            // for each wisp we want to spawn...
            for (uint i = 0; i < numWisps; ++i)
            {
                // Calculate their position in the circle
                float x = circleRadius * (float)Math.Cos(angleDelta * i);
                float y = circleRadius * (float)Math.Sin(angleDelta * i);

                // Save that position as a location 
                location wispSpawnLoc = Location(x, y);
                // Spawn the wisp - It's easier to use ByName if you want to spawn 
                // a built-in unit compared to a custom unit 
                CreateUnitAtLocByName(neutralHostile, "Wisp", wispSpawnLoc, 0.0f);
                // Clean up location now that we no longer need it 
                RemoveLocation(wispSpawnLoc);
            }

            //int grassId = Helpers.GetId("Lgrs");
            //SetTerrainType(0, 0, grassId, 0, 3, 0);

            Helpers.DebugPrint("Hello War3 C#!");
        }
    }
}   