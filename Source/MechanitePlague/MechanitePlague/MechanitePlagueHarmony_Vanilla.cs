using RimWorld;
using Verse;
using HarmonyLib;

namespace MP_MechanitePlague
{
    [StaticConstructorOnStartup]
    static class Patcher
    {
        static Patcher()
        {
            //Log.Warning("Patch was applied!");
            new Harmony("rimworld.Chairheir.MechanitePlague").PatchAll();
        }
    }

    //Death patch, to suppress death notifications.
    [HarmonyPatch(typeof(PawnUtility), "ShouldSendNotificationAbout")]
    static class DeathPatch
    {
        static bool Prefix(ref bool __result, Pawn p)
        {
            //Simple death message suppression.
            if (p.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MP_SpawnDecay")) != null || p.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MP_SpawnDecayRemove")) != null)
            {
                __result = false;
                return false;
            }
            else
            {
                return true;
            }

        }
    }

    //Hit patch, where if the comp is available, plague is applied on ALL ranged weapon hits.
    [HarmonyPatch(typeof(DamageWorker_AddInjury), "ApplySpecialEffectsToPart")]
    static class HitPatch
    {
        static void Prefix(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            //Check through each piece of apparel the attacker is wearing.
            if (dinfo.Instigator != null && dinfo.Instigator is Pawn attackerPawn && attackerPawn.RaceProps.Humanlike && dinfo.Weapon != null && dinfo.Weapon.thingClass != typeof(Building_TurretGun))
            {
                System.Collections.Generic.List<Apparel> appList = attackerPawn.apparel.WornApparel;
                if (appList != null)
                {
                    foreach (Apparel item in appList)
                    {
                        //For each piece of equipment, try and get the comp. If we find it, stop looping since it'll only apply once.
                        Comp_MechPlagueInflictApparel comp = item.TryGetComp<Comp_MechPlagueInflictApparel>();
                        if (comp != null)
                        {
                            comp.PlagueOnDamage(pawn, totalDamage, dinfo, result);
                        }
                    }
                }
            }
        }
    }    
}
