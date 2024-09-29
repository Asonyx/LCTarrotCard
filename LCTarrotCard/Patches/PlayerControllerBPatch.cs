using GameNetcodeStuff;
using HarmonyLib;

namespace LCTarrotCard.Patches {
    
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class PlayerControllerBPatch {
        
        [HarmonyPatch(nameof(PlayerControllerB.KillPlayer))]
        [HarmonyPrefix]
        private static bool AllowExtraLife(PlayerControllerB __instance, CauseOfDeath causeOfDeath) {
            if (!Networker.AllowExtraLife || !__instance.IsOwner) return true;
            
            PluginLogger.Debug("Allowing extra life for player " + __instance.playerUsername);
            PluginLogger.Debug("Cause of death: " + causeOfDeath);
            Networker.Instance.ExtraLifeServerRpc((int)__instance.playerClientId);
            __instance.deadBody = null;
            return false;
        }
        
    }
}