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

        static bool filterCondition()
        {
            // The unit being potentially filtered out of the group
            unit checkedUnit = GetFilterUnit();
            // The unit that activated the trigger, in this case the caster 
            unit caster = GetTriggerUnit();
            // Include unit in the group if its an enemy and selectable (alive)
            return IsUnitEnemy(checkedUnit, GetOwningPlayer(caster)) && 
                   BlzIsUnitSelectable(checkedUnit);
        }

        static void spellActions()
        {
            // Range around the caster that targets can be hit from 
            const float spellRange = 750;
            // Max number of targets the spell can hit 
            const uint maxTargets = 6;
            // The amount of damage each strike deals 
            const float damage = 250;

            // Gets the unit that cast the spell associated with this trigger 
            // and saves it into a variable 
            unit caster = GetSpellAbilityUnit();
            // Gets the location of the caster 
            location startPos = GetUnitLoc(caster);

            // Create a group variable to hold the units the spell will hit 
            group targets = CreateGroup();
            // a variable to decrement each time we hit a target 
            uint count = maxTargets;
            // a unit variable to hold the target we're currently hitting 
            unit currentTarget;

            // Put all units within spellRange of startPos into the targets  group 
            GroupEnumUnitsInRangeOfLoc(targets, startPos, spellRange, Condition(filterCondition));
            // store the first unit in the group into the currentTarget variable
            currentTarget = FirstOfGroup(targets);

            // Time to play attack animation
            float attackTime = 0.6f;
            // Time to pause before the next teleport
            float teleportDelay = 0.05f;
            // Total time used for each unit to hit 
            float timePerUnit = attackTime + teleportDelay;
            // Determine how many units will be hit
            int numUnitsHit = (int)Math.Min(BlzGroupGetSize(targets), maxTargets);
            // Total time the spell should take 
            float followThroughTime = timePerUnit * numUnitsHit;
            // Sets the spell follow through time to the calculated value 
            BlzSetAbilityRealLevelField(GetSpellAbility(), ABILITY_RLF_FOLLOW_THROUGH_TIME, 0, followThroughTime);

            // While there's still a target to hit and we have't yet hit max targets
            while (currentTarget != null && count > 0)
            {

                // Get the location of the enemy we're targeting 
                location targetLocation = GetUnitLoc(currentTarget);
                // Teleport our caster to the enemy's location 
                SetUnitPositionLoc(caster, targetLocation);

                // You teleported to the enemy, but you didn't teleport in their 
                // exact same location, you got pushed out in some direction
                location newCasterPos = GetUnitLoc(caster);
                // Get the diference between the caster and the target 
                float deltaX = GetLocationX(targetLocation) - GetLocationX(newCasterPos);
                float deltaY = GetLocationY(targetLocation) - GetLocationY(newCasterPos);
                // Take the inverse tangent of that difference vector 
                // and convert it from radians to degrees 
                float angleInDegrees = 57.2957f * (float)Math.Atan2(deltaY, deltaX);
                // Make the caster face the calculated angle. 
                SetUnitFacing(caster, angleInDegrees);
                // Cleanup
                RemoveLocation(newCasterPos);

                // Have the caster play its attack animation
                SetUnitAnimation(caster, "attack");

                // Sleep before dealing damage while attack animation is playing
                TriggerSleepAction(attackTime);

                // Have the caster deal damage to the enemy 
                UnitDamageTarget(caster, currentTarget, damage, true, false,
                    ATTACK_TYPE_CHAOS, DAMAGE_TYPE_NORMAL, null);

                // Decrement the count, as we hit a target 
                count -= 1;

                // Take a brief pause before teleporting to the next target 
                TriggerSleepAction(teleportDelay);

                // Cleanup 
                RemoveLocation(targetLocation);

                // Remove the unit we just considered from the group 
                GroupRemoveUnit(targets, currentTarget);

                // Get the next unit in the group to consider. If the group is
                // empty, this will return null and break out of the while loop
                currentTarget = FirstOfGroup(targets);
            }

            // Cleanup
            RemoveLocation(startPos);
            DestroyGroup(targets);
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