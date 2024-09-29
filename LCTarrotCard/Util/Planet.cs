using System;
using System.Collections.Generic;
using System.Linq;

namespace LCTarrotCard.Util {
    public enum Planet {
        Experimentation = 0,
        Assurance = 1,
        Vow = 2,
        Gordion = 3,
        March = 4,
        Adamance = 5,
        Rend = 6,
        Dine = 7,
        Offense = 8,
        Titan = 9,
        Artifice = 10,
        Liquidation = 11,
        Embrion = 12
    }

    public class SpawnRarity {

        private readonly Dictionary<Planet, int> rarities = new Dictionary<Planet, int>();
        private readonly Item item;
        private int defaultValue;

        public SpawnRarity(int baseValue, Item itemIn) {
            this.item = itemIn;
            defaultValue = baseValue;
            foreach (Planet planet in Enum.GetValues(typeof(Planet))) {
                rarities.Add(planet, baseValue);
            }
        }

        public int GetRarityFor(Planet planet) {
            return rarities[planet];
        }
        
        public void MultiplyAllRarities(float multiplier) {
            foreach (Planet planet in Enum.GetValues(typeof(Planet))) {
                rarities[planet] = (int) (rarities[planet] * multiplier);
            }
        }

        public void SetValues(int experimentation = -1, int assurance = -1, int vow = -1, int march = -1, int offense = -1, int adamance = -1, 
            int rend = -1, int dine = -1, int titan = -1, int artifice = -1, int embrion = -1) {
            rarities[Planet.Experimentation] = experimentation;
            rarities[Planet.Assurance] = assurance;
            rarities[Planet.Vow] = vow;
            rarities[Planet.March] = march;
            rarities[Planet.Offense] = offense;
            rarities[Planet.Adamance] = adamance;
            rarities[Planet.Rend] = rend;
            rarities[Planet.Dine] = dine;
            rarities[Planet.Titan] = titan;
            rarities[Planet.Artifice] = artifice;
            rarities[Planet.Embrion] = embrion;
        }

        public void CopyRaritiesFrom(SpawnRarity other) {
            rarities[Planet.Experimentation] = other.rarities[Planet.Experimentation];
            rarities[Planet.Assurance] = other.rarities[Planet.Assurance];
            rarities[Planet.Vow] = other.rarities[Planet.Vow];
            rarities[Planet.March] = other.rarities[Planet.March];
            rarities[Planet.Offense] = other.rarities[Planet.Offense];
            rarities[Planet.Adamance] = other.rarities[Planet.Adamance];
            rarities[Planet.Rend] = other.rarities[Planet.Rend];
            rarities[Planet.Dine] = other.rarities[Planet.Dine];
            rarities[Planet.Titan] = other.rarities[Planet.Titan];
            rarities[Planet.Artifice] = other.rarities[Planet.Artifice];
            rarities[Planet.Embrion] = other.rarities[Planet.Embrion];
        }

        public SpawnableItemWithRarity GetSpawnableItemForPlanet(Planet planet) {
            return new SpawnableItemWithRarity {
                spawnableItem = item,
                rarity = AccessRaritySafe((int)planet)
            };
        }

        public SpawnableItemWithRarity GetSpawnableItemForPlanet(int planet) {
            return new SpawnableItemWithRarity {
                spawnableItem = this.item,
                rarity = AccessRaritySafe(planet)
            };
        }

        private int AccessRaritySafe(int planet) {
            try {
                return rarities.Values.ToList()[planet];
            }
            catch (Exception) {
                // ignored
            }

            return defaultValue;
        }

        public void ApplySpawnRarity(SelectableLevel level) {
            level.spawnableScrap.Add(GetSpawnableItemForPlanet(level.levelID));
        }
    }
}