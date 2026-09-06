using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LCTarrotCard.Config;
using LCTarrotCard.Util;
using UnityEngine.SceneManagement;
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
        }
        
        private static int GetValidWeight(int weight, int defaultWeight) {
            return weight >= 0 && weight <= 100 ? weight : defaultWeight;
        }
        
        /// <summary>
        /// Use this to register a custom card from another mod.
        /// Use this in your mod's awake/start method
        /// </summary>
        /// <param name="cardType">The type of your card, do typeof(YourCardClass)</param>
        /// <param name="weight">The probability of the card to be pulled</param>
        [UsedImplicitly]
        public static void RegisterCard(Type cardType, int weight) {
            if (!typeof(Card).IsAssignableFrom(cardType)) {
                PluginLogger.Warning("Trying to register a non-card type (type : " + cardType.Name + ")");
                return;
            }
            
            if (AllCardsWeighted.ContainsKey(cardType)) {
                PluginLogger.Warning("Trying to register a card that is already registered (type : " + cardType.Name + ")");
                return;
            }
            PluginLogger.Debug("Adding a new card to the standard deck (type : " + cardType.Name + "; new size : " + AllCardsWeighted.Count + "; total weight :  " + CalculateTotalWeight(AllCardsWeighted) + ")");
            AllCardsWeighted.Add(cardType, weight);
        }
        
        /// <summary>
        /// Convert the cardset of paired card type and weight to just a list of the card type
        /// </summary>
        /// <param name="cardSet">The card set following this pattern : {CardClass.GetType() : weight}</param>
        /// <returns>A list of the card type</returns>
        public static List<Type> GetAllCardsAsList(Dictionary<Type, int> cardSet) {
            return new List<Type>(cardSet.Keys);
        }
        
        /// <summary>
        /// Compute the sum of all the weights
        /// </summary>
        /// <param name="cardSet">The card set following this pattern : {CardClass.GetType() : weight}</param>
        /// <returns>The total weight</returns>
        private static int CalculateTotalWeight(Dictionary<Type, int> cardSet) {
            return cardSet.Values.Sum();
        }

        /// <summary>
        /// Pull a random card from the deck with a probability according to the weight
        /// </summary>
        /// <param name="cardSet">The card set following this pattern : {CardClass.GetType() : weight}</param>
        /// <returns>The randomly pulled card</returns>
        public static Type PullRandomCard(Dictionary<Type, int> cardSet) {
            
            PluginLogger.Debug("Pulling a card from the deck :\n" + Helper.DictionnaryToString(cardSet));
            
            int currentWeight = 0;
            int randomNumber = Random.Range(0, CalculateTotalWeight(cardSet) + 1);
            Type cardChoose = typeof(object);
            foreach (KeyValuePair<Type, int> entry in cardSet) {
                if (entry.Value + currentWeight >= randomNumber) {
                    cardChoose = entry.Key;
                    break;
                }
                currentWeight += entry.Value;
            }
            
            PluginLogger.Debug("Pulled card : " + cardChoose);

            if (typeof(Card).IsAssignableFrom(cardChoose)) return cardChoose;
            
            PluginLogger.Warning("Trying to pull a card with a non-card type (type : " + cardChoose.Name + ")");
            return typeof(FoolCard);

        }
        
    }
    
}