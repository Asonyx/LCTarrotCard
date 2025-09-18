using GameNetcodeStuff;

namespace LCTarrotCard.Event {
    public class SpawnFakeTraps : IEvent {
        public string GetEventName() {
            return "Spawn Fake Traps";
        }
        public string ExecuteEvent(PlayerControllerB targetPlayer) {
            throw new System.NotImplementedException();
        }
        public float GetEventDangerLevel() {
            return 0.5f;
        }
        public float GetEventWeight() {
            return 0.4f;
        }
        public float GetEventRange() {
            return 0.2f;
        }
    }
}