using System;
using System.Collections.Generic;
using LCTarrotCard.Cards;
using LCTarrotCard.Config;
using UnityEngine;

namespace LCTarrotCard.Items {
    public class TarotBehaviour : TarotDeckBaseBehaviour {
        
        public override bool ShouldDrawFoolWhenCantDraw() {
            return true;
        }

        public override Material GetCardBackMaterial() {
            return null; // Will set the default material when null
        }

        public override Dictionary<Type, int> GetCardSet() {
            return AllCards.AllCardsWeighted;
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