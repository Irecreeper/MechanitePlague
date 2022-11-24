using HarmonyLib;
using Verse;
using RimWorld;
using System;

namespace MP_MechanitePlague
{
    [StaticConstructorOnStartup]
    class MechPlague_VEPatcher
    {
        static MechPlague_VEPatcher()
        {
            //Initiate the patching!
            DoPatching();
        }

        public static void DoPatching()
        {
            var harmony = new Harmony("rimworld.Chairheir.MechanitePlague");

            if (Verse.ModsConfig.IsActive("OskarPotocki.VanillaFactionsExpanded.Core"))
            {
                //Log.Message("Mechanite Plague: VFE:M detected! Patching...");
                var original = AccessTools.Method(Type.GetType("VFE.Mechanoids.HarmonyPatches.MachinesDie, VFECore"), "Postfix"); // NOTE: you are patching a method called "Postfix", this is not specifying that this is a postfix!!!
                var prefix = typeof(MechPlague_VEPatcher).GetMethod("VFEMDeathMessageSuppress");
                harmony.Patch(original, prefix: new HarmonyMethod(prefix));
            }

            if (Verse.ModsConfig.IsActive("VanillaExpanded.VPsycastsE"))
            {
                //Log.Message("Mechanite Plague: VPE detected! Patching...");
                var original2 = AccessTools.Method(typeof(ThoughtWorker_HasAddedBodyPart), "CurrentStateInternal");
                var prefix2 = typeof(MechPlague_VEPatcher).GetMethod("VPETranshumanistLichdom");
                harmony.Patch(original2, prefix: new HarmonyMethod(prefix2));
            }
        }

        public static bool VFEMDeathMessageSuppress(Pawn __0)
        {
            //If the entity that died has one of two hediffs, skip this part.
            if (__0.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MP_SpawnDecay")) != null || __0.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MP_SpawnDecayRemove")) != null)
            {
                return false; //skip
            }
            else
            {
                return true; //don't skip
            }
        }

        public static bool VPETranshumanistLichdom(ref ThoughtState __result, Pawn __0)
        {
            if (__0.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MP_VPE_Lichdom")) != null)
            {
                __result = ThoughtState.ActiveAtStage(20); //5 is the maximum, but just in case a mod does something...
                return false; //skip
            }
            else
            {
                return true; //don't skip
            }
        }
    }
}
