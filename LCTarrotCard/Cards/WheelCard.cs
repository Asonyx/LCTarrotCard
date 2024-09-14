using GameNetcodeStuff;
using LCTarrotCard.Ressource;
using UnityEngine;

namespace LCTarrotCard.Cards {
    public class WheelCard : Card {

        private bool outcome;

        public override void InitCard() {
            outcome = Random.Range(0, 2) == 0;
            base.InitCard(); 
        }

        public override Material GetCardMaterial() {
            return Assets.Materials.CardWheelMat;
        }

        public override Material GetCardBurn() {
            return outcome ? Assets.Materials.BurnGreen : Assets.Materials.BurnRed;
        }

        public override void ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int rng = Random.Range(0, 2);

            if (outcome) {
                switch (rng) {
                    case 0:
                        PluginLogger.Debug("Healing by 20 hp");
                        if (playerWhoDrew.health >= 80) {
                            Networker.Instance.SetPlayerHealthServerRpc((int)playerWhoDrew.playerClientId, 100);
                        }
                        else Networker.Instance.SetPlayerHealthServerRpc((int)playerWhoDrew.playerClientId, playerWhoDrew.health + 20);
                        return;
                    case 1:
                        PluginLogger.Debug("Multiplying some scrap value by 1.1");
                        Networker.Instance.MultiplyRandomScrapValueServerRpc(1.1f);
                        return;
                }
            }
            else {
                switch (rng) {
                    case 0:
                        PluginLogger.Debug("Damaging by 20 hp");
                        Networker.Instance.DamagePlayerServerRpc((int) playerWhoDrew.playerClientId, 20, default, CauseOfDeath.Unknown);
                        return;
                    case 1:
                        PluginLogger.Debug("Multiplying some scrap value by 0.9");
                        Networker.Instance.MultiplyRandomScrapValueServerRpc(0.9f);
                        return;
                }
            }
        }

        public WheelCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}