using RimWorld;
using Verse;
using System.Collections.Generic;

namespace MP_MechanitePlague
{
    //Basically, this is the comp itself. Override methods here.
    public class Comp_CleanBursterCorpse : ThingComp
    {
        //This is basically a connection between us and our properties.
        public CompProperties_CleanBursterCorpse CorpseSweeper
        {
            get
            {
                return this.props as CompProperties_CleanBursterCorpse;
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            //Oh, make sure this thing is powered first.
            CompPowerTrader powerComp = this.parent.TryGetComp<CompPowerTrader>();
            if (powerComp != null && powerComp.PowerOn)
            {
                currentTimer++;
            }

            if (currentTimer >= CorpseSweeper.timeBetweenCleans)
            {
                //If powered, do the thing!
                Map map = this.parent.Map;
                currentTimer = 0;

                //Locate the closest Burster corpse, and delete it.
                Thing foundThing = GenClosest.ClosestThing_Global(this.parent.Position, map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse), validator:
                delegate (Thing thing)
                {
                    if (thing is Corpse corpse && corpse.InnerPawn.def.defName == "MP_Mech_Burster")
                    {
                        return true;
                    }
                    return false;
                });

                if (foundThing != null)
                {
                    foundThing.Destroy();

                    //Generate TWO mechanite canisters. So generous.
                    IntVec3 location = this.parent.Position;
                    Thing drop = ThingMaker.MakeThing(ThingDef.Named("MP_MechaniteCanister"));
                    drop.stackCount = 2;
                    GenSpawn.Spawn(drop, location, this.parent.Map);
                }
            }

        }

        private int currentTimer = 0;
    }

    //This stores information on the comp.
    public class CompProperties_CleanBursterCorpse : CompProperties
    {
        //A connection between us and the comp we provide properties for.
        public CompProperties_CleanBursterCorpse()
        {
            this.compClass = typeof(Comp_CleanBursterCorpse);
        }

        public int timeBetweenCleans; //note: one day is 60,000 ticks, or 240 rare ticks
    }
}
