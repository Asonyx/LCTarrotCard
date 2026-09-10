using System;
using GameNetcodeStuff;

namespace LCTarrotCard.Event {
    public interface ICardHandler {

        /// <summary>
        /// Used to handle a card's effect using the event system
        /// </summary>
        /// <param name="card">The card that was drawn</param>
        /// <param name="playerWhoDrew">The player who drew the card</param>
        void HandleCard(EventCard card, PlayerControllerB playerWhoDrew);

        /// <summary>
        /// Should return the cards that are to be handled in priority by this event handler
        /// </summary>
        /// <returns>The list of cards type</returns>
        Type[] GetNativelyHandledCards();

    }
}