using Verse;
using RimWorld;
using System.Collections.Generic;

namespace MP_MechanitePlague
{
    class Recipe_MechPlagueForceIncubate : RecipeWorker
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (this.IsViolationOnPawn(pawn, part, Faction.OfPlayer))
            {
                base.ReportViolation(pawn, billDoer, pawn.HomeFaction, -100);
            }
            Hediff hediff = HediffMaker.MakeHediff(HediffDef.Named("MP_RapidIncubation"), pawn);
            hediff.Severity = 0.01f;
            pawn.health.AddHediff(hediff);
        }
    }
}
