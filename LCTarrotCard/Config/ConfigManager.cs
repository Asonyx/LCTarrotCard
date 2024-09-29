using BepInEx.Configuration;

namespace LCTarrotCard.Config {
    public static class ConfigManager {

        // General config
        public static ConfigEntry<float> DeckMultiplySpawnChance;
        public static ConfigEntry<int> DeckSize;
        public static ConfigEntry<bool> DisplayDrawnCard;
        public static ConfigEntry<bool> DisplayCardEffect;
        public static ConfigEntry<bool> DisplayCardLeft;

        // Card chance
        public static ConfigEntry<int> TowerCardChance;
        public static ConfigEntry<int> WheelCardChance;
        public static ConfigEntry<int> SunCardChance;
        public static ConfigEntry<int> MoonCardChance;
        public static ConfigEntry<int> DevilCardChance;
        public static ConfigEntry<int> HermitCardChance;
        public static ConfigEntry<int> HighPriestessCardChance;
        public static ConfigEntry<int> DeathCardChance;
        public static ConfigEntry<int> HangedManCardChance;
        public static ConfigEntry<int> FoolCardChance;
        
        // Effects chance
        
        // Tower
        public static ConfigEntry<int> TowerShipLeaveChance;
        public static ConfigEntry<int> TowerSecurityDoorsChance;
        public static ConfigEntry<int> TowerDoorsChance;
        public static ConfigEntry<int> TowerShipDoorChance;
        public static ConfigEntry<int> TowerBreakerChance;
        
        // Wheel
        public static ConfigEntry<int> WheelMultiplyValueChance;
        public static ConfigEntry<float> WheelBadMultiplyRange;
        public static ConfigEntry<float> WheelGoodMultiplyRange;
        public static ConfigEntry<int> WheelHealOrDamageChance;
        public static ConfigEntry<int> WheelHealOrDamageValue;
        
        // Sun/Moon
        public static ConfigEntry<int> SunHealOrDamageChance;
        public static ConfigEntry<float> SunMaxMultiplyRange;
        public static ConfigEntry<float> SunMinMultiplyRange;
        public static ConfigEntry<float> MoonMaxMultiplyRange;
        public static ConfigEntry<float> MoonMinMultiplyRange;
        
        // Devil
        public static ConfigEntry<int> DevilBlowChance;
        
        // Hermit
        public static ConfigEntry<int> HermitTpChance;
        
        // Death
        public static ConfigEntry<bool> DeathEnableCoilhead;
        public static ConfigEntry<bool> DeathEnableJester;
        public static ConfigEntry<bool> DeathEnableGhostGirl;
        public static ConfigEntry<bool> DeathEnableBracken;
        public static ConfigEntry<bool> DeathEnableDog;
        public static ConfigEntry<bool> DeathEnableGiant;
        public static ConfigEntry<bool> DeathEnableOldBird;
        public static ConfigEntry<bool> DeathEnableWorm;
        
        // Debug
        internal static ConfigEntry<bool> DebugModeSetting;
        
        public static void Init(ConfigFile configFile) {
            DeckMultiplySpawnChance = configFile.Bind("General", "DeckSpawnChance", 1f, 
                new ConfigDescription("Multiplier of the chance for the deck to spawn (default : 1)", new AcceptableValueRange<float>(0f, 100f)));
            DeckSize = configFile.Bind("General", "DeckSize", 10,
                new ConfigDescription("Size of the deck (default : 10 cards)", new AcceptableValueRange<int>(1, 100)));
            DisplayDrawnCard = configFile.Bind("General", "DisplayDrawnCard", false,
                new ConfigDescription("Display the drawn card (default : false)"));
            DisplayCardEffect = configFile.Bind("General", "DisplayCardEffect", false,
                new ConfigDescription("Display a hint about what the card did (default : false)"));
            DisplayCardLeft = configFile.Bind("General", "DisplayCardLeft", false,
                new ConfigDescription("Display the number of card left in the deck (default : false)"));
            
            InitCardConfig(configFile);
            DebugModeSetting = configFile.Bind("Debug", "DebugMode", false,
                new ConfigDescription("Enable debug mode (show debug logs in the console and make the Tarot spawn when hitting the backspace key, the one above the tab key) (default : false)"));
        }

        private static void InitCardConfig(ConfigFile configFile) {
            // Card chance
            TowerCardChance = configFile.Bind("Card", "TowerCardChance", 20,
                new ConfigDescription("Chance for the Tower card to be drawn (default : 20)",
                    new AcceptableValueRange<int>(0, 100)));
            WheelCardChance = configFile.Bind("Card", "WheelCardChance", 20,
                new ConfigDescription("Chance for the Wheel card to be drawn (default : 20)",
                    new AcceptableValueRange<int>(0, 100)));
            SunCardChance = configFile.Bind("Card", "SunCardChance", 5,
                new ConfigDescription("Chance for the Sun card to be drawn (default : 5)",
                    new AcceptableValueRange<int>(0, 100)));
            MoonCardChance = configFile.Bind("Card", "MoonCardChance", 5,
                new ConfigDescription("Chance for the Moon card to be drawn (default : 5)",
                    new AcceptableValueRange<int>(0, 100)));
            DevilCardChance = configFile.Bind("Card", "DevilCardChance", 10,
                new ConfigDescription("Chance for the Devil card to be drawn (default : 10)",
                    new AcceptableValueRange<int>(0, 100)));
            HermitCardChance = configFile.Bind("Card", "HermitCardChance", 10,
                new ConfigDescription("Chance for the Hermit card to be drawn (default : 10)",
                    new AcceptableValueRange<int>(0, 100)));
            HighPriestessCardChance = configFile.Bind("Card", "HighPriestessCardChance", 2,
                new ConfigDescription("Chance for the High Priestess card to be drawn (default : 2)",
                    new AcceptableValueRange<int>(0, 100)));
            DeathCardChance = configFile.Bind("Card", "DeathCardChance", 10,
                new ConfigDescription("Chance for the Death card to be drawn (default : 10)",
                    new AcceptableValueRange<int>(0, 100)));
            HangedManCardChance = configFile.Bind("Card", "HangedManCardChance", 1,
                new ConfigDescription("Chance for the Hanged Man card to be drawn (default : 1)",
                    new AcceptableValueRange<int>(0, 100)));
            FoolCardChance = configFile.Bind("Card", "FoolCardChance", 17,
                new ConfigDescription("Chance for the Fool card to be drawn (default : 17)",
                    new AcceptableValueRange<int>(0, 100)));
            
            // Effects chance
            // Tower
            TowerShipLeaveChance = configFile.Bind("TowerCard", "TowerShipLeaveChance", 1,
                new ConfigDescription("Chance for the Tower card to make the ship leave early (default : 1)",
                    new AcceptableValueRange<int>(0, 100)));
            TowerSecurityDoorsChance = configFile.Bind("TowerCard", "TowerSecurityDoorsChance", 20,
                new ConfigDescription("Chance for the Tower card to open/close random security doors (default : 20)",
                    new AcceptableValueRange<int>(0, 100)));
            TowerDoorsChance = configFile.Bind("TowerCard", "TowerDoorsChance", 20,
                new ConfigDescription("Chance for the Tower card to open/close random doors (default : 20)",
                    new AcceptableValueRange<int>(0, 100)));
            TowerShipDoorChance = configFile.Bind("TowerCard", "TowerShipDoorChance", 20,
                new ConfigDescription("Chance for the Tower card to invert ship door state (default : 20)",
                    new AcceptableValueRange<int>(0, 100)));
            TowerBreakerChance = configFile.Bind("TowerCard", "TowerBreakerChance", 20,
                new ConfigDescription("Chance for the Tower card to turn off the breaker (default : 20)",
                    new AcceptableValueRange<int>(0, 100)));
            
            // Wheel
            WheelMultiplyValueChance = configFile.Bind("WheelCard", "WheelMultiplyValueChance", 50,
                new ConfigDescription("Chance for the Wheel card to multiply some scrap value (default : 50)",
                    new AcceptableValueRange<int>(0, 100)));
            WheelBadMultiplyRange = configFile.Bind("WheelCard", "WheelBadMultiplyRange", 0.9f,
                new ConfigDescription("Range for the Wheel card to multiply some scrap value (bad outcome) (default : 0.9)",
                    new AcceptableValueRange<float>(0.1f, 1f)));
            WheelGoodMultiplyRange = configFile.Bind("WheelCard", "WheelGoodMultiplyRange", 1.1f,
                new ConfigDescription("Range for the Wheel card to multiply some scrap value (good outcome) (default : 1.1)",
                    new AcceptableValueRange<float>(1f, 2f)));
            WheelHealOrDamageChance = configFile.Bind("WheelCard", "WheelHealOrDamageChance", 50,
                new ConfigDescription("Chance for the Wheel card to heal or damage (default : 50)",
                    new AcceptableValueRange<int>(0, 100)));
            WheelHealOrDamageValue = configFile.Bind("WheelCard", "WheelHealOrDamageValue", 20,
                new ConfigDescription("Value for the Wheel card to heal or damage (default : 20)",
                    new AcceptableValueRange<int>(1, 100)));
            
            // Sun/Moon
            SunHealOrDamageChance = configFile.Bind("SunMoonCard", "SunHealOrDamageChance", 50,
                new ConfigDescription("Chance for the Sun card to heal or damage instead of multiplying scrap value (default : 50)",
                    new AcceptableValueRange<int>(0, 100)));
            SunMaxMultiplyRange = configFile.Bind("SunMoonCard", "SunMaxMultiplyRange", 1.5f,
                new ConfigDescription("Max range for the Sun card to multiply scrap value (default : 1.5)",
                    new AcceptableValueRange<float>(1f, 2f)));
            SunMinMultiplyRange = configFile.Bind("SunMoonCard", "SunMinMultiplyRange", 1.1f,
                new ConfigDescription("Min range for the Sun card to multiply scrap value (default : 1.1)",
                    new AcceptableValueRange<float>(1f, 2f)));
            MoonMaxMultiplyRange = configFile.Bind("SunMoonCard", "MoonMaxMultiplyRange", 0.9f,
                new ConfigDescription("Max range for the Moon card to multiply scrap value (default : 0.9)",
                    new AcceptableValueRange<float>(0f, 1f)));
            MoonMinMultiplyRange = configFile.Bind("SunMoonCard", "MoonMinMultiplyRange", 0.5f,
                new ConfigDescription("Min range for the Moon card to multiply scrap value (default : 0.5)",
                    new AcceptableValueRange<float>(0f, 1f)));
            
            // Devil
            DevilBlowChance = configFile.Bind("DevilCard", "DevilBlowChance", 50,
                new ConfigDescription("Chance for the Devil card to play the ghost breathe SFX (default : 50)",
                    new AcceptableValueRange<int>(0, 100)));
            
            // Hermit
            HermitTpChance = configFile.Bind("HermitCard", "HermitTpChance", 50,
                new ConfigDescription("Chance for the Hermit card to teleport you away (default : 50)",
                    new AcceptableValueRange<int>(0, 100)));
            
            // Death
            DeathEnableCoilhead = configFile.Bind("DeathCard", "DeathEnableCoilhead", true,
                new ConfigDescription("Enable Coilhead aggro on Death card (default : true)"));
            DeathEnableJester = configFile.Bind("DeathCard", "DeathEnableJester", true,
                new ConfigDescription("Enable Jester aggro on Death card (default : true)"));
            DeathEnableGhostGirl = configFile.Bind("DeathCard", "DeathEnableGhostGirl", true,
                new ConfigDescription("Enable Ghost aggro on Death card (default : true)"));
            DeathEnableBracken = configFile.Bind("DeathCard", "DeathEnableBracken", true,
                new ConfigDescription("Enable Bracken aggro on Death card (default : true)"));
            DeathEnableDog = configFile.Bind("DeathCard", "DeathEnableDog", true,
                new ConfigDescription("Enable Dog aggro on Death card (default : true)"));
            DeathEnableGiant = configFile.Bind("DeathCard", "DeathEnableGiant", true,
                new ConfigDescription("Enable Giant aggro on Death card (default : true)"));
            DeathEnableOldBird = configFile.Bind("DeathCard", "DeathEnableOldBird", true,
                new ConfigDescription("Enable Old Bird aggro on Death card (default : true)"));
            DeathEnableWorm = configFile.Bind("DeathCard", "DeathEnableWorm", true,
                new ConfigDescription("Enable Worm aggro on Death card (default : true)"));
        }

    }
}