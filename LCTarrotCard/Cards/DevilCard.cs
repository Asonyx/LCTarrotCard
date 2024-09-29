using System.Collections;
using System.Collections.Generic;
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

        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int rng = Random.Range(0, 100);

            List<int> playerIds = new List<int>();
            for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++) {
                playerIds.Add(i);
            }
            Helper.Shuffle(playerIds);

            PlayerControllerB randomPlayer;
            int j = 0;

            do {
                randomPlayer = StartOfRound.Instance.allPlayerScripts[playerIds[j]];
                j++;
            } while (randomPlayer.isPlayerDead || !randomPlayer.isPlayerControlled && j < playerIds.Count);
            
            if (randomPlayer.isPlayerDead || !randomPlayer.isPlayerControlled) return "Error : No player to target";


            if (ConfigManager.DevilBlowChance.Value < rng) {
                randomPlayer.StartCoroutine(WaitAndBlow(randomPlayer));
                
            }
            else {
                Vector3 tpPos = randomPlayer.transform.position + randomPlayer.transform.forward * 5;
                Networker.Instance.TeleportRandomEntityServerRpc(tpPos, randomPlayer.isInsideFactory);
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