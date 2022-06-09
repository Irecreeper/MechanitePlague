using Verse;
using HarmonyLib;
using MP_MechanitePlague;
using CombatExtended;
using System;

namespace MechanitePlague
{
    [StaticConstructorOnStartup]
    static class MechPlague_CEPatcher
    {
        static MechPlague_CEPatcher()
        {
            //Initiate the patching
            DoPatching();
        }

        public static void DoPatching()
        {
            var harmony = new Harmony("rimworld.Chairheir.MechanitePlague");

            if (Verse.ModsConfig.IsActive("CETeam.CombatExtended"))
            {
                var original = typeof(Comp_ProjectileSpawnBursters).GetMethod("PostDestroy");
                var postfix = typeof(MechPlague_CEPatcher).GetMethod("Postfix");
                harmony.Patch(original, postfix: new HarmonyMethod(postfix));
            }
        }

        public static void Postfix(Comp_ProjectileSpawnBursters __instance, DestroyMode mode, Map previousMap)
        {
            if (__instance.parent is ProjectileCE bullet)
            {
                BursterHelper.SpawnBurster(bullet.Position, previousMap, bullet.launcher, "MP_Mech_Burster", 0, bullet.launcher.Faction, DecayMode.DecayRemove, __instance.Props.amountSpawned);
            }
        }
    }
}
