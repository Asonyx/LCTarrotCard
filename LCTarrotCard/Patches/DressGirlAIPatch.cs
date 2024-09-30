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
            if (!ChasingGirls.ContainsKey(__instance.NetworkObjectId)) return;
            
            __instance.hauntingPlayer = StartOfRound.Instance.allPlayerScripts[ChasingGirls[__instance.NetworkObjectId]];
            
            if (__instance.hauntingPlayer.playerClientId ==
                GameNetworkManager.Instance.localPlayerController.playerClientId) {
                __instance.SwitchToBehaviourStateOnLocalClient(1);
            }
        }
        
        public static readonly Dictionary<ulong, float> ChaseTimes = new Dictionary<ulong, float>();
        
        [HarmonyPatch("StopChasing")]
        [HarmonyPrefix]
        public static bool StopChasingPatch(DressGirlAI __instance) {
            if (ChaseTimes.TryGetValue(__instance.NetworkObjectId, out float time)) {
                
                float deltaTime = Time.time - time;
                
                if (deltaTime > 20f || Vector3.Distance(__instance.transform.position, __instance.hauntingPlayer.transform.position) > 50f
                    || __instance.hauntingPlayer == null || 
                    __instance.hauntingPlayer.playerClientId != GameNetworkManager.Instance.localPlayerController.playerClientId) {
                    
                    PluginLogger.Debug("Chase time expired, stopping chase");
                    PluginLogger.Debug("Distance: " + Vector3.Distance(__instance.transform.position, __instance.hauntingPlayer.transform.position));
                    PluginLogger.Debug("Time: " + deltaTime);
                    PluginLogger.Debug("Haunting player: " + (__instance.hauntingPlayer == null ? "null" : __instance.hauntingPlayer.playerUsername));
                    PluginLogger.Debug("Is local ? " + (__instance.hauntingPlayer.playerClientId == GameNetworkManager.Instance.localPlayerController.playerClientId));
                    ChasingGirls.Remove(__instance.NetworkObjectId);
                    ChaseTimes.Remove(__instance.NetworkObjectId);
                    return true;
                }

                __instance.EnableEnemyMesh(true, true);
                return false;
                
            }
            
            if (ChasingGirls.ContainsKey(__instance.NetworkObjectId)) {
                ChasingGirls.Remove(__instance.NetworkObjectId);
            }

            return true;
        }
        
        [HarmonyPatch(nameof(DressGirlAI.Update))]
        [HarmonyPrefix]
        internal static void UpdatePatch(DressGirlAI __instance) {
            if (!ChasingGirls.ContainsKey(__instance.NetworkObjectId)) {
                if (ChaseTimes.ContainsKey(__instance.NetworkObjectId)) {
                    ChaseTimes.Remove(__instance.NetworkObjectId);
                }
                return;
            }
            
            if (!ChaseTimes.ContainsKey(__instance.NetworkObjectId)) {
                ChaseTimes.Add(__instance.NetworkObjectId, Time.time);
            }
            
            if (__instance.hauntingPlayer != null && 
                __instance.hauntingPlayer.playerClientId == GameNetworkManager.Instance.localPlayerController.playerClientId && 
                __instance.currentBehaviourStateIndex != 1) {
                
                __instance.SwitchToBehaviourStateOnLocalClient(1);
            }
        }
        
        [HarmonyPatch("OnCollideWithPlayer")]
        [HarmonyPostfix]
        public static void OnCollideWithPlayerPatch(DressGirlAI __instance) {
            if (ChasingGirls.ContainsKey(__instance.NetworkObjectId)) {
                ChasingGirls.Remove(__instance.NetworkObjectId);
            }
        }
        
        
        
    }
}