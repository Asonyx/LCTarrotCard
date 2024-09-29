using GameNetcodeStuff;
using LCTarrotCard.Config;
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

        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            bool rng = Random.Range(0, 100) > ConfigManager.HermitTpChance.Value;
            
            if (rng) {
                PluginLogger.Debug("Teleporting enemy away");
                Networker.Instance.TeleportEnemyAwayServerRpc();
                return "It seems like a mysterious force has moved some things away";
            }
            
            PluginLogger.Debug("Teleporting player to random inside node");
            Vector3 randomInsideNode = RoundManager.Instance.insideAINodes
                [Random.Range(0, RoundManager.Instance.insideAINodes.Length)].transform.position;
            Networker.Instance.TeleportPlayerServerRpc((int)playerWhoDrew.playerClientId, randomInsideNode);
            return "Where am I?";

        }

        public override string GetCardName() {
            return "The Hermit";
        }

        public HermitCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}