using HarmonyLib;

namespace LCTarrotCard.Patches {
    
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch {
        
        [HarmonyPrefix]
        [HarmonyPatch("EndOfGame")]
        internal static void EndOfGamePatch() {
            TrapsPatch.fakeTraps.Clear();
        }
        
    }
}