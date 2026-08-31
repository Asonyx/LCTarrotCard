using System.Collections.Generic;
using HarmonyLib;
using Unity.Netcode;

namespace LCTarrotCard.Patches {
    public class TrapsPatch {
        public static List<ulong> fakeTraps = new List<ulong>();
        
        [HarmonyPatch(typeof(SpikeRoofTrap))]
        internal class SpikeRoofTrapPatch {
            [HarmonyPrefix]
            [HarmonyPatch("ToggleSpikesEnabledLocalClient")]
            internal static bool ToggleSpikesPatch(SpikeRoofTrap __instance) {
                if (__instance.gameObject.GetComponentInParent<NetworkObject>() &&
                    fakeTraps.Contains(__instance.gameObject.GetComponentInParent<NetworkObject>().NetworkObjectId)) {
                    __instance.trapActive = false;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(Turret))]
        internal class TurretPatch {
            
            [HarmonyPrefix]
            [HarmonyPatch("Update")]
            internal static void UpdatePatch(Turret __instance) {
                if (!__instance.gameObject.GetComponentInParent<NetworkObject>() ||
                    !fakeTraps.Contains(__instance.gameObject.GetComponentInParent<NetworkObject>().NetworkObjectId))
                    return;
                
                if (__instance.turretMode == TurretMode.Firing) {
                    __instance.turretMode = TurretMode.Charging;
                }
                else if (__instance.turretMode == TurretMode.Berserk) {
                    __instance.turretMode = TurretMode.Detection;
                }
            }
        }


    }
}