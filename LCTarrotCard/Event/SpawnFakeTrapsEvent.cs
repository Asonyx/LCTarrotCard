using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using LCTarrotCard.Patches;
using LCTarrotCard.Util;
using Unity.Netcode;
using UnityEngine;

namespace LCTarrotCard.Event {
    public class SpawnFakeTrapsEvent : IEvent {
        public string GetEventName() {
            return "Spawn Fake Traps";
        }
        public string ExecuteEvent(PlayerControllerB targetPlayer) {
            List<SpawnableMapObject> vanillaTraps = RoundManager.Instance.currentLevel.spawnableMapObjects.Where(
                mapObject => mapObject.prefabToSpawn.GetComponentInChildren<Landmine>() != null || 
                             mapObject.prefabToSpawn.GetComponentInChildren<SpikeRoofTrap>() != null || 
                             mapObject.prefabToSpawn.GetComponentInChildren<Turret>() != null).ToList();

            if (vanillaTraps.Count == 0) return "Nothing happened. (really)";
            
            for (int i = 0; i < Random.Range(7, 20); i++) {
                GameObject hazardObject = Object.Instantiate(vanillaTraps[Random.Range(0, vanillaTraps.Count)].prefabToSpawn, 
                    Helper.GetRandomAINodePosition(), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)),
                    RoundManager.Instance.mapPropsContainer.transform);
                hazardObject.GetComponent<NetworkObject>().Spawn(true);
                Landmine landmine = hazardObject.GetComponentInChildren<Landmine>();
                if (landmine != null) landmine.hasExploded = true;
                SpikeRoofTrap spikeTrap = hazardObject.GetComponentInChildren<SpikeRoofTrap>();
                if (spikeTrap != null) {
                    spikeTrap.trapActive = false;
                    TrapsPatch.fakeTraps.Add(hazardObject.GetComponent<NetworkObject>().NetworkObjectId);
                }
                Turret turret = hazardObject.GetComponentInChildren<Turret>();
                if (turret != null) TrapsPatch.fakeTraps.Add(hazardObject.GetComponent<NetworkObject>().NetworkObjectId);
            }
            return "Traps ?!";
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