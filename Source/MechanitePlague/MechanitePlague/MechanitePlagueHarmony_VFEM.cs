using HarmonyLib;
using Verse;
using System;

namespace MP_MechanitePlague
{
    [StaticConstructorOnStartup]
    class MechPlague_VFEMPatcher
    {
        static MechPlague_VFEMPatcher()
        {
            //Initiate the patching!
            DoPatching();
        }

        public static void DoPatching()
        {
            var harmony = new Harmony("rimworld.Chairheir.MechanitePlague");

            if (Verse.ModsConfig.IsActive("OskarPotocki.VanillaFactionsExpanded.Core"))
            {
                var original = AccessTools.Method(Type.GetType("VFE.Mechanoids.HarmonyPatches.MachinesDie, VFECore"), "Postfix");
                var prefix = typeof(MechPlague_VFEMPatcher).GetMethod("Prefix");
                harmony.Patch(original, prefix: new HarmonyMethod(prefix));
                //Log.Message("Mechanite Plague: VFE:M detected! Patching...");
            }
        }

        public static bool Prefix(Pawn __0)
        {
            //If the mech that died is of a certain type, SKIP this part.
            if (__0.def == ThingDef.Named("MP_Mech_Burster"))
            {
                return false; //skip
            }
            else
            {
                return true; //don't skip
            }
        }
    }
}
