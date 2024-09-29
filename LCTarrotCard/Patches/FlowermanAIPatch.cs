using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LCTarrotCard.Patches {
    [HarmonyPatch(typeof(FlowermanAI))]
    public class FlowermanAIPatch {

        public static Dictionary<ulong, KeyValuePair<int, float>> AngryBrackens = new Dictionary<ulong, KeyValuePair<int, float>>();
        
        [HarmonyPatch(nameof(FlowermanAI.DoAIInterval))]
        [HarmonyPrefix]
        internal static bool DoAIIntervalPatch(FlowermanAI __instance) {
            
            if (AngryBrackens.ContainsKey(__instance.NetworkObjectId)) {
                KeyValuePair<int, float> playerAndTime = AngryBrackens[__instance.NetworkObjectId];
                PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerAndTime.Key];
                if (__instance.isOutside == player.isInsideFactory || Time.time - playerAndTime.Value > 15f || !player.isPlayerControlled) {
                    AngryBrackens.Remove(__instance.NetworkObjectId);
                    return true;
                }
                __instance.targetPlayer = player;
                __instance.angerMeter = 2.5f;
                
                if (__instance.currentBehaviourStateIndex != 2) {
                    __instance.SwitchToBehaviourState(2);
                    return true;
                }
                __instance.SetMovingTowardsTargetPlayer(player);
                if (!__instance.inKillAnimation &&
                    __instance.targetPlayer != GameNetworkManager.Instance.localPlayerController) {
                    __instance.ChangeOwnershipOfEnemy(__instance.targetPlayer.actualClientId);
                }
                _DoAIInterval(__instance);
                return false;
            }
            
            return true;
        }
        
        [HarmonyPatch(nameof(FlowermanAI.KillPlayerAnimationClientRpc))]
        [HarmonyPostfix]
        internal static void StopChasingWhenKilling(FlowermanAI __instance) {
            if (AngryBrackens.ContainsKey(__instance.NetworkObjectId)) {
                AngryBrackens.Remove(__instance.NetworkObjectId);
            }
        }
        
        [HarmonyPatch("killAnimation")]
        [HarmonyPrefix]
        internal static bool PreventCrash(FlowermanAI __instance, ref IEnumerator __result) {
            if (Networker.AllowExtraLife) {
                WalkieTalkie.TransmitOneShotAudio(__instance.crackNeckAudio, __instance.crackNeckSFX, 1f);
                __instance.crackNeckAudio.PlayOneShot(__instance.crackNeckSFX);
                __result = _ContinueBrackenKillPatch(__instance);
                return false;
            }

            return true;
        }

        private static IEnumerator _ContinueBrackenKillPatch(FlowermanAI bracken) {
            yield return null;
            bracken.creatureAnimator.SetBool("killing", false);
            bracken.creatureAnimator.SetBool("carryingBody", true);
            yield return new WaitForSeconds(0.65f);
            if (bracken.inSpecialAnimationWithPlayer == null) yield break;
            bracken.inSpecialAnimationWithPlayer.KillPlayer(Vector3.zero, true, CauseOfDeath.Strangulation);
            bracken.FinishKillAnimation(false);
        }

        private static void _DoAIInterval(FlowermanAI flowerman) {
            if (flowerman.moveTowardsDestination) {
                flowerman.agent.SetDestination(flowerman.destination);
            }
            flowerman.SyncPositionToClients();
        }

    }
}