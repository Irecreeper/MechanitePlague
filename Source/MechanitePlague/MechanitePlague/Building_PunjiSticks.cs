using Verse;
using Verse.Sound;
using RimWorld;
using System;
using MechanitePlague;

namespace MP_MechanitePlague
{
    class Building_PunjiSticks : Building_TrapDamager
    {
        protected override void SpringSub(Pawn p)
        {
            // Play sound!
            SoundDefOf.TrapSpring.PlayOneShot(new TargetInfo(base.Position, base.Map, false));

            // Only actually do trap things if the pawn... exists.
            if (p != null)
            {
                // Get damage stats and target info.
                float damageAmount = this.GetStatValue(StatDefOf.TrapMeleeDamage);
                float armorPenetration = damageAmount * 0.015f;
                BodyPartHeight targetHeight = (p.BodySize >= 0.35f) ? BodyPartHeight.Bottom : BodyPartHeight.Undefined;
                BodyPartRecord targetPart = p.health.hediffSet.GetRandomNotMissingPart(PlagueDamageDefOf.MP_StabInfect, targetHeight, BodyPartDepth.Undefined, null);
                DamageInfo dinfo = new DamageInfo(PlagueDamageDefOf.MP_StabInfect, damageAmount, armorPenetration, -1, this, targetPart, this.def, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true);

                // Actually deal the damage.
                DamageWorker.DamageResult damageResult = p.TakeDamage(dinfo);

                // Store the results of the combat in the log.
                BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(p, RulePackDefOf.DamageEvent_TrapSpike, null);
                Find.BattleLog.Add(battleLogEntry_DamageTaken);
                damageResult.AssociateWithLog(battleLogEntry_DamageTaken);
            }
        }
    }
}
