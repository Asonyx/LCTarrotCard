using GameNetcodeStuff;
using LCTarrotCard.Cards;
using UnityEngine;

namespace LCTarrotCard.Event {
    public abstract class EventCard : Card {
        
        protected EventCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }

        public abstract float GetDangerLevel();

        public abstract float GetSoftmaxTemperature();
        
        protected bool TriggerEvent(PlayerControllerB playerWhoDrew) {
            if (!EventManager.IsEventSystemEnabled()) {
                return false;
            }
            ICardHandler handler = EventManager.GetHandler(this);
            if (handler == null) return false;
            handler.HandleCard(this, playerWhoDrew);
            return true;
        }
    }
}