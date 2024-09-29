using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace LCTarrotCard.Patches {
    
    [HarmonyPatch(typeof(DressGirlAI))]
    public class DressGirlAIPatch {
        
        public static readonly Dictionary<ulong, int> ChasingGirls = new Dictionary<ulong, int>();
        
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void ChosePlayerPatch(DressGirlAI __instance) {
            if (ChasingGirls.ContainsKey(__instance.NetworkObjectId)) {
                __instance.hauntingPlayer = StartOfRound.Instance.allPlayerScripts[ChasingGirls[__instance.NetworkObjectId]];
            }
        }
        
        public static readonly Dictionary<ulong, float> ChaseTimes = new Dictionary<ulong, float>();
        
        [HarmonyPatch("StopChasing")]
        [HarmonyPrefix]
        public static bool StopChasingPatch(DressGirlAI __instance) {
            if (ChaseTimes.TryGetValue(__instance.NetworkObjectId, out float time)) {
                float deltaTime = Time.time - time;
                if (deltaTime <= 20f && Vector3.Distance(__instance.transform.position, __instance.hauntingPlayer.transform.position) <= 50f) {
                    return false;
                }
                ChasingGirls.Remove(__instance.NetworkObjectId);
            }
            
            if (ChasingGirls.ContainsKey(__instance.NetworkObjectId)) {
                ChasingGirls.Remove(__instance.NetworkObjectId);
            }

            return true;
        }
        
    }
}