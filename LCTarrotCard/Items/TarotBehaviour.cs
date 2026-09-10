using System;
using System.Collections.Generic;
using LCTarrotCard.Cards;
using LCTarrotCard.Config;
using LCTarrotCard.Util;
using UnityEngine;

namespace LCTarrotCard.Items {
    public class TarotBehaviour : TarotDeckBaseBehaviour {
        
        // For testing one card at a time, not meant to be used in production
        private static Dictionary<Type, int> testDeck = new Dictionary<Type, int>() {
            { typeof(TowerCard), 100 }, { typeof(FoolCard), 1 } 
        };
        
        public override bool ShouldDrawFoolWhenCantDraw() {
            return true;
        }

        public override Material GetCardBackMaterial() {
            return null; // Will set the default material when null
        }

        public override Dictionary<Type, int> GetCardSet() {
            return DebugOnly.OnlyIfTesting(testDeck, AllCards.AllCardsWeighted);
        }

        public override void Start() {
            base.Start();
            if (!IsOwner) return;
            if (ConfigManager.DeckSize.Value > 0 && ConfigManager.DeckSize.Value <= 100) {
                cardLeft = ConfigManager.DeckSize.Value;
            }
            SetNumberOfCardsServerRpc(cardLeft);
        }
    }
}