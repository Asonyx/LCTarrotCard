using System.Collections.Generic;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace LCTarrotCard.Event {
    public class SpawnMonsterEvent : IEvent {
        public string GetEventName() {
            return "Monster Wave";
        }
        public string ExecuteEvent(PlayerControllerB targetPlayer) {
            float addedPower = 0f;
            float targetPower = RoundManager.Instance.currentMaxInsidePower - RoundManager.Instance.currentEnemyPower;
            
            List<int> weights = new List<int>();
            int totalWeight = 0;
            foreach (SpawnableEnemyWithRarity enemy in RoundManager.Instance.currentLevel.Enemies) {
                weights.Add(enemy.rarity);
                totalWeight += enemy.rarity;
            }
            
            while (addedPower < targetPower) {
                int randomWeightedIndex = Random.Range(0, totalWeight);
                EnemyType randomEnemy = null;
                int cumulativeWeight = 0;
                foreach (int i in weights) {
                    cumulativeWeight += i;
                    if (randomWeightedIndex >= cumulativeWeight) continue;
                    randomEnemy = RoundManager.Instance.currentLevel.Enemies[weights.IndexOf(i)].enemyType;
                    break;
                }
                if (randomEnemy == null) continue;
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
            return 0.5f;
        }
        public float GetEventRange() {
            return 0.15f;
        }
    }
}