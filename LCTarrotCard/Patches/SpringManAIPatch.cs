using System.Collections.Generic;
using HarmonyLib;

namespace LCTarrotCard.Patches {
    
    [HarmonyPatch(typeof(SpringManAI))]
    public class SpringManAIPatch {
        public static readonly List<ulong> ChasingSprings = new List<ulong>();
        
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        internal static void UpdatePatch(SpringManAI __instance, ref bool ___stoppingMovement, ref bool ___setOnCooldown) {
            if (!ChasingSprings.Contains(__instance.NetworkObjectId)) return;
            if (___stoppingMovement) {
                ChasingSprings.Remove(__instance.NetworkObjectId);
                return;
            }
            if (__instance.currentBehaviourStateIndex != 1) {
                __instance.SwitchToBehaviourState(1);
            }

            __instance.timeSpentMoving = 0f;
            ___setOnCooldown = false;
        }

    }
}