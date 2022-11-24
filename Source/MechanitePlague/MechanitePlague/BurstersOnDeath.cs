using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MP_MechanitePlague
{
    //A simple death worker that causes Incubators to spawn Bursters on death.
    class BurstersOnDeath : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse)
        {
            IntVec3 position = corpse.Position;
            Map map = corpse.Map;

            BursterHelper.SpawnBurster(position, map, corpse.InnerPawn, "MP_Mech_Burster", 0, corpse.InnerPawn.Faction, DecayMode.DecayStandard, 4);
        }
    }
}
