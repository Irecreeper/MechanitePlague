using System;
using Verse;
using RimWorld;
using System.Collections.Generic;

namespace MP_MechanitePlague
{
    /* 
     * A specialized hediff class for the Mechanite Plague.
     * Causes it to deal damage over time to downed targets.
     * Can be disabled through mod settings.
     */
    public class Hediff_MechanitePlague : HediffWithComps
    {
        public override void Tick()
        {
            base.Tick();
            this.damageOverTimeDelay++;
            if (this.damageOverTimeDelay >= this.damageOverTimeBetween)
            {
                //Reset delay.
                this.damageOverTimeDelay = 0;

                //Check if we can legally harm the target.
                Hediff plagueOnPawn = this.pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MP_MechanitePlague"));
                bool settingFlag = LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().doDownedDamageOverTime;
                bool tendFlag = this.IsTended();
                bool downFlag = this.pawn.Downed && this.pawn.CarriedBy == null;
                bool progressionFlag = plagueOnPawn.Severity >= 0.15f;
                bool validSpeciesFlag = this.pawn.RaceProps.Humanlike || (this.pawn.RaceProps.Insect && LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowInsectSpawns) || LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowAnimalSpawns;
                bool deathrestFlag = this.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("Deathrest")) == null && this.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("RegenerationComa")) == null;
                if (settingFlag && !tendFlag && downFlag && progressionFlag && validSpeciesFlag && deathrestFlag)
                {
                    //If we get through all of these checks, attempt the most taxing check.
                    //Search through the map to find if there is a pawn close enough to you to prevent the DoT.
                    bool finalCheck = true;
                    List<Pawn> pawns = Find.CurrentMap.mapPawns.AllPawnsSpawned;
                    IntVec3 cell = this.pawn.Position;
                    foreach (Pawn pawn in pawns)
                    {
                        if (pawn != this.pawn && !pawn.Downed && pawn.RaceProps.Humanlike && cell.DistanceTo(pawn.Position) <= 1.5f)
                        {
                            finalCheck = false;
                            break;
                        }
                    }

                    if (finalCheck)
                    {
                        //Damage the core part of the pawn.
                        BodyPartRecord targetPart = pawn.RaceProps.body.corePart;
                        this.pawn.TakeDamage(new DamageInfo(DefDatabase<DamageDef>.GetNamed("Stab", true), 1f, 0f, -1f, null, targetPart, null, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true));
                    }
                }
            }
        }

        //The time between DoT ticks.
        private int damageOverTimeBetween = 128;

        //The time until the next DoT tick. (When equal to DamageOverTimeBetween, damages.)
        private int damageOverTimeDelay = 0;
    }
}
