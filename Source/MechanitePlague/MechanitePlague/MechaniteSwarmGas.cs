using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MP_MechanitePlague
{
    // Causes a gas cloud to damage organic targets with the designated damage type.
    class MechaniteSwarmGas : Gas
    {
        public override void Tick()
        {
            base.Tick();
            damageOverTimeDelay--;
            if (damageOverTimeDelay <= 0)
            {
                damageOverTimeDelay = damageOverTimeBetween;

                try // there's a weird error that i can't seem to isolate that also seems to not be my fault. odd.
                {
                    List<Thing> thingsInGas = new List<Thing>(base.Position.GetThingList(this.Map));
                    if (thingsInGas != null)
                    {
                        foreach (Thing thing in thingsInGas)
                        {
                            if (thing != null && thing is Pawn pawn && !pawn.RaceProps.IsMechanoid)
                            {
                                // apply plague through script instead of damage: otherwise, faction issues
                                DamageWorker.DamageResult damageResult = pawn.TakeDamage(new DamageInfo(DefDatabase<DamageDef>.GetNamed("Cut", true), 4f, 0.5f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true));
                                if (damageResult.wounded)
                                {
                                    PlagueMethodHolder.InfectPawn(pawn, Faction.OfPlayer, 0.04f, 0.05f, 0);
                                }
                            }
                        }
                    }
                }
                catch
                { 
                    // blank
                }
            }
        }

        //The time between DoT ticks.
        private int damageOverTimeBetween = 120;

        //The time until the next DoT tick. (When equal to DamageOverTimeBetween, damages.)
        private int damageOverTimeDelay = 0;
    }
}
