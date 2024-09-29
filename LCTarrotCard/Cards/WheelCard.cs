using System;
using GameNetcodeStuff;
using LCTarrotCard.Config;
using LCTarrotCard.Ressource;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LCTarrotCard.Cards {
    public class WheelCard : Card {

        private bool outcome;

        public override void InitCard(System.Random random) {
            outcome = random.Next(0, 2) == 0;
            base.InitCard(random); 
        }

        public override Material GetCardMaterial() {
            return Assets.Materials.CardWheelMat;
        }

        public override Material GetCardBurn() {
            return outcome ? Assets.Materials.BurnGreen : Assets.Materials.BurnRed;
        }

        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            float rng = Random.Range(0f, 100f);

            if (outcome) {
                if (rng <= ConfigManager.WheelHealOrDamageChance.Value) {
                    PluginLogger.Debug("Healing player");
                    Networker.Instance.SetPlayerHealthServerRpc((int)playerWhoDrew.playerClientId,
                        Math.Min(playerWhoDrew.health + ConfigManager.WheelHealOrDamageValue.Value, 100));
                    return "Lucky - You've healed from your injuries";
                }
                if (ConfigManager.WheelMultiplyValueChance.Value > 0) {
                    float multiplier = ConfigManager.WheelGoodMultiplyRange.Value;
                    PluginLogger.Debug("Multiplying some scrap value by " + multiplier);
                    Networker.Instance.MultiplyRandomScrapValueServerRpc(multiplier);
                    return "Lucky - Stonks";
                }
                
            }
            else {
                if (rng <= ConfigManager.WheelHealOrDamageChance.Value) {
                    PluginLogger.Debug("Damaging player");
                    Networker.Instance.DamagePlayerServerRpc((int) playerWhoDrew.playerClientId, ConfigManager.WheelHealOrDamageValue.Value, default, CauseOfDeath.Unknown);
                    return "Unlucky - You've been hurt";
                }

                if (ConfigManager.WheelMultiplyValueChance.Value > 0) {
                    float multiplier = ConfigManager.WheelBadMultiplyRange.Value;
                    PluginLogger.Debug("Multiplying some scrap value by " + multiplier);
                    Networker.Instance.MultiplyRandomScrapValueServerRpc(multiplier);
                    return "Unlucky - The value of some items plummeted";
                }
            }
            return "Nothing happened";
        }

        public override string GetCardName() {
            return "The Wheel of Fortune";
        }

        public WheelCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}