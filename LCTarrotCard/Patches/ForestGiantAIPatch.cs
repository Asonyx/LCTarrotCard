using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LCTarrotCard.Patches {
    
    [HarmonyPatch(typeof(ForestGiantAI))]
    public class ForestGiantAIPatch {
        public static readonly Dictionary<ulong, int> ChasingGiants = new Dictionary<ulong, int>();

        [HarmonyPatch(nameof(ForestGiantAI.Update))]
        [HarmonyPostfix]
        internal static void MakeGiantChasePlayerPatch(ForestGiantAI __instance, ref float ___noticePlayerTimer, ref bool ___lostPlayerInChase) {
            if (!ChasingGiants.ContainsKey(__instance.NetworkObjectId)) return;
            
            PlayerControllerB chasedPlayer =  StartOfRound.Instance.allPlayerScripts[ChasingGiants[__instance.NetworkObjectId]];
            if (!chasedPlayer.isPlayerControlled || chasedPlayer.isInsideFactory || chasedPlayer.isInHangarShipRoom || 
                Vector3.Distance(chasedPlayer.transform.position, __instance.transform.position) < 10f) {
                PluginLogger.Debug("Removing giant from chasing list : player doesn't meet requirements");
                ChasingGiants.Remove(__instance.NetworkObjectId);
                return;
            }
            if (__instance.currentBehaviourStateIndex != 1) {
                __instance.SwitchToBehaviourState(1);
            }

            ___noticePlayerTimer = 0f;
            __instance.chasingPlayer = chasedPlayer;
            ___lostPlayerInChase = false;

        }
        
    }
}