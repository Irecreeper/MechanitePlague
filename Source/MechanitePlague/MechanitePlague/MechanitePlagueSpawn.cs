using Verse;
using Verse.Sound;
using RimWorld;
using Verse.AI.Group;

namespace MP_MechanitePlague {

    //Spawns a single entity from the location of the target corpse.
    public class HediffCompProperties_MechPlagueSpawnFromCorpse : HediffCompProperties {

        public HediffCompProperties_MechPlagueSpawnFromCorpse() {
            this.compClass = typeof(HediffComp_MechanitePlagueSpawn);
        }

        public string thingSpawned = "";
        public float severityToSpawn = 0.15f;
        public int bonusSpawnCount = 0;
        public bool forcePlayerFaction = false;
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

        public override void Notify_PawnDied()
        {
            //The base number of spawns per spawn function. Can vary with mod settings.
            int baseSpawns = Rand.Range(LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().intervalSpawnCount.min, 1 + LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().intervalSpawnCount.max);

            //First, make sure the dead pawn is actually on a map!
            if (this.parent.pawn.MapHeld != null)
            {
                //Check if we have a map (which we probably do), and if our target is a humanlike.
                Map map = this.parent.pawn.Corpse.Map;
                bool legalSpawn = this.parent.pawn.RaceProps.Humanlike || (this.parent.pawn.RaceProps.Insect && LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowInsectSpawns) || LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowAnimalSpawns;
                if (map != null && this.parent.Severity >= this.Props.severityToSpawn && legalSpawn && this.parent.pawn.RaceProps.IsFlesh)
                {
                    //Determine the spawned mech's faction.
                    Faction newFaction;

                    if (Props.forcePlayerFaction)
                    {
                        newFaction = Find.FactionManager.OfPlayer;
                    }
                    else
                    {
                        newFaction = this.parent.pawn.GetComp<ThingComp_InfectorFaction>().infectorFaction;
                        if (newFaction == null)
                        { //in case it was applied through some odd means, like a detonating mortar shell
                            newFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.Mechanoid);
                        }
                    }

                    for (int i = 0; i < baseSpawns + Props.bonusSpawnCount; i++)
                    { 
                        //Create a new lord. Having multiple SHOULD be fine.
                        Lord newLord = LordMaker.MakeNewLord(newFaction, new LordJob_AssaultColony(newFaction, canKidnap: false, canTimeoutOrFlee: false, canSteal: false, useAvoidGridSmart: false, breachers: false, canPickUpOpportunisticWeapons: false), map, null);
                       
                        //Spawn the pawn!
                        Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDef.Named(this.Props.thingSpawned), newFaction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null));
                        PawnUtility.TrySpawnHatchedOrBornPawn(pawn, this.parent.pawn.Corpse);
                        if (pawn.relations == null) 
                        {
                            pawn.relations = new Pawn_RelationsTracker(this.Pawn);
                        }
                        newLord.AddPawn(pawn);

                        //Give the pawn a disease that'll eventually kill them.
                        Hediff hediff = HediffMaker.MakeHediff(HediffDef.Named("MP_SpawnDecay"), pawn);
                        hediff.Severity = 0.01f;
                        pawn.health.AddHediff(hediff);

                        //Spawn filth from the birth of the pawn.
                        for (int j = 0; j < 10; j++)
                        {
                            IntVec3 intVec;
                            CellFinder.TryFindRandomReachableCellNear(this.parent.pawn.Corpse.Position, map, 2f, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.NoPassClosedDoorsOrWater), null, null, out intVec, 999999);
                            FilthMaker.TryMakeFilth(intVec, this.parent.pawn.Corpse.Map, ThingDefOf.Filth_Blood, 1, 0);
                        }
                    }

                    //Play a sound.
                    SoundStarter.PlayOneShot(SoundDefOf.Hive_Spawn, new TargetInfo(this.parent.pawn.Corpse.Position, map, false));

                    //Rot the body, if possible.
                    CompRottable canRot = this.parent.pawn.Corpse.TryGetComp<CompRottable>();
                    if (canRot != null) 
                    {
                        canRot.RotImmediately();
                    }
                }
            }
        }
    }
}
