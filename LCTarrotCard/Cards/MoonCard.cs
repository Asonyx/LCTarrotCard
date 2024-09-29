using System.Linq;
using GameNetcodeStuff;
using LCTarrotCard.Config;
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

        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int amountOfObjects = playerWhoDrew.ItemSlots.Count(item => item);
            int rng = Random.Range(0, 100);

            if (ConfigManager.SunHealOrDamageChance.Value < rng) {
                PluginLogger.Debug("Damaging player to 2 health");
                if (!playerWhoDrew.isPlayerDead && playerWhoDrew.health > 2) 
                    Networker.Instance.SetPlayerHealthServerRpc((int) playerWhoDrew.playerClientId, 2);
                return "You feel weak";
            }

            if (amountOfObjects > 2 && Random.Range(0, 4) != 0) {
                float randomValueMultiplier = Random.Range(ConfigManager.MoonMinMultiplyRange.Value, 
                    ConfigManager.MoonMaxMultiplyRange.Value);
                PluginLogger.Debug("Multiplying inventory value by " + randomValueMultiplier);
                Networker.Instance.MultiplyInventoryValueServerRpc((int)playerWhoDrew.playerClientId, randomValueMultiplier);
            }
            else {
                float randomValueMultiplier = Random.Range(ConfigManager.MoonMinMultiplyRange.Value, 
                    ConfigManager.MoonMaxMultiplyRange.Value);
                PluginLogger.Debug("Multiplying random scrap value by " + randomValueMultiplier);
                Networker.Instance.MultiplyRandomScrapValueServerRpc(randomValueMultiplier);
            }
            
            return "Somehow, it seems like some items are less valuable";

        }

        public override string GetCardName() {
            return "The Moon";
        }

        public MoonCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}