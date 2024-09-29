using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LCTarrotCard.Patches {
    
    [HarmonyPatch(typeof(RadMechAI))]
    public class RadMechAIPatch {

        public static readonly Dictionary<ulong, int> ChasingBirds = new Dictionary<ulong, int>();
        
        [HarmonyPatch(nameof(RadMechAI.DoAIInterval))]
        [HarmonyPrefix]
        internal static void MakeBirdChasePlayer(RadMechAI __instance, ref bool ___lostCreatureInChase, ref float ___losTimer) {
            if (!ChasingBirds.TryGetValue(__instance.NetworkObjectId, out int playerId)) return;
            
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerId];
            if (!player.isPlayerControlled || player.isInsideFactory || player.isInHangarShipRoom || 
                Vector3.Distance(player.transform.position, __instance.transform.position) < 10f) {
                PluginLogger.Debug("Player is not valid anymore, stopping chase");
                ChasingBirds.Remove(__instance.NetworkObjectId);
                return;
            }
            
            if (!__instance.spotlight.activeSelf) __instance.EnableSpotlight();
            
            if (__instance.currentBehaviourStateIndex != 1) {
                __instance.SwitchToBehaviourState(1);
            }
            __instance.SetTargetedThreat(player, player.transform.position, 0f);
            __instance.focusedThreatTransform = player.transform;
            __instance.alertTimer = 2f;
            __instance.isAlerted = true;
            ___lostCreatureInChase = false;
            ___losTimer = 0f;
        }
        
        private static readonly List<ulong> ActiveBlowtorches = new List<ulong>();
        
        [HarmonyPatch(nameof(RadMechAI.EnableBlowtorch))]
        [HarmonyPostfix]
        internal static void EnableBlowtorch(RadMechAI __instance) {
            if (!ActiveBlowtorches.Contains(__instance.NetworkObjectId)) {
                ActiveBlowtorches.Add(__instance.NetworkObjectId);
            }
        }
        
        [HarmonyPatch(nameof(RadMechAI.DisableBlowtorch))]
        [HarmonyPostfix]
        internal static void DisableBlowtorch(RadMechAI __instance) {
            if (ActiveBlowtorches.Contains(__instance.NetworkObjectId)) {
                ActiveBlowtorches.Remove(__instance.NetworkObjectId);
            }
        }

        [HarmonyPatch("TorchPlayerAnimation")]
        [HarmonyPrefix]
        internal static bool TorchPlayerAnimPatch(RadMechAI __instance, ref IEnumerator __result) {
            __instance.creatureAnimator.SetBool("AttemptingGrab", true);
            __instance.creatureAnimator.SetBool("GrabSuccessful", true);
            __instance.creatureAnimator.SetBool("GrabUnsuccessful", false);
            float startTime = Time.realtimeSinceStartup;
            PluginLogger.Debug("Starting torching player animation pt2");
            __result = ContinueTorchPlayerAnim(__instance, startTime);
            return false;
        }
        
        private static IEnumerator ContinueTorchPlayerAnim(RadMechAI bird, float startTime) {
            yield return new WaitUntil(() => ActiveBlowtorches.Contains(bird.NetworkObjectId) || Time.realtimeSinceStartup - startTime > 6f);
            PluginLogger.Debug("Start Damages");
            startTime = Time.realtimeSinceStartup;
            while (ActiveBlowtorches.Contains(bird.NetworkObjectId) && Time.realtimeSinceStartup - startTime < 6f) {
                yield return new WaitForSeconds(0.25f);
                if (bird.inSpecialAnimationWithPlayer != null) {
                    bird.inSpecialAnimationWithPlayer.DamagePlayer(20, true, true, CauseOfDeath.Burning, 6);
                }
                else {
                    bird.StartCoroutine(FinishTorching(bird));
                    yield break;
                }
            }

            if (bird.inSpecialAnimationWithPlayer != null &&
                bird.inSpecialAnimationWithPlayer == GameNetworkManager.Instance.localPlayerController &&
                !bird.inSpecialAnimationWithPlayer.isPlayerDead) {
                PluginLogger.Debug("Killing player !");
                bird.inSpecialAnimationWithPlayer.KillPlayer(Vector3.zero, true, CauseOfDeath.Burning, 6);
            }

            bird.StartCoroutine(FinishTorching(bird));
        }

        private static IEnumerator FinishTorching(RadMechAI bird) {
            PluginLogger.Debug("Finishing torching player animation");
            yield return new WaitForSeconds(1.5f);
            bird.CancelTorchPlayerAnimation();
            if (bird.IsServer) {
                bird.inTorchPlayerAnimation = false;
                bird.inSpecialAnimationWithPlayer = null;
                bird.inSpecialAnimation = false;
                bird.agent.enabled = true;
            }
        }

    }
}