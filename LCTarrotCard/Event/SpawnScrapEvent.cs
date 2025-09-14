using System.Collections.Generic;
using GameNetcodeStuff;
using LCTarrotCard.Ressource;
using Unity.Netcode;
using UnityEngine;

namespace LCTarrotCard.Event {
    public class SpawnScrapEvent : IEvent {
        public string GetEventName() {
            return "Spawn Scrap";
        }
        public string ExecuteEvent(PlayerControllerB targetPlayer) {
            List<int> weights = new List<int>();
            int totalWeight = 0;
            foreach (SpawnableItemWithRarity item in RoundManager.Instance.currentLevel.spawnableScrap) {
                if (item.spawnableItem == Assets.TarotItem) continue;
                weights.Add(item.rarity);
                totalWeight += item.rarity;
            }
            
            if (totalWeight == 0) return "No scrap could be spawned.";
            
            for (int i = 0; i < Random.Range(1, 5); i++) {
                int cumulativeWeight = 0;
                int randomWeight = Random.Range(0, totalWeight);
                Item item = null;
                for (int j = 0; j < weights.Count; j++) {
                    cumulativeWeight += weights[j];
                    if (randomWeight >= cumulativeWeight) continue;
                    item = RoundManager.Instance.currentLevel.spawnableScrap[j].spawnableItem;
                    break;
                }
                if (item == null) continue;
                
                GameObject obj = Object.Instantiate(item.spawnPrefab, targetPlayer.transform.position + 
                                        new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)), Quaternion.identity);
                GrabbableObject component = obj.GetComponent<GrabbableObject>();
                component.transform.rotation = Quaternion.Euler(component.itemProperties.restingRotation);
                component.fallTime = 0f;
                component.scrapValue = 1;
                NetworkObject no = obj.GetComponent<NetworkObject>();
                no.Spawn();
                component.FallToGround(false, true);
                // TODO : sync value
            }
            
            return "Some scrap has spawned around you.";
        }
        
        public float GetEventDangerLevel() {
            return 0.25f;
        }

        public float GetEventWeight() {
            return 0.4f;
        }
        public float GetEventRange() {
            return 0.15f;
        }
    }
}