using RimWorld;
using Verse;
using Verse.Sound;
using Verse.AI.Group;

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

                //If the pawn is rotten or inhuman, do not spawn a thing.
                if (innerPawn.Corpse.GetRotStage() == RotStage.Fresh && innerPawn.RaceProps.Humanlike && innerPawn.RaceProps.IsFlesh)
                {
                    //First, make sure the dead pawn is actually on a map!
                    if (currentMap != null)
                    {
                        //Determine the spawned mech's faction. It'll be the same as the caster's.
                        Faction casterFaction = this.parent.pawn.Faction;

                        //Create a new lord. Having multiple SHOULD be fine.
                        Lord newLord = LordMaker.MakeNewLord(casterFaction, new LordJob_AssaultColony(casterFaction, canKidnap: false, canTimeoutOrFlee: false, canSteal: false, useAvoidGridSmart: false, breachers: false, canPickUpOpportunisticWeapons: false), currentMap, null);

                        //Spawn the pawn!
                        Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDef.Named("MP_Mech_Burster"), casterFaction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null));
                        PawnUtility.TrySpawnHatchedOrBornPawn(pawn, innerPawn);
                        if (pawn.relations == null)
                        {
                            pawn.relations = new Pawn_RelationsTracker(innerPawn);
                        }
                        newLord.AddPawn(pawn);

                        //Give the pawn a disease that'll eventually kill them. This variant will remove them from the map on death.
                        Hediff hediff = HediffMaker.MakeHediff(HediffDef.Named("MP_SpawnDecayRemove"), pawn);
                        hediff.Severity = 0.01f;
                        pawn.health.AddHediff(hediff);

                        //Spawn filth from the birth of the pawn.
                        for (int j = 0; j < 10; j++)
                        {
                            IntVec3 intVec;
                            CellFinder.TryFindRandomReachableCellNear(innerPawn.Position, currentMap, 2f, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.NoPassClosedDoorsOrWater), null, null, out intVec, 999999);
                            FilthMaker.TryMakeFilth(intVec, currentMap, ThingDefOf.Filth_Blood, 1, 0);
                        }

                        IntVec3 corpsePos = innerPawn.Corpse.Position;
                        FleckMaker.Static(corpsePos, currentMap, FleckDefOf.PsycastSkipOuterRingExit, 1f);
                        SoundDefOf.Psycast_Skip_Exit.PlayOneShot(new TargetInfo(innerPawn.Corpse.Position, currentMap, false));
                    }

                    //Oh, also rot the pawn. Since we're here.
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
