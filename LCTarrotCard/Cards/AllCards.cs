using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace LCTarrotCard.Cards {
    
    public static  class AllCards {
        
        public static class ProbabilityTable {
            public static int TowerCard = 20;
            public static int WheelCard = 20;
            public static int SunCard = 5;
            public static int MoonCard = 5;
            public static int DevilCard = 10;
            public static int HermitCard = 10;
            public static int HighPriestessCard = 2;
            public static int DeathCard = 10;
            public static int HangedManCard = 1;
            public static int FoolCard = 17;
        }

        public static readonly Dictionary<Type, int> AllCardsWeighted = new Dictionary<Type, int>();
        private static int _totalWeight;
        
        internal static void Init() {
            AllCardsWeighted.Add(typeof(TowerCard), ProbabilityTable.TowerCard);
            AllCardsWeighted.Add(typeof(WheelCard), ProbabilityTable.WheelCard);
            AllCardsWeighted.Add(typeof(SunCard), ProbabilityTable.SunCard);
            AllCardsWeighted.Add(typeof(MoonCard), ProbabilityTable.MoonCard);
            AllCardsWeighted.Add(typeof(DevilCard), ProbabilityTable.DevilCard);
            AllCardsWeighted.Add(typeof(HermitCard), ProbabilityTable.HermitCard);
            AllCardsWeighted.Add(typeof(HighPriestessCard), ProbabilityTable.HighPriestessCard);
            AllCardsWeighted.Add(typeof(DeathCard), ProbabilityTable.DeathCard);
            AllCardsWeighted.Add(typeof(HangedManCard), ProbabilityTable.HangedManCard);
            AllCardsWeighted.Add(typeof(FoolCard), ProbabilityTable.FoolCard);
            RecalculateTotalWeight();
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
            
            PluginLogger.Debug(cardChoose.Name);

            if (typeof(Card).IsAssignableFrom(cardChoose)) return cardChoose;
            
            PluginLogger.Warning("Trying to pull a card with a non-card type (type : " + cardChoose.Name + ")");
            return typeof(FoolCard);

        }
        
    }
    
}