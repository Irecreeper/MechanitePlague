using Verse;
using Verse.Sound;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;

namespace MP_MechanitePlague {

    //An enum to help determine the spawn mode.
    public enum DecayMode
    { 
        DecayNone, //No decay is applied.
        DecayStandard, //Applies a decay that kills.
        DecayRemove //Applies a decay that kills, then removes the body.
    }

    //Spawns a single entity from the location of the target corpse.
    public class HediffCompProperties_MechPlagueSpawnFromCorpse : HediffCompProperties 
    {

        public HediffCompProperties_MechPlagueSpawnFromCorpse() {
            this.compClass = typeof(HediffComp_MechanitePlagueSpawn);
        }

        public string thingSpawned = "";
        public int bonusSpawnCount = 0;
        public bool forcePlayerFaction = false;
    }

    public class BursterHelper
    {
        public static List<Pawn> SpawnBurster(IntVec3 spawnPosition, Map map, Thing mother, string thingSpawned, int bonusSpawnCount, Faction factionToAssign, DecayMode decayMode, int forceSpawnCount = 0)
        {
            List<Pawn> spawnedPawns = new List<Pawn>();

            //Get the base number of spawns.
            int baseSpawns = 0;
            if (forceSpawnCount != 0)
            {
                baseSpawns = forceSpawnCount;
            }
            else
            {
                baseSpawns = bonusSpawnCount + Rand.Range(LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().intervalSpawnCount.min, 1 + LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().intervalSpawnCount.max);
            }

            //Spawn the boy.
            for (int i = 0; i < baseSpawns; i++)
            {
                //Create a new lord. Having multiple SHOULD be fine.
                Lord newLord = LordMaker.MakeNewLord(factionToAssign, new LordJob_AssaultColony(factionToAssign, canKidnap: false, canTimeoutOrFlee: false, canSteal: false, useAvoidGridSmart: false, breachers: false, canPickUpOpportunisticWeapons: false), map, null);

                //Spawn the pawn!
                Pawn spawnedPawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDef.Named(thingSpawned), factionToAssign, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, 1f, false, false, false, false, false, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null));
                GenPlace.TryPlaceThing(spawnedPawn, spawnPosition, map, ThingPlaceMode.Direct);
                if (spawnedPawn.relations == null)
                {
                    spawnedPawn.relations = new Pawn_RelationsTracker(spawnedPawn);
                }
                newLord.AddPawn(spawnedPawn);
                spawnedPawns.Add(spawnedPawn);

                //Give the pawn a disease that'll eventually kill them. Maybe.
                if (decayMode == DecayMode.DecayStandard)
                {
                    Hediff hediff = HediffMaker.MakeHediff(HediffDef.Named("MP_SpawnDecay"), spawnedPawn);
                    hediff.Severity = 0.01f;
                    spawnedPawn.health.AddHediff(hediff);
                }
                else if (decayMode == DecayMode.DecayRemove)
                {
                    Hediff hediff = HediffMaker.MakeHediff(HediffDef.Named("MP_SpawnDecayRemove"), spawnedPawn);
                    hediff.Severity = 0.01f;
                    spawnedPawn.health.AddHediff(hediff);
                }
            }

            //Play a sound.
            SoundStarter.PlayOneShot(SoundDefOf.Hive_Spawn, new TargetInfo(spawnPosition, map, false));

            //Return the list of spawned pawns, just in case.
            return spawnedPawns;
        }
    }

    public class HediffComp_MechanitePlagueSpawn : HediffComp
    {

        public HediffCompProperties_MechPlagueSpawnFromCorpse Props
        {
            get
            {
                return (HediffCompProperties_MechPlagueSpawnFromCorpse)this.props;
            }
        }

        //A method called when the pawn with the plague dies.
        public override void Notify_PawnDied()
        {
            //Read spawning severity.
            float severityToSpawn = LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().intensityToSpawn;

            //Gather the information of Props.
            string thingSpawned = this.Props.thingSpawned;
            int bonusSpawnCount = this.Props.bonusSpawnCount;
            bool forcePlayerFaction = this.Props.forcePlayerFaction;

            //Get our victim.
            Pawn victim = this.parent.pawn;
            Map map = victim.Corpse.Map;

            //Determine faction.
            Faction newFaction;

            if (forcePlayerFaction)
            {
                newFaction = Find.FactionManager.OfPlayer;
            }
            else
            {
                newFaction = victim.GetComp<ThingComp_InfectorFaction>().infectorFaction;
                if (newFaction == null)
                { //in case it was applied through some odd means, like a detonating mortar shell
                    newFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.Mechanoid);
                }
            }

            //Determine amount of extra bursters.
            int extraSpawnsFromWeapon = victim.GetComp<ThingComp_InfectorFaction>().extraSpawns;

            //If all checks are met, do the thing.
            bool legalSpawn = victim.RaceProps.Humanlike || (victim.RaceProps.Insect && LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowInsectSpawns) || LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowAnimalSpawns;
            if (this.parent.Severity >= severityToSpawn && map != null && legalSpawn && victim.RaceProps.IsFlesh)
            {
                List<Pawn> pawnList = BursterHelper.SpawnBurster(victim.Position, map, victim, thingSpawned, bonusSpawnCount + extraSpawnsFromWeapon, newFaction, DecayMode.DecayStandard);

                //Spawn filth from the birth of the pawn.
                for (int j = 0; j < 10; j++)
                {
                    IntVec3 intVec;
                    CellFinder.TryFindRandomReachableCellNear(victim.Position, map, 2f, TraverseParms.For(pawnList[0], Danger.Deadly, TraverseMode.NoPassClosedDoorsOrWater), null, null, out intVec, 999999);
                    FilthMaker.TryMakeFilth(intVec, map, ThingDefOf.Filth_Blood, 1, 0);
                }

                //Rot the body, if possible / allowed.
                CompRottable canRot = victim.Corpse.TryGetComp<CompRottable>();
                if (canRot != null && LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().corpsesRotOnSpawn)
                {
                    canRot.RotImmediately();
                }
            }
        }
    }
}
