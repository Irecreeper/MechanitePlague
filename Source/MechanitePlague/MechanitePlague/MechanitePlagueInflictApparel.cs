using RimWorld;
using Verse;

namespace MP_MechanitePlague
{
    //Reminder to you that this is a THING comp. For things.
    public class CompProperties_MechPlagueInflictApparel : CompProperties
    {
        public CompProperties_MechPlagueInflictApparel()
        {
            this.compClass = typeof(Comp_MechPlagueInflictApparel);
        }
    }

    public class Comp_MechPlagueInflictApparel : ThingComp
    {
        public CompProperties_MechPlagueInflictApparel Props
        {
            get
            {
                return this.props as CompProperties_MechPlagueInflictApparel;
            }
        }

        public void PlagueOnDamage(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            //Since we're not using props, just sorta... do our own check.
            if (pawn != null && pawn.RaceProps.IsFlesh)
            {
                PlagueMethodHolder.InfectPawn(pawn, dinfo.Instigator.Faction, totalDamage / 100 * 0.45f, totalDamage / 100 * 0.55f, 0);
            }
        }
    }
}
