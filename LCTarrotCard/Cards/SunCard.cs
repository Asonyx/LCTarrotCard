using System.Linq;
using GameNetcodeStuff;
using LCTarrotCard.Config;
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

        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int amountOfObjects = playerWhoDrew.ItemSlots.Count(item => item);
            int rng = Random.Range(0, 100);

            if (ConfigManager.SunHealOrDamageChance.Value < rng) {
                PluginLogger.Debug("Healing player to 100 health");
                if (!playerWhoDrew.isPlayerDead && playerWhoDrew.health < 100) 
                    Networker.Instance.SetPlayerHealthServerRpc((int) playerWhoDrew.playerClientId, 100);
                return "Refreshing !";
            }

            if (amountOfObjects > 2 && Random.Range(0, 4) != 0) {
                float randomValueMultiplier = Random.Range(ConfigManager.SunMinMultiplyRange.Value, 
                    ConfigManager.SunMaxMultiplyRange.Value);
                PluginLogger.Debug("Multiplying inventory value by " + randomValueMultiplier);
                Networker.Instance.MultiplyInventoryValueServerRpc((int)playerWhoDrew.playerClientId, randomValueMultiplier);
            }
            else {
                float randomValueMultiplier = Random.Range(ConfigManager.SunMinMultiplyRange.Value, 
                    ConfigManager.SunMaxMultiplyRange.Value);
                PluginLogger.Debug("Multiplying random scrap value by " + randomValueMultiplier);
                Networker.Instance.MultiplyRandomScrapValueServerRpc(randomValueMultiplier);
            }

            return "Seems like some items are more valuable today";
        }

        public override string GetCardName() {
            return "The Sun";
        }

        public SunCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}