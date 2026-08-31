using System;
using JetBrains.Annotations;
using LCTarrotCard.Util;

namespace LCTarrotCard.Event {
    public static class EventManager {
        // List of events registered by danger level (0 is good, 0.5 is neutral, 1 is bad)
        // Also larger range, means the event can be chosen in a larger range of danger level
        // Weight is the probability of the event being chosen compared to other events in the same range
        private static ProbabilityRangedList<IEvent> eventList = new ProbabilityRangedList<IEvent>();
        
        /// <summary>
        /// Register an event to be used by the tarot card.
        /// Use this in your mod's awake/start method if you want to add custom events.
        /// </summary>
        /// <param name="eventToRegister">The event to be added</param>
        public static void RegisterEvent(IEvent eventToRegister) {
            if (eventToRegister.GetEventDangerLevel() < 0f || eventToRegister.GetEventDangerLevel() > 1f) {
                PluginLogger.Warning("Trying to register an event with an invalid danger level (level : " + eventToRegister.GetEventDangerLevel() + ")");
                return;
            }
            eventList.Add(eventToRegister, eventToRegister.GetEventRange(), eventToRegister.GetEventWeight(), eventToRegister.GetEventDangerLevel());
        }

        public static void RegisterEvents(IEvent[] eventsToRegister) {
            foreach (IEvent eventToRegister in eventsToRegister) {
                RegisterEvent(eventToRegister);
            }
        }

        internal static void Init() {
            RegisterEvents(new IEvent[] {
                new SpawnMonsterEvent(), new SpawnScrapEvent(), new EnemyComeToMeEvent(), 
                new MoreTrapEvent(), new SpawnFakeTrapsEvent(), new OopsAllTwoHandedEvent()
            });
        }

        /// <summary>
        /// Get a random event based on the given danger level.
        /// Meant to be called server-side only.
        /// </summary>
        /// <param name="dangerLevel">0 is good, 0.5 is neutral, 1 is bad</param>
        /// <param name="softmaxTemp">Softmax temperature to use when choosing the event. Higher values mean more randomness,
        /// lower values mean more deterministic. Default is 1.</param>
        /// <returns>A random event corresponding to the danger level</returns>
        [CanBeNull]
        public static IEvent GetRandomEvent(float dangerLevel, float softmaxTemp = 1f) {
            IEvent eventToReturn = null;
            try {
                eventToReturn = eventList.GetRandom(dangerLevel, softmaxTemp);
            } catch (Exception e) {
                PluginLogger.Error("Error while getting a random event : " + e.Message);
            }

            return eventToReturn;
        }
        
        
    }
}