using GameNetcodeStuff;
using LCTarrotCard.Ressource;
using UnityEngine;

namespace LCTarrotCard.Cards {
    public class HermitCard : Card {

        public override Material GetCardMaterial() {
            return Assets.Materials.CardHermitMat;
        }

        public override Material GetCardBurn() {
            return Assets.Materials.BurnAqua;
        }

        public override void ExecuteEffect(PlayerControllerB playerWhoDrew) {
            bool rng = Random.Range(0, 2) == 0;
            
            if (rng) {
                PluginLogger.Debug("Teleporting enemy away");
                Networker.Instance.TeleportEnemyAwayServerRpc();
            }
            else {
                PluginLogger.Debug("Teleporting player to random inside node");
                Vector3 randomInsideNode = RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length)].transform.position;
                Networker.Instance.TeleportPlayerServerRpc((int)playerWhoDrew.playerClientId, randomInsideNode);
            }
        }

        public HermitCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}