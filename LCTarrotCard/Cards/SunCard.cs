using System.Linq;
using GameNetcodeStuff;
using LCTarrotCard.Ressource;
using UnityEngine;

namespace LCTarrotCard.Cards {
    public class SunCard : Card {

        public override Material GetCardMaterial() {
            return Assets.Materials.CardSunMat;
        }

        public override Material GetCardBurn() {
            return Assets.Materials.BurnYellow;
        }

        public override void ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int amountOfObjects = playerWhoDrew.ItemSlots.Count(item => item);
            int rng = Random.Range(0, 10);

            if (rng > 6 && amountOfObjects > 2) {
                float randomValueMultiplier = Random.Range(1.1f, 1.5f);
                PluginLogger.Debug("Multiplying inventory value by " + randomValueMultiplier);
                Networker.Instance.MultiplyInventoryValueServerRpc((int)playerWhoDrew.playerClientId, randomValueMultiplier);
            }
            else if (rng == 0) {
                float randomValueMultiplier = Random.Range(1.1f, 1.5f);
                PluginLogger.Debug("Multiplying random scrap value by " + randomValueMultiplier);
                Networker.Instance.MultiplyRandomScrapValueServerRpc(randomValueMultiplier);
            }
            else {
                PluginLogger.Debug("Healing player to 100 health");
                if (!playerWhoDrew.isPlayerDead && playerWhoDrew.health < 100) Networker.Instance.SetPlayerHealthServerRpc((int) playerWhoDrew.playerClientId, 100);
            }
        }

        public SunCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}