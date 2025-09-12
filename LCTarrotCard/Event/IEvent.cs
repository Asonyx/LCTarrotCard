namespace LCTarrotCard.Event {
    public interface IEvent {
        /// <returns>The name of the event</returns>
        string GetEventName();
        
        /// <summary>
        /// Execute the event.
        /// WARNING : This runs server-side only, use RPC to execute client-side code
        /// </summary>
        /// <returns>A quick and generic description of what happened</returns>
        string ExecuteEvent();
        
        /// <returns>The danger level of the effect (0 = good, 0.5 = neutral, 1 = bad)</returns>
        float GetEventDangerLevel();

        /// <returns>The weight of the event compared to other events in the same range</returns>
        float GetEventWeight();
        
        /// <returns>The range of danger levels this event can be chosen for</returns>
        float GetEventRange();
    }
}