using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using LCTarrotCard.Config;
using LCTarrotCard.Ressource;
using LCTarrotCard.Util;
using UnityEngine;

namespace LCTarrotCard.Cards {
    public class DevilCard : Card {

        public override Material GetCardMaterial() {
            return Assets.Materials.CardDevilMat;
        }

        public override Material GetCardBurn() {
            return Assets.Materials.BurnRed;
        }
        
        public static bool AreThereEnemiesSpawned(bool inside = true, bool outside = false) {
            return RoundManager.Instance.SpawnedEnemies.Any(
                enemy => !enemy.isEnemyDead && enemy.IsSpawned && (inside != enemy.isOutside || outside == enemy.isOutside));
        }

        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int rng = Random.Range(0, 100);

            List<int> playerIds = new List<int>();
            for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++) {
                playerIds.Add(i);
            }
            Helper.Shuffle(playerIds);

            
            PlayerControllerB targetPlayer;

            if (!playerWhoDrew.isPlayerDead && playerWhoDrew.isPlayerControlled) {
                targetPlayer = playerWhoDrew;
            }
            else {
                int j = 0;
                do {
                    targetPlayer = StartOfRound.Instance.allPlayerScripts[playerIds[j]];
                    j++;
                } while (targetPlayer.isPlayerDead || !targetPlayer.isPlayerControlled && j < playerIds.Count);
            }
            
            if (targetPlayer.isPlayerDead || !targetPlayer.isPlayerControlled) return "Error : No player to target";


            if (rng < ConfigManager.DevilBlowChance.Value || !AreThereEnemiesSpawned()) {
                targetPlayer.StartCoroutine(WaitAndBlow(targetPlayer));
                
            }
            else {
                Vector3 playerForward = targetPlayer.transform.forward;
                playerForward.y = 0;
                playerForward.Normalize();
                Vector3 tpPos = targetPlayer.transform.position + playerForward * 5;
                Networker.Instance.TeleportRandomEntityServerRpc(tpPos, targetPlayer.isInsideFactory);
            }
            
            return "A mysterious force is moving";
        }

        public override string GetCardName() {
            return "The Devil";
        }

        private static IEnumerator WaitAndBlow(PlayerControllerB player) {
            yield return new WaitForSeconds(2.5f);
            Networker.Instance.GhostBreatheServerRpc(player.transform.position);
        }

        public DevilCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}