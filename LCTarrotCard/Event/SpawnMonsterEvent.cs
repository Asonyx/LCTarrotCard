using Unity.Netcode;
using UnityEngine;

namespace LCTarrotCard.Event {
    public class SpawnMonsterEvent : IEvent {
        public string GetEventName() {
            return "Monster Wave";
        }
        public string ExecuteEvent() {
            float addedPower = 0f;
            float targetPower = RoundManager.Instance.currentMaxInsidePower - RoundManager.Instance.currentEnemyPower;
            while (addedPower < targetPower) {
                EnemyType randomEnemy = RoundManager.Instance.currentLevel.
                    Enemies[Random.Range(0, RoundManager.Instance.currentLevel.Enemies.Count)].enemyType;
                addedPower += randomEnemy.PowerLevel;
                Vector3 randomVentPos = RoundManager.Instance.
                    allEnemyVents[Random.Range(0, RoundManager.Instance.allEnemyVents.Length)].transform.position;
                GameObject enemyObj = Object.Instantiate(randomEnemy.enemyPrefab, randomVentPos, Quaternion.identity);
                enemyObj.GetComponentInChildren<NetworkObject>().Spawn(true);
                RoundManager.Instance.SpawnedEnemies.Add(enemyObj.GetComponent<EnemyAI>());
            }
            return "Meet your new friends!";
        }
        public float GetEventDangerLevel() {
            return 0.7f;
        }

        public float GetEventWeight() {
            return 0.8f;
        }
        public float GetEventRange() {
            return 0.15f;
        }
    }
}