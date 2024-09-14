using System;
using System.Collections.Generic;
using System.Linq;

namespace LCTarrotCard.Util {
    public enum Planet : int {
        EXPERIMENTATION = 0,
        ASSURANCE = 1,
        VOW = 2,
        GORDION = 3,
        MARCH = 4,
        ADAMANCE = 5,
        REND = 6,
        DINE = 7,
        OFFENSE = 8,
        TITAN = 9,
        ARTIFICE = 10,
        LIQUIDATION = 11,
        EMBRION = 12
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

        public int GetRarityFor(int planet) {
            return rarities.Values.ToList()[planet];
        }

        public void SetValues(int experimentation = -1, int assurance = -1, int vow = -1, int march = -1, int offense = -1, int adamance = -1, 
            int rend = -1, int dine = -1, int titan = -1, int artifice = -1, int embrion = -1) {
            rarities[Planet.EXPERIMENTATION] = experimentation;
            rarities[Planet.ASSURANCE] = assurance;
            rarities[Planet.VOW] = vow;
            rarities[Planet.MARCH] = march;
            rarities[Planet.OFFENSE] = offense;
            rarities[Planet.ADAMANCE] = adamance;
            rarities[Planet.REND] = rend;
            rarities[Planet.DINE] = dine;
            rarities[Planet.TITAN] = titan;
            rarities[Planet.ARTIFICE] = artifice;
            rarities[Planet.EMBRION] = embrion;
        }

        public void CopyRaritiesFrom(SpawnRarity other) {
            rarities[Planet.EXPERIMENTATION] = other.rarities[Planet.EXPERIMENTATION];
            rarities[Planet.ASSURANCE] = other.rarities[Planet.ASSURANCE];
            rarities[Planet.VOW] = other.rarities[Planet.VOW];
            rarities[Planet.MARCH] = other.rarities[Planet.MARCH];
            rarities[Planet.OFFENSE] = other.rarities[Planet.OFFENSE];
            rarities[Planet.ADAMANCE] = other.rarities[Planet.ADAMANCE];
            rarities[Planet.REND] = other.rarities[Planet.REND];
            rarities[Planet.DINE] = other.rarities[Planet.DINE];
            rarities[Planet.TITAN] = other.rarities[Planet.TITAN];
            rarities[Planet.ARTIFICE] = other.rarities[Planet.ARTIFICE];
            rarities[Planet.EMBRION] = other.rarities[Planet.EMBRION];
        }

        public SpawnableItemWithRarity GetSpawnableItemForPlanet(Planet planet) {
            return new SpawnableItemWithRarity {
                spawnableItem = this.item,
                rarity = accessRaritySafe((int)planet)
            };
        }

        public SpawnableItemWithRarity GetSpawnableItemForPlanet(int planet) {
            return new SpawnableItemWithRarity {
                spawnableItem = this.item,
                rarity = accessRaritySafe(planet)
            };
        }

        private int accessRaritySafe(int planet) {
            return rarities.Values.ToList<int>().Count >= planet ? defaultValue : rarities.Values.ToList<int>()[planet];
        }

        public void ApplySpawnRarity(SelectableLevel level) {
            level.spawnableScrap.Add(this.GetSpawnableItemForPlanet(level.levelID));
        }
    }
}