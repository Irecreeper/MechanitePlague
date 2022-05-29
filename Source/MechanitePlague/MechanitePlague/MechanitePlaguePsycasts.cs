using RimWorld;
using Verse;
using Verse.Sound;
using Verse.AI.Group;
using System.Collections.Generic;

namespace MP_MechanitePlague
{
    public class CompAbilityEffect_MechPlagueSpawn : CompAbilityEffect
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (target.Thing == null || target.Thing is Corpse) 
            {
                //Get the inner pawn.
                Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
                Map currentMap = innerPawn.MapHeld;
                bool legalSpawn = innerPawn.RaceProps.Humanlike || (innerPawn.RaceProps.Insect && LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowInsectSpawns) || LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowAnimalSpawns;

                if (innerPawn.Corpse.GetRotStage() == RotStage.Fresh && legalSpawn && innerPawn.RaceProps.IsFlesh)
                {
                    List<Pawn> pawnList = BursterHelper.SpawnBurster(innerPawn.Corpse.Position, currentMap, innerPawn, "MP_Mech_Burster", 0, Faction.OfPlayer, DecayMode.DecayRemove, 1);

                    //Special effects!
                    IntVec3 corpsePos = target.Thing.Position;
                    FleckMaker.Static(corpsePos, currentMap, FleckDefOf.PsycastSkipOuterRingExit, 1f);
                    SoundDefOf.Psycast_Skip_Exit.PlayOneShot(new TargetInfo(innerPawn.Corpse.Position, currentMap, false));

                    //Spawn filth from the birth of the pawn.
                    for (int j = 0; j < 10; j++)
                    {
                        IntVec3 intVec;
                        CellFinder.TryFindRandomReachableCellNear(corpsePos, currentMap, 2f, TraverseParms.For(pawnList[0], Danger.Deadly, TraverseMode.NoPassClosedDoorsOrWater), null, null, out intVec, 999999);
                        FilthMaker.TryMakeFilth(intVec, currentMap, ThingDefOf.Filth_Blood, 1, 0);
                    }

                    //Rot the body, if possible.
                    CompRottable canRot = innerPawn.Corpse.TryGetComp<CompRottable>();
                    if (canRot != null)
                    {
                        canRot.RotImmediately();
                    }
                }
            }
        }
    }
}
