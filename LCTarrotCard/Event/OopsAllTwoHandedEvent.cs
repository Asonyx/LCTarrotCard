namespace LCTarrotCard.Event {
    public class OopsAllTwoHandedEvent : IEvent {
        public string GetEventName() {
            return "Oops, All Two-Handed!";
        }
        public string ExecuteEvent() {
            throw new System.NotImplementedException();
        }
        public float GetEventDangerLevel() {
            return 0.8f;
        }

        public float GetEventWeight() {
            return 0.5f;
        }
        public float GetEventRange() {
            return 0.2f;
        }
    }
}