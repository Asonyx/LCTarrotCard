using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace LCTarrotCard.Event {
    public class OopsAllTwoHandedEvent : IEvent {
        public string GetEventName() {
            return "Oops, All Two-Handed!";
        }
        public string ExecuteEvent(PlayerControllerB targetPlayer) {
            GrabbableObject[] objectsInScene = Object.FindObjectsOfType<GrabbableObject>();
            List<SpawnableItemWithRarity> spawnableItems = RoundManager.Instance.currentLevel.spawnableScrap.
                                                                        Where(rarity => rarity.spawnableItem.twoHanded).ToList();
            
            List<NetworkObjectReference> spawnerItemList = new List<NetworkObjectReference>();
            foreach (GrabbableObject grabbableObject in objectsInScene) {
                if (grabbableObject.radarIcon == null || grabbableObject.radarIcon.gameObject == null || 
                    !grabbableObject.radarIcon.gameObject.activeSelf || grabbableObject.isHeld) continue;
                Vector3 position = grabbableObject.transform.position + new Vector3(0, 0.2f, 0);
                int value = grabbableObject.scrapValue;
                grabbableObject.DestroyObjectInHand(grabbableObject.playerHeldBy);
                
                GameObject newItemObject = Object.Instantiate(spawnableItems[Random.Range(0, spawnableItems.Count)].spawnableItem.spawnPrefab, 
                                            position, Quaternion.identity);
                GrabbableObject newGrabbable = newItemObject.GetComponent<GrabbableObject>();
                newGrabbable.scrapValue = value;
                newGrabbable.transform.rotation = Quaternion.Euler(newGrabbable.itemProperties.restingRotation);
                newGrabbable.fallTime = 0f;
                NetworkObject netObj = newItemObject.GetComponent<NetworkObject>();
                netObj.Spawn();
                // TODO : sync value
                
            }
            
            return "All one-handed items have been replaced with two-handed items!";
        }

        private static IEnumerator WaitAndSyncValue() {
            yield break;
        }
        
        public float GetEventDangerLevel() {
            return 0.9f;
        }

        public float GetEventWeight() {
            return 0.05f;
        }
        public float GetEventRange() {
            return 0.1f;
        }
    }
}