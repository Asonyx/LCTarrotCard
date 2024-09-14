using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
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

        public override void ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int rng = Random.Range(0, 4);

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
            
            if (randomPlayer.isPlayerDead || !randomPlayer.isPlayerControlled) return;
            
            Debug.Log("Chose player : " + randomPlayer.playerUsername);
            
            if (rng <= 2) {
                Debug.Log("Will tp mob");
                Vector3 tpPos = randomPlayer.transform.position + randomPlayer.gameplayCamera.transform.forward * 5;
                Networker.Instance.TeleportRandomEntityServerRpc(tpPos, randomPlayer.isInsideFactory);
            }
            else {
                randomPlayer.StartCoroutine(WaitAndBlow(randomPlayer));
            }
        }

        private static IEnumerator WaitAndBlow(PlayerControllerB player) {
            yield return new WaitForSeconds(2.5f);
            Networker.Instance.GhostBreatheServerRpc(player.transform.position);
        }

        public DevilCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}