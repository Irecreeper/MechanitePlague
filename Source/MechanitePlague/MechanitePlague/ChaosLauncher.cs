using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MP_MechanitePlague
{
    class Projectile_ExplosiveSpawnChaos : Projectile_Explosive
    {
        protected override void Explode()
        {
            //Get our position.
            IntVec3 cell = this.Position;

            //Stun foes in an area.
            List<Pawn> pawnList = Find.CurrentMap.mapPawns.AllPawnsSpawned;
            foreach (Pawn checkPawn in pawnList)
            {
                if (cell.DistanceTo(checkPawn.Position) <= 5.8f)
                {
                    checkPawn.stances.stunner.StunFor(GenTicks.SecondsToTicks(3), this.Launcher, false, true);
                }
            }

            //Generate burster horde.
            BursterHelper.SpawnBurster(cell, this.Map, this, "MP_Mech_Burster", 0, this.launcher.Faction, DecayMode.DecayStandard, 8);

            //also explode
            base.Explode();
        }
    }

    class Projectile_ExplosiveSpawnIncubator : Projectile_Explosive
    {
        protected override void Explode()
        {
            //Get our position.
            IntVec3 cell = this.Position;

            //Generate (small) burster horde.
            BursterHelper.SpawnBurster(cell, this.Map, this, "MP_Mech_Burster", 0, this.launcher.Faction, DecayMode.DecayRemove, 1);

            //also explode
            base.Explode();
        }
    }
}
