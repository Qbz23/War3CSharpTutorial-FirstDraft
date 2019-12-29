﻿using static War3Api.Common;

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

            //int grassId = Helpers.GetId("Lgrs");
            //SetTerrainType(0, 0, grassId, 0, 3, 0);

            Helpers.DebugPrint("Hello War3 C#!");
        }
    }
}   