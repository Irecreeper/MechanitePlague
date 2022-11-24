using System.Collections.Generic;
using VanillaPsycastsExpanded;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;
using MP_MechanitePlague;

namespace MP_MechanitePlagueVPE
{
    // Hediff class for lich food + rest restoration.
    public class HediffCompProperties_LichdomFoodRest : HediffCompProperties
    {
        public HediffCompProperties_LichdomFoodRest()
        {
            this.compClass = typeof(HediffComp_LichdomFoodRest);
        }
    }

    public class HediffComp_LichdomFoodRest : HediffComp
    {
        int regenTime = 256;
        int regenUntil = 256;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.regenUntil--;
            if (this.regenUntil <= 0)
            {
                regenUntil = regenTime;
                if (this.Pawn.needs.food != null) {
                    this.Pawn.needs.food.CurLevel += 0.01f;
                }

                if (this.Pawn.needs.rest != null) {
                    this.Pawn.needs.rest.CurLevel += 0.01f;
                }
            }
        }
    }

    // Hediff comp class for incredibly violent damage over time.
    public class HediffCompProperties_DisintegrateDamage : HediffCompProperties
    {
        public HediffCompProperties_DisintegrateDamage()
        {
            this.compClass = typeof(HediffComp_DisintegrateDamage);
        }
    }

    public class HediffComp_DisintegrateDamage : HediffComp
    {
        int tickTime = 30;
        int tickUntil = 30;

        public HediffCompProperties_DisintegrateDamage Props
        {
            get
            {
                return (HediffCompProperties_DisintegrateDamage)this.props;
            }
        }

        // DoT
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            
            this.tickUntil--;
            if (this.tickUntil <= 0)
            {
                tickUntil = tickTime;
                this.Pawn.TakeDamage(new DamageInfo(DefDatabase<DamageDef>.GetNamed("Cut", true), 3f, 1f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true));
            }
        }
    }
}
