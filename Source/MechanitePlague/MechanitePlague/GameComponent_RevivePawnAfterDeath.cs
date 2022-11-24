using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;

namespace MP_MechanitePlague
{
    // game components are automatically hooked up due to Reflection, just as a result of having GameComponent as a parent
    // poggers
    public class MP_RevivePawnAfterDelay : GameComponent
    {
        private int rareTickTimer = 250;
        private int rareTickUntil = 250;
        private int revivalTime = 50; // number of RareTicks to revive
        private List<PawnToRevive> deadPawns = new List<PawnToRevive>();

        public MP_RevivePawnAfterDelay(Game game)
        {
            // this space left blank intentionally
        }

        /*
        public override void FinalizeInit() // when loading a game, regardless of starting anew or loading old saves (probablyyyy)
        {
            base.FinalizeInit();
            Log.Warning("FI");
        }

        public override void StartedNewGame() // when starting a new game...
        {
            base.StartedNewGame();
            Log.Warning("SNG");
        }

        public override void LoadedGame() // when loading a game...
        {
            base.LoadedGame();
            Log.Warning("LG");
        }
        */

        public override void GameComponentTick() // every tick (normal)...
        {
            base.GameComponentTick();

            // simulate rare ticks so we do this processing less often
            rareTickUntil--;
            if (rareTickUntil <= 0) 
            { 
                rareTickUntil = rareTickTimer;
                foreach (PawnToRevive toRevive in deadPawns)
                {
                    toRevive.timeUntilRevive--;

                    try
                    {
                        // If the pawn's corpse is gone, :bwaah:
                        if (toRevive.pawn.Corpse == null)
                        {
                            Messages.Message("MP_ReviveFail".Translate(toRevive.pawn), null, MessageTypeDefOf.NegativeEvent, true);
                        }

                        if (toRevive.timeUntilRevive <= 0)
                        {
                            // If the pawn is in a grave or something, free them first.
                            if (!toRevive.pawn.Corpse.Spawned && toRevive.pawn.Corpse.StoringThing() != null)
                            {
                                GenSpawn.Spawn(toRevive.pawn.Corpse, toRevive.pawn.PositionHeld, toRevive.pawn.MapHeld, WipeMode.Vanish);
                            }
                            // Revive pawn.
                            ResurrectionUtility.Resurrect(toRevive.pawn);
                            Messages.Message("MP_ReviveSuccess".Translate(toRevive.pawn), toRevive.pawn, MessageTypeDefOf.PositiveEvent, true);
                        }
                    }
                    catch
                    { 
                        // blank catch to suppress errors
                    }
                }
                
                // do a removal here because otherwise enumeration errors
                deadPawns.RemoveAll(x => x.timeUntilRevive <= 0 || x.pawn.Corpse == null);
            }
        }

        // Adds a pawn to the list for revival reasons.
        public void AddToRevivalList(Pawn pawn)
        {
            bool foundDupe = false;
            foreach (PawnToRevive previousEntry in deadPawns) {
                if (previousEntry.pawn == pawn) {
                    foundDupe = true;
                    break;
                }
            }

            if (!foundDupe) {
                PawnToRevive pawnToRevive = new PawnToRevive(revivalTime, pawn);
                deadPawns.Add(pawnToRevive);
            }
        }

        // Exposes data for saving.
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref deadPawns, "deadPawns", LookMode.Deep);
        }
    }

    public class PawnToRevive : IExposable
    {
        public int timeUntilRevive;
        public Pawn pawn;

        public PawnToRevive()
        {
            // this space left blank intentionally
        }

        public PawnToRevive(int timeUntilRevive, Pawn pawn)
        {
            this.timeUntilRevive = timeUntilRevive;
            this.pawn = pawn;
        }

        public void ExposeData()
        {
            Scribe_References.Look<Pawn>(ref pawn, "pawn", true);
            Scribe_Values.Look<int>(ref timeUntilRevive, "timeUntilRevive", 1, false);
        }
    }
}
