using GameNetcodeStuff;
using HarmonyLib;

namespace LCTarrotCard.Patches {
    
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class PlayerControllerBPatch {
        
        [HarmonyPatch(nameof(PlayerControllerB.KillPlayer))]
        [HarmonyPrefix]
        private static bool AllowExtraLife(PlayerControllerB __instance) {
            if (!Networker.AllowExtraLife) return true;
            
            Networker.Instance.ExtraLifeServerRpc((int)__instance.playerClientId);
            return false;
        }
        
    }
}