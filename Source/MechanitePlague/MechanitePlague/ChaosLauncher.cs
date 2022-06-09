using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MP_MechanitePlague
{
    public class CompProperties_ProjectileSpawnBursters : CompProperties
    {
        public CompProperties_ProjectileSpawnBursters()
        {
            this.compClass = typeof(Comp_ProjectileSpawnBursters);
        }

        public int amountSpawned = 1;
        public bool doStunPulse = false;
        public float stunPulseRadius = 5.0f;
    }

    public class Comp_ProjectileSpawnBursters : ThingComp
    {
        public CompProperties_ProjectileSpawnBursters Props
        {
            get
            {
                return (CompProperties_ProjectileSpawnBursters)this.props;
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            //Get our position.
            IntVec3 cell = this.parent.Position;

            //Stun foes in an area.
            if (this.Props.doStunPulse)
            {
                List<Pawn> pawnList = Find.CurrentMap.mapPawns.AllPawnsSpawned;
                foreach (Pawn checkPawn in pawnList)
                {
                    if (cell.DistanceTo(checkPawn.Position) <= this.Props.stunPulseRadius)
                    {
                        checkPawn.stances.stunner.StunFor(GenTicks.SecondsToTicks(3), this.parent, false, true);
                    }
                }
            }

            //Generate (small) burster horde.
            if (this.parent is Projectile bullet)
            {
                BursterHelper.SpawnBurster(bullet.Position, previousMap, bullet.Launcher, "MP_Mech_Burster", 0, bullet.Launcher.Faction, DecayMode.DecayRemove, this.Props.amountSpawned);
            }
            

            //Do... base things.
            base.PostDestroy(mode, previousMap);
        }
    }
}
