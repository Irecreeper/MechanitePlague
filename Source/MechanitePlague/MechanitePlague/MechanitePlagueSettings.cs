using RimWorld;
using UnityEngine;
using Verse;

namespace MP_MechanitePlague
{
    //This is basically like... the list of statistics that can be changed by the user.
    public class MechPlagueSettings : ModSettings
    {
        //The available settings.
        public IntRange intervalSpawnCount = new IntRange(1, 2); //The range of Bursters that can spawn from each corpse. (default: 1-2) (max: 1-5)
        public bool allowInsectSpawns = false; //Can Bursters spawn from insects? (default: false)
        public bool allowAnimalSpawns = false; //Can Bursters spawn from animals in general? (default: false)
        public bool doDownedDamageOverTime = true; //Do downed pawns take damage over time? (default: true)

        //The part that writes settings to file.
        public override void ExposeData()
        {
            int min = this.intervalSpawnCount.min;
            int max = this.intervalSpawnCount.max;
            Scribe_Values.Look<int>(ref min, "MP_BursterSpawnMin", 1, false);
            Scribe_Values.Look<int>(ref max, "MP_BursterSpawnMax", 2, false);
            this.intervalSpawnCount = new IntRange(min, max);

            Scribe_Values.Look<bool>(ref this.allowInsectSpawns, "MP_AllowInsectSpawns", false, false);
            Scribe_Values.Look<bool>(ref this.allowAnimalSpawns, "MP_AllowAnimalSpawns", false, false);
            Scribe_Values.Look<bool>(ref this.doDownedDamageOverTime, "MP_DoDownedDamageOverTime", true, false);

            base.ExposeData();
        }

        //The probably important part that allows us to set our settings through GUI.
        public void DoSettingsWindowContents(Rect inRect)
        {
            //Begin Settings
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            //Burster Spawn Amounts
            listingStandard.Label("Bursters spawned per Plagued Pawn:", -1f, "When a pawn perishes with the Mechanite Plague, how many Bursters will spawn? Does not affect psycasts.");
            listingStandard.IntRange(ref this.intervalSpawnCount, 1, 5);

            //Burster Specific Spawns
            listingStandard.CheckboxLabeled("Allow Bursters to spawn from Insects", ref this.allowInsectSpawns, "This setting, if enabled, will allow Bursters to spawn from Insectoid attackers.");
            listingStandard.CheckboxLabeled("Allow Bursters to spawn from Animals", ref this.allowAnimalSpawns, "This setting, if enabled, will allow Bursters to spawn from all animals, including insects. Probably overpowered. Still won't work on mechanoids.");

            //Damage Over Time
            listingStandard.CheckboxLabeled("Enable Mechanite Plague damage over time to downed Pawns", ref this.doDownedDamageOverTime, "This setting, if enabled, will cause downed, non-tended pawns with the Mechanite Plague to take damage over time.");

            //End Settings
            listingStandard.End();
        }
    }

    //This is like, the mod itself...? But really, it loads settings.
    public class MechPlague : Mod
    {
        //A reference to our settings.
        public static MechPlagueSettings settings;

        //Resolves the refernce to the settings.
        public MechPlague(ModContentPack content) : base(content)
        {
            MechPlague.settings = GetSettings<MechPlagueSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            MechPlague.settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "The Mechanite Plague";
        }
    }
}
