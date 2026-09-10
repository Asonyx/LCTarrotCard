using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace LCTarrotCard.Event {
    public static class EventManager {

        private static ICardHandler[] cardEventHandlers;
        
        private static Dictionary<Type, uint> nativeCardEventHandlerMap = new Dictionary<Type, uint>();

        public static bool IsEventSystemEnabled() {
            return cardEventHandlers != null && cardEventHandlers.Length > 0;
        }
        
        public static void RegisterCardEventHandler(ICardHandler cardEventHandler) {
            if (cardEventHandlers == null) {
                cardEventHandlers = new ICardHandler[] { cardEventHandler };
            } else {
                Array.Resize(ref cardEventHandlers, cardEventHandlers.Length + 1);
                cardEventHandlers[cardEventHandlers.Length - 1] = cardEventHandler;
            }
            UpdateMapping();
        }

        [CanBeNull]
        public static ICardHandler GetHandler(EventCard card) {
            if (cardEventHandlers.Length <= 0) return null;
            Type cardType = card.GetType();
            return nativeCardEventHandlerMap.TryGetValue(cardType, out uint handlerIndex) ? cardEventHandlers[handlerIndex] : cardEventHandlers[0];
        }
        
        private static void UpdateMapping() {
            nativeCardEventHandlerMap.Clear();
            for (uint i = 0; i < cardEventHandlers.Length; i++) {
                Type[] handledCards = cardEventHandlers[i].GetNativelyHandledCards();
                foreach (Type cardType in handledCards) {
                    if (nativeCardEventHandlerMap.ContainsKey(cardType)) continue;
                    nativeCardEventHandlerMap.Add(cardType, i);
                }
            }
        }
        
    }
}