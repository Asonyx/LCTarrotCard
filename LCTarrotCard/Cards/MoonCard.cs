using System.Linq;
using GameNetcodeStuff;
using LCTarrotCard.Ressource;
using UnityEngine;

namespace LCTarrotCard.Cards {
    public class MoonCard : Card {

        public override Material GetCardMaterial() {
            return Assets.Materials.CardMoonMat;
        }

        public override Material GetCardBurn() {
            return Assets.Materials.BurnWhite;
        }

        public override void ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int amountOfObjects = playerWhoDrew.ItemSlots.Count(item => item);
            int rng = Random.Range(0, 10);

            if (rng > 6 && amountOfObjects > 2) {
                float randomValueMultiplier = Random.Range(0.1f, 0.9f);
                PluginLogger.Debug("Multiplying inventory value by " + randomValueMultiplier);
                Networker.Instance.MultiplyInventoryValueServerRpc((int)playerWhoDrew.playerClientId, randomValueMultiplier);
            }
            else if (rng == 0) {
                float randomValueMultiplier = Random.Range(0.1f, 0.9f);
                PluginLogger.Debug("Multiplying random scrap value by " + randomValueMultiplier);
                Networker.Instance.MultiplyRandomScrapValueServerRpc(randomValueMultiplier);
            }
            else {
                PluginLogger.Debug("Damaging player to 2 health");
                if (!playerWhoDrew.isPlayerDead && playerWhoDrew.health > 2) Networker.Instance.SetPlayerHealthServerRpc((int) playerWhoDrew.playerClientId, 2);
            }
            
        }

        public MoonCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}