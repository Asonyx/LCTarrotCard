using GameNetcodeStuff;
using LCTarrotCard.Ressource;
using UnityEngine;

namespace LCTarrotCard.Cards {
    public class HangedManCard : Card {

        public override Material GetCardMaterial() {
            return Assets.Materials.CardHangedMat;
        }

        public override Material GetCardBurn() {
            return Assets.Materials.BurnRed;
        }

        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            if (!playerWhoDrew.isPlayerDead && playerWhoDrew.isPlayerControlled) 
                Networker.Instance.KillPlayerServerRpc((int)playerWhoDrew.playerClientId, new Vector3(0, 1, 0), CauseOfDeath.Unknown);
            return "";
        }

        public override string GetCardName() {
            return "The Hanged Man";
        }

        public HangedManCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}