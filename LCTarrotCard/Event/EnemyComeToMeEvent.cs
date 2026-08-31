using GameNetcodeStuff;

namespace LCTarrotCard.Event {
    public class EnemyComeToMeEvent : IEvent {
        public string GetEventName() {
            return "Enemy Come To Me";
        }
        public string ExecuteEvent(PlayerControllerB targetPlayer) {
            foreach (EnemyAI spawnedEnemy in RoundManager.Instance.SpawnedEnemies) {
                if (spawnedEnemy && !spawnedEnemy.isEnemyDead && spawnedEnemy.isOutside != targetPlayer.isInsideFactory) {
                    spawnedEnemy.SetDestinationToPosition(targetPlayer.transform.position);
                }
            }
            return "Ding Dong !";
        }
        public float GetEventDangerLevel() {
            return 0.8f;
        }
        public float GetEventWeight() {
            return 0.3f;
        }
        public float GetEventRange() {
            return 0.1f;
        }
    }
}