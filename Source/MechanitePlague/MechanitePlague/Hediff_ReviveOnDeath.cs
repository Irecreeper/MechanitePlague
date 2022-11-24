using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;
using MP_MechanitePlague;

namespace MP_MechanitePlague
{
    // Hediff comp class for lich revive.
    public class HediffCompProperties_ResurrectPostDeath : HediffCompProperties
    {

        public HediffCompProperties_ResurrectPostDeath()
        {
            this.compClass = typeof(HediffComp_ResurrectPostDeath);
        }
    }

    class HediffComp_ResurrectPostDeath : HediffComp
    {
        public HediffCompProperties_ResurrectPostDeath Props
        {
            get
            {
                return (HediffCompProperties_ResurrectPostDeath)this.props;
            }
        }

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            this.parent.Severity = 0.00f;

            // Activate the revival protocol.
            Messages.Message("MP_ReviveStart".Translate(this.parent.pawn), this.parent.pawn, MessageTypeDefOf.PositiveEvent, true);
            Current.Game.GetComponent<MP_RevivePawnAfterDelay>().AddToRevivalList(this.Pawn);
        }
    }
}
