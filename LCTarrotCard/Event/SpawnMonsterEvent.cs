using Unity.Netcode;
using UnityEngine;

namespace LCTarrotCard.Event {
    public class SpawnMonsterEvent : IEvent {
        public string GetEventName() {
            return "Monster Wave";
        }
        public string ExecuteEvent() {
            int nbToSpawnInside = Random.Range(1, 3);
            int nbToSpawnOutside = Random.Range(1, 3);
            for (int i = 0; i < nbToSpawnInside; i++) {
                EnemyType randomEnemy = RoundManager.Instance.currentLevel.
                    Enemies[Random.Range(0, RoundManager.Instance.currentLevel.Enemies.Count)].enemyType;
                Vector3 randomVentPos = RoundManager.Instance.
                    allEnemyVents[Random.Range(0, RoundManager.Instance.allEnemyVents.Length)].transform.position;
                GameObject enemyObj = Object.Instantiate(randomEnemy.enemyPrefab, randomVentPos, Quaternion.identity);
                enemyObj.GetComponentInChildren<NetworkObject>().Spawn(true);
                RoundManager.Instance.SpawnedEnemies.Add(enemyObj.GetComponent<EnemyAI>());
            }
            return "Meet your new friends!";
        }
        public float GetEventDangerLevel() {
            return 0.6f;
        }

        public float GetEventWeight() {
            throw new System.NotImplementedException();
        }
        public float GetEventRange() {
            throw new System.NotImplementedException();
        }
    }
}