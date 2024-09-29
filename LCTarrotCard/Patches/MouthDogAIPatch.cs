using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LCTarrotCard.Patches {
    
    [HarmonyPatch(typeof(MouthDogAI))]
    public class MouthDogAIPatch {
        
        public static Dictionary<ulong, int> ChasingDogs = new Dictionary<ulong, int>();
        
        [HarmonyPatch("SearchForPreviouslyHeardSound")]
        [HarmonyPrefix]
        internal static bool MakeDogChasePlayer(MouthDogAI __instance) {
            if (!ChasingDogs.TryGetValue(__instance.NetworkObjectId, out int playerId)) return true;
            
            
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerId];
            if (!player.isPlayerControlled || player.isInsideFactory || Vector3.Distance(player.transform.position, __instance.transform.position) > 5f) {
                ChasingDogs.Remove(__instance.NetworkObjectId);
                return true;
            }
            
            Networker.Instance.SetDogPathServerRpc(__instance.NetworkObjectId, (int)player.playerClientId);
            return false;
        }
        
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        internal static void UpdatePatch(MouthDogAI __instance) {
            if (!ChasingDogs.ContainsKey(__instance.NetworkObjectId)) return;
            __instance.suspicionLevel = 12;
        }
        
    }
}