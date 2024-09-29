using System;
using System.Collections.Generic;
using LCTarrotCard.Config;
using Random = UnityEngine.Random;

namespace LCTarrotCard.Cards {
    
    public static  class AllCards {
        
        public static class BaseProbabilityTable {
            public static readonly int TowerCard = 20;
            public static readonly int WheelCard = 20;
            public static readonly int SunCard = 5;
            public static readonly int MoonCard = 5;
            public static readonly int DevilCard = 10;
            public static readonly int HermitCard = 10;
            public static readonly int HighPriestessCard = 2;
            public static readonly int DeathCard = 10;
            public static readonly int HangedManCard = 1;
            public static readonly int FoolCard = 17;
        }

        public static readonly Dictionary<Type, int> AllCardsWeighted = new Dictionary<Type, int>();
        private static int _totalWeight;
        
        internal static void Init() {
            AllCardsWeighted.Add(typeof(TowerCard), GetValidWeight(ConfigManager.TowerCardChance.Value, BaseProbabilityTable.TowerCard));
            AllCardsWeighted.Add(typeof(WheelCard), GetValidWeight(ConfigManager.WheelCardChance.Value, BaseProbabilityTable.WheelCard));
            AllCardsWeighted.Add(typeof(SunCard), GetValidWeight(ConfigManager.SunCardChance.Value, BaseProbabilityTable.SunCard));
            AllCardsWeighted.Add(typeof(MoonCard), GetValidWeight(ConfigManager.MoonCardChance.Value, BaseProbabilityTable.MoonCard));
            AllCardsWeighted.Add(typeof(DevilCard), GetValidWeight(ConfigManager.DevilCardChance.Value, BaseProbabilityTable.DevilCard));
            AllCardsWeighted.Add(typeof(HermitCard), GetValidWeight(ConfigManager.HermitCardChance.Value, BaseProbabilityTable.HermitCard));
            AllCardsWeighted.Add(typeof(HighPriestessCard), GetValidWeight(ConfigManager.HighPriestessCardChance.Value, BaseProbabilityTable.HighPriestessCard));
            AllCardsWeighted.Add(typeof(DeathCard), GetValidWeight(ConfigManager.DeathCardChance.Value, BaseProbabilityTable.DeathCard));
            AllCardsWeighted.Add(typeof(HangedManCard), GetValidWeight(ConfigManager.HangedManCardChance.Value, BaseProbabilityTable.HangedManCard));
            AllCardsWeighted.Add(typeof(FoolCard), GetValidWeight(ConfigManager.FoolCardChance.Value, BaseProbabilityTable.FoolCard));
            RecalculateTotalWeight();
        }
        
        private static int GetValidWeight(int weight, int defaultWeight) {
            return weight >= 0 && weight <= 100 ? weight : defaultWeight;
        }
        
        public static void RegisterCard(Type cardType, int weight) {
            if (!typeof(Card).IsAssignableFrom(cardType)) {
                PluginLogger.Warning("Trying to register a non-card type (type : " + cardType.Name + ")");
                return;
            }
            
            if (AllCardsWeighted.ContainsKey(cardType)) {
                PluginLogger.Warning("Trying to register a card that is already registered (type : " + cardType.Name + ")");
                return;
            }
            AllCardsWeighted.Add(cardType, weight);
            RecalculateTotalWeight();
        }
        
        public static List<Type> GetAllCardsAsList() {
            return new List<Type>(AllCardsWeighted.Keys);
        }
        
        private static void RecalculateTotalWeight() {
            _totalWeight = 0;
            foreach (int weight in AllCardsWeighted.Values) {
                _totalWeight += weight;
            }
        }

        public static Type PullRandomCard() {

            int currentWeight = 0;
            int randomNumber = Random.Range(0, _totalWeight + 1);
            Type cardChoose = typeof(object);
            foreach (KeyValuePair<Type, int> entry in AllCardsWeighted) {
                if (entry.Value + currentWeight >= randomNumber) {
                    cardChoose = entry.Key;
                    break;
                }
                currentWeight += entry.Value;
            }

            if (typeof(Card).IsAssignableFrom(cardChoose)) return cardChoose;
            
            PluginLogger.Warning("Trying to pull a card with a non-card type (type : " + cardChoose.Name + ")");
            return typeof(FoolCard);

        }
        
    }
    
}