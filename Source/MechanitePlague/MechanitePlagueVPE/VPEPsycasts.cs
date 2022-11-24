using System.Collections.Generic;
using VanillaPsycastsExpanded;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;
using Verse.AI.Group;
using MP_MechanitePlague;

namespace MP_MechanitePlagueVPE
{
    class Ability_MechskipVPE : VFECore.Abilities.Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            foreach (GlobalTargetInfo globalTargetInfo in targets)
            {
                Corpse corpse = globalTargetInfo.Thing as Corpse;
                IntVec3 position = corpse.Position;

                //Get the inner pawn.
                Pawn innerPawn = corpse.InnerPawn;
                Map currentMap = innerPawn.MapHeld;
                bool legalSpawn = innerPawn.RaceProps.Humanlike || (innerPawn.RaceProps.Insect && LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowInsectSpawns) || LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowAnimalSpawns;

                if (innerPawn.Corpse.GetRotStage() == RotStage.Fresh && legalSpawn && innerPawn.RaceProps.IsFlesh)
                {
                    // Spawn!
                    List<Pawn> pawnList = BursterHelper.SpawnBurster(innerPawn.Corpse.Position, currentMap, innerPawn, "MP_Mech_Burster", 0, Faction.OfPlayer, DecayMode.DecayRemove, 1);

                    //Spawn filth from the birth of the pawn.
                    FilthMaker.TryMakeFilth(position, this.pawn.Map, ThingDefOf.Filth_Blood, 3, FilthSourceFlags.None);

                    //Rot the body, if possible.
                    CompRottable canRot = innerPawn.Corpse.TryGetComp<CompRottable>();
                    if (canRot != null)
                    {
                        canRot.RotImmediately();
                    }
                }
            }
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            bool result = false;
            Corpse corpse = target.Thing as Corpse;
            if (corpse != null)
            {
                bool legalSpawn = !corpse.InnerPawn.RaceProps.IsMechanoid
                    && corpse.GetRotStage() == RotStage.Fresh
                    && (corpse.InnerPawn.RaceProps.Humanlike || (corpse.InnerPawn.RaceProps.Insect && LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowInsectSpawns) || LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowAnimalSpawns);
                if (legalSpawn)
                {
                    result = base.ValidateTarget(target, showMessages);
                }
            }

            if (!result && showMessages)
            {
                Messages.Message("MP_VPE_ValidMechskipTarget".Translate(), corpse, MessageTypeDefOf.CautionInput, true);
            }

            return result;
        }
    }

    class Ability_MechskipMassVPE : VFECore.Abilities.Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            foreach (GlobalTargetInfo globalTargetInfo in targets)
            {
                Thing thing = globalTargetInfo.Cell.GetFirstItem(globalTargetInfo.Map);
                if (thing != null && thing is Corpse corpse)
                {
                    IntVec3 position = corpse.Position;

                    //Get the inner pawn.
                    Pawn innerPawn = corpse.InnerPawn;
                    Map currentMap = innerPawn.MapHeld;
                    bool legalSpawn = innerPawn.RaceProps.Humanlike || (innerPawn.RaceProps.Insect && LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowInsectSpawns) || LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowAnimalSpawns;

                    if (innerPawn.Corpse.GetRotStage() == RotStage.Fresh && legalSpawn && innerPawn.RaceProps.IsFlesh)
                    {
                        List<Pawn> pawnList = BursterHelper.SpawnBurster(innerPawn.Corpse.Position, currentMap, innerPawn, "MP_Mech_Burster", 0, Faction.OfPlayer, DecayMode.DecayRemove, 1);

                        //Special effects!
                        IntVec3 corpsePos = corpse.Position;
                        FleckMaker.Static(corpsePos, currentMap, FleckDefOf.PsycastSkipOuterRingExit, 1f);
                        SoundDefOf.Psycast_Skip_Exit.PlayOneShot(new TargetInfo(innerPawn.Corpse.Position, currentMap, false));

                        //Spawn filth from the birth of the pawn.
                        FilthMaker.TryMakeFilth(position, this.pawn.Map, ThingDefOf.Filth_Blood, 3, FilthSourceFlags.None);

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

    class Ability_SpawnScytherDirectVPE : VFECore.Abilities.Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            foreach (GlobalTargetInfo globalTargetInfo in targets)
            {
                IntVec3 position = globalTargetInfo.Cell;
                BursterHelper.SpawnBurster(position, globalTargetInfo.Map, null, "MP_VPE_Mech_Scyther_Summoned", 0, Faction.OfPlayer, DecayMode.DecayRemove, 1);
            }
        }
    }

    class Ability_MultishotVPE : VFECore.Abilities.Ability_ShootProjectile
    {
        int extraShots;
        int timeBetweenShots;
        int timeUntilNextShot;
        GlobalTargetInfo spellTarget;

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets); // fires once!

            extraShots = 4;
            timeBetweenShots = 12;
            timeUntilNextShot = timeBetweenShots;
            Log.Warning("cast!");

            foreach (GlobalTargetInfo target in targets)
            {
                spellTarget = target;
            }
        }

        public override void Tick()
        {
            base.Tick();
            timeUntilNextShot--;
            if (timeUntilNextShot <= 0 && extraShots > 0)
            {
                timeUntilNextShot = timeBetweenShots;
                extraShots--;
                ShootProjectile(spellTarget);
            }
        }
    }

    class Ability_PlagueSwarmCloudVPE : VFECore.Abilities.Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            foreach (GlobalTargetInfo globalTargetInfo in targets)
            {
                GenExplosion.DoExplosion(globalTargetInfo.Cell, globalTargetInfo.Map, this.GetRadiusForPawn(), DamageDefOf.Smoke, this.Caster, -1, -1f, null, null, null, null, ThingDef.Named("MP_MechaniteSwarmCloud"), 1f, 1, null, false, null, 0f, 1, 0f, false, null, null);
            }
        }
    }

    class Ability_CorpseExplosionVPE : VFECore.Abilities.Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            foreach (GlobalTargetInfo globalTargetInfo in targets)
            {
                Corpse corpse = globalTargetInfo.Thing as Corpse;
                GenExplosion.DoExplosion(corpse.Position, corpse.Map, this.GetRadiusForPawn(), PlagueDamageDefOf.MP_BlastInfectMortar, this.Caster, -1, -1f, null, null, null, null, null, 1f, 1, null, false, null, 0f, 1, 0f, false, null, null);
                corpse.Destroy();
            }
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            bool result = false;
            Corpse corpse = target.Thing as Corpse;
            CompRottable canRot = corpse.TryGetComp<CompRottable>();
            if (corpse != null)
            {
                bool legalSpawn = !corpse.InnerPawn.RaceProps.IsMechanoid 
                    && (corpse.InnerPawn.RaceProps.Humanlike 
                    || (corpse.InnerPawn.RaceProps.Insect && LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowInsectSpawns) 
                    || LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowAnimalSpawns)
                    && canRot != null && canRot.Stage != RotStage.Dessicated;
                if (legalSpawn)
                {
                    result = base.ValidateTarget(target, showMessages);
                }
            }

            if (!result && showMessages)
            {
                Messages.Message("MP_VPE_ValidFesteringBlastTarget".Translate(), corpse, MessageTypeDefOf.CautionInput, true);
            }

            return result;
        }
    }

    class Ability_LichdomVPE : VFECore.Abilities.Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);

            // Get the count of Mechanite Canisters on the map.
            Map map = Caster.Map;
            List<Thing> canisterList = map.listerThings.ThingsOfDef(PlagueThingDefOf.MP_MechaniteCanister);
            int canisterCount = GetCountOfThing(canisterList);

            // If we're not a lich, run check to apply main hediff.
            if (CasterPawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MP_VPE_Lichdom")) == null)
            {
                int spellCost = 100;

                // If we have enough, apply buff!
                if (canisterCount >= spellCost)
                {
                    RemoveNumberOfThing(canisterList, spellCost);
                    Hediff hediffMain = HediffMaker.MakeHediff(HediffDef.Named("MP_VPE_Lichdom"), CasterPawn);
                    hediffMain.Severity = 1.0f;
                    CasterPawn.health.AddHediff(hediffMain);

                    Hediff hediffSecondary = HediffMaker.MakeHediff(HediffDef.Named("MP_VPE_Lichdom_Talons"), CasterPawn);
                    hediffSecondary.Severity = 1.0f;
                    CasterPawn.health.AddHediff(hediffSecondary, CasterPawn.RaceProps.body.corePart);
                    
                    Messages.Message("MP_VPE_LichdomSuccess".Translate(CasterPawn.Name.ToStringShort), Caster, MessageTypeDefOf.PositiveEvent);
                }
                else
                {
                    Messages.Message("MP_VPE_LichdomFail".Translate(canisterCount), Caster, MessageTypeDefOf.RejectInput);
                }
            }
            else
            // If we ARE a lich, run check to apply resurrection hediff.
            {
                int spellCost = 50;

                // If we have enough, apply buff!
                if (canisterCount >= spellCost)
                {
                    RemoveNumberOfThing(canisterList, spellCost);
                    Hediff hediff;
                    hediff = CasterPawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MP_VPE_LichdomResurrect"));
                    if (hediff == null)
                    {
                        hediff = HediffMaker.MakeHediff(HediffDef.Named("MP_VPE_LichdomResurrect"), CasterPawn);
                        CasterPawn.health.AddHediff(hediff);
                    }
                    hediff.Severity = 1.0f;

                    Messages.Message("MP_VPE_LichdomSecondCastSuccess".Translate(CasterPawn.Name.ToStringShort), Caster, MessageTypeDefOf.PositiveEvent);
                }
                else
                {
                    Messages.Message("MP_VPE_LichdomSecondCastFail".Translate(canisterCount), Caster, MessageTypeDefOf.RejectInput);
                }
            }
        }

        // Given a list of things, adds up their stack sizes as a count.
        private int GetCountOfThing(List<Thing> thingList)
        {
            int count = 0;
            foreach (Thing thing in thingList)
            {
                count += thing.stackCount;
            }
            return count;
        }

        // Removes a specified number of a thing. If there's not enough, well that's too bad.
        private void RemoveNumberOfThing(List<Thing> thingList, int countToRemove)
        {
            // Make a copy of the list.
            List<Thing> copiedList = new List<Thing>(thingList);

            int itemsRemaining = countToRemove;
            foreach (Thing thing in copiedList)
            {
                if (thing.stackCount <= itemsRemaining)
                {
                    itemsRemaining -= thing.stackCount;
                    thing.Destroy();
                }
                else
                {
                    thing.stackCount -= itemsRemaining;
                    itemsRemaining = 0;
                }

                if (itemsRemaining <= 0) break;
            }
        }
    }

    class ModExtension_DamageWorkerDisintegrateBlast : DamageWorker_AddInjury
    {
        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageResult result)
        {
            // apply first so we can erase body on death
            Hediff hediff = HediffMaker.MakeHediff(HediffDef.Named("MP_VPE_DisintegrateDamage"), pawn);
            pawn.health.AddHediff(hediff);

            // then run base
            base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);
        }
    }

    class Ability_UpgradeVPE : VFECore.Abilities.Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);

            foreach (GlobalTargetInfo target in targets)
            {
                // sanity check, just in case?
                if (target.Thing != null && target.Thing is Pawn pawn && pawn.RaceProps.IsMechanoid && pawn.Faction == CasterPawn.Faction)
                {
                    // Check for decay or disintegration, so we can add the appropriate one to the spawned Drider.
                    string hediffToAdd = null;
                    if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MP_SpawnDecay")) != null)
                    {
                        hediffToAdd = "MP_SpawnDecay";
                    }
                    else if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MP_SpawnDecayRemove")) != null)
                    {
                        hediffToAdd = "MP_SpawnDecayRemove";
                    }

                    // Obliterate old pawn, replace with new pawn.
                    IntVec3 position = pawn.Position;
                    string pawnDefName = pawn.def.defName;
                    pawn.Destroy();

                    Lord newLord = LordMaker.MakeNewLord(CasterPawn.Faction, new LordJob_AssaultColony(CasterPawn.Faction, canKidnap: false, canTimeoutOrFlee: false, canSteal: false, useAvoidGridSmart: false, breachers: false, canPickUpOpportunisticWeapons: false), CasterPawn.Map, null);
                    Pawn spawnedPawn;

                    // Generate a different pawn based on the targeted pawn.
                    if (pawnDefName == "MP_Mech_Burster")
                    {
                        spawnedPawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDef.Named("MP_Mech_Drider"), CasterPawn.Faction, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, 1f, false, false, false, false, false, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null));
                    }
                    else 
                    { 
                        spawnedPawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDef.Named("MP_Mech_Drider_Constructable"), CasterPawn.Faction, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, 1f, false, false, false, false, false, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null));
                        spawnedPawn.needs.energy.CurLevel = 0f;
                        CasterPawn.psychicEntropy.OffsetPsyfocusDirectly(-1f);
                    }
                    
                    GenPlace.TryPlaceThing(spawnedPawn, position, CasterPawn.Map, ThingPlaceMode.Direct);
                    if (spawnedPawn.relations == null)
                    {
                        spawnedPawn.relations = new Pawn_RelationsTracker(spawnedPawn);
                    }
                    newLord.AddPawn(spawnedPawn);

                    if (hediffToAdd != null)
                    {
                        Hediff hediff = HediffMaker.MakeHediff(HediffDef.Named(hediffToAdd), spawnedPawn);
                        hediff.Severity = 0.01f;
                        spawnedPawn.health.AddHediff(hediff);
                    }
                }
            }
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            bool result = false;
            Pawn pawn = target.Pawn as Pawn;

            if (pawn != null && (pawn.def.defName == "MP_Mech_Burster" || pawn.def.defName == "MP_Mech_Burster_Constructable") && pawn.Faction == CasterPawn.Faction)
            {
                result = true;
            }

            if (!result && showMessages)
            {
                Messages.Message("MP_VPE_ValidHardwareUpgradeTarget".Translate(), pawn, MessageTypeDefOf.CautionInput, true);
            }

            return result;
        }
    }
}