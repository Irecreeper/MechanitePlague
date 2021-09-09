using RimWorld;
using System;
using Verse;

namespace MP_MechanitePlague {
    public class ModExtension_MechPlagueOdds : DefModExtension {
        public float addHediffChance = 0.05f;
        public float hediffLowerValue = 0.10f;
        public float hediffUpperValue = 0.15f;
    }

    //A ThingComp to store the faction of the previous infector.
    public class ThingComp_InfectorFaction : ThingComp
    {
        public Faction infectorFaction = Faction.OfMechanoids;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_References.Look(ref infectorFaction, "infectorFaction");
        }
    }

    //Holds the method that applies plague to the target pawn.
    public class PlagueMethodHolder : DamageWorker_AddInjury
    {
        //Infect the target.
        public static void InfectPawn(Pawn hitPawn, Faction newFaction, float lowerVal, float higherVal)
        {
            //check if the target has the plague
            Hediff plagueOnPawn = hitPawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MP_MechanitePlague"));
            float randomSeverity = Rand.Range(lowerVal, higherVal) * (1 / hitPawn.BodySize) * Math.Max(0.5f + hitPawn.GetStatValue(StatDefOf.ToxicSensitivity) * 0.5f, 0.1f);

            //if the target has the plague, boost its intensity; otherwise, apply it
            if (plagueOnPawn != null)
            {
                plagueOnPawn.Severity += randomSeverity;
                hitPawn.GetComp<ThingComp_InfectorFaction>().infectorFaction = newFaction;
            }
            else
            {
                if (hitPawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MP_GuardianMechanitesHigh")) == null)
                {
                    Hediff hediff = HediffMaker.MakeHediff(HediffDef.Named("MP_MechanitePlague"), hitPawn);
                    hediff.Severity = randomSeverity;
                    hitPawn.health.AddHediff(hediff);
                    hitPawn.GetComp<ThingComp_InfectorFaction>().infectorFaction = newFaction;
                }
            }
        }

        //Check the plague statistics of the weapon. Roll plague, and apply if successful.
        public static void TryInfect(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result) 
        {
            ModExtension_MechPlagueOdds plagueInfo = dinfo.Weapon.GetModExtension<ModExtension_MechPlagueOdds>();

            if (plagueInfo != null && pawn != null && pawn is Pawn hitPawn && hitPawn.RaceProps.IsFlesh)
            {
                float rand = Rand.Value;
                if (rand <= plagueInfo.addHediffChance)
                {
                    Thing attacker = dinfo.Instigator;
                    PlagueMethodHolder.InfectPawn(hitPawn, attacker.Faction, plagueInfo.hediffLowerValue, plagueInfo.hediffUpperValue);
                }
            }
        }
    }

    //A class for ranged weapons to apply the plague.
    public class ModExtension_DamageWorkerPlagueRanged : DamageWorker_AddInjury
    {
        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            PlagueMethodHolder.TryInfect(pawn, totalDamage, dinfo, result);
            base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);
        }
    }

    //A few classes for melee attackers to apply the plague.
    //Finally did some code reuse, pog. I hope. There's a chance everything explodes, though.
    //Not sure how to work with Props properly yet, though. That's a problem for later, though.
    //If you're looking here, maybe shoot me a message? Anything helps.

    //Bite (Weaponless: uses damage type instead of weapon info)
    public class ModExtension_DamageWorkerPlagueBite : DamageWorker_Bite
    {
        public ModExtension_MechPlagueOdds Props => base.def.GetModExtension<ModExtension_MechPlagueOdds>();

        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            if (Props != null && pawn != null && pawn is Pawn hitPawn && hitPawn.RaceProps.IsFlesh)
            {
                float rand = Rand.Value;
                if (rand <= Props.addHediffChance)
                {
                    Thing attacker = dinfo.Instigator;
                    PlagueMethodHolder.InfectPawn(hitPawn, attacker.Faction, Props.hediffLowerValue, Props.hediffUpperValue);
                }
            }

            base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);
        }
    }

    //Cut 
    public class ModExtension_DamageWorkerPlagueCut : DamageWorker_Cut
    {
        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            PlagueMethodHolder.TryInfect(pawn, totalDamage, dinfo, result);
            base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);
        }
    }

    //Stab
    public class ModExtension_DamageWorkerPlagueStab : DamageWorker_Stab
    {
        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            PlagueMethodHolder.TryInfect(pawn, totalDamage, dinfo, result);
            base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);
        }
    }

    //...Basic? (Weaponless: uses damage type instead of weapon info)
    public class ModExtension_DamageWorkerPlagueBasic : DamageWorker_AddInjury
    {
        public ModExtension_MechPlagueOdds Props => base.def.GetModExtension<ModExtension_MechPlagueOdds>();

        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);

            if (Props != null && pawn != null && pawn is Pawn hitPawn && hitPawn.RaceProps.IsFlesh)
            {
                float rand = Rand.Value;
                if (rand <= Props.addHediffChance)
                {
                    Thing attacker = dinfo.Instigator;
                    PlagueMethodHolder.InfectPawn(hitPawn, attacker.Faction, Props.hediffLowerValue, Props.hediffUpperValue);
                }
            }
        }
    }
}