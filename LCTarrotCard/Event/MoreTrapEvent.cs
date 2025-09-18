using GameNetcodeStuff;
using LCTarrotCard.Util;
using Unity.Netcode;
using UnityEngine;

namespace LCTarrotCard.Event {
    public class MoreTrapEvent : IEvent {
        public string GetEventName() {
            return "More Trap";
        }
        public string ExecuteEvent(PlayerControllerB targetPlayer) {
            string debugStr = "";
            for (int i = 0; i < Random.Range(1, 10); i++) {
                GameObject hazardObject = Object.Instantiate(RoundManager.Instance.spawnableMapObjects[
                    Random.Range(0, RoundManager.Instance.spawnableMapObjects.Length)].prefabToSpawn, 
                    Helper.GetRandomAINodePosition(), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)),
                    RoundManager.Instance.mapPropsContainer.transform);
                hazardObject.GetComponent<NetworkObject>().Spawn(true);
                debugStr += "|";
            }
            return "Traps ! " + debugStr;
        }
        public float GetEventDangerLevel() {
            return 0.7f;
        }
        public float GetEventWeight() {
            return 0.6f;
        }
        public float GetEventRange() {
            return 0.2f;
        }
    }
}