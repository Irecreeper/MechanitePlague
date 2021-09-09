using Verse;
using RimWorld;

namespace MP_MechanitePlague
{
    //Reminder to you that this is a HEDIFF comp. For hediffs.
    public class HediffCompProperties_MechPlagueRemoveOnDeath : HediffCompProperties
    {

        public HediffCompProperties_MechPlagueRemoveOnDeath()
        {
            this.compClass = typeof(HediffComp_MechPlagueRemoveOnDeath);
        }
    }

    public class HediffComp_MechPlagueRemoveOnDeath : HediffComp
    {
        public HediffCompProperties_MechPlagueRemoveOnDeath Props
        {
            get
            {
                return (HediffCompProperties_MechPlagueRemoveOnDeath)this.props;
            }
        }

        public override void Notify_PawnDied()
        {
            Map map = this.parent.pawn.Corpse.Map;
            if (map != null) {
                this.parent.pawn.Corpse.Destroy(DestroyMode.Vanish);
            }
        }
    }


}
