using GameNetcodeStuff;
using LCTarrotCard.Config;
using LCTarrotCard.Ressource;
using UnityEngine;

namespace LCTarrotCard.Cards {
    public class TowerCard : Card {

        public override Material GetCardMaterial() {
            return Assets.Materials.CardTowerMat;
        }

        public override Material GetCardBurn() {
            return Assets.Materials.BurnBlue;
        }

        private static readonly string[] messages =
        {
            "I can feel a presence somewhere", "Is there a ghost fooling around ?", "It feels like I'm not alone",
            "What was that noise ?", "I think I saw something", "Something moved !"
        };
        
        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int totalWeight = ConfigManager.TowerShipLeaveChance.Value + ConfigManager.TowerDoorsChance.Value +
                              ConfigManager.TowerSecurityDoorsChance.Value + ConfigManager.TowerShipDoorChance.Value +
                              ConfigManager.TowerBreakerChance.Value;
            int rng = Random.Range(0, totalWeight);
            string message = messages[Random.Range(0, messages.Length)];
            
            if (rng <= ConfigManager.TowerShipLeaveChance.Value) {
                PluginLogger.Debug("Ship will leave early");
                Networker.Instance.ShipLeaveEarlyServerRpc();
                return message;
            }

            if (rng <= ConfigManager.TowerShipLeaveChance.Value + ConfigManager.TowerSecurityDoorsChance.Value) {
                PluginLogger.Debug("Opening or closing random security doors");
                TerminalAccessibleObject[] objs = Object.FindObjectsOfType<TerminalAccessibleObject>();
                foreach (TerminalAccessibleObject terminalObj in objs) {
                    if (terminalObj.isBigDoor && Random.Range(0, 2) == 0) {
                        terminalObj.SetDoorToggleLocalClient();
                    }
                }
                return message;
            }

            if (rng <= ConfigManager.TowerShipLeaveChance.Value + ConfigManager.TowerSecurityDoorsChance.Value + 
                ConfigManager.TowerDoorsChance.Value) {
                PluginLogger.Debug("Opening or closing rng doors");
                Networker.Instance.OpenOrCloseRandomDoorServerRpc((int)playerWhoDrew.playerClientId);
                return message;
            }

            if (rng <= ConfigManager.TowerShipLeaveChance.Value + ConfigManager.TowerSecurityDoorsChance.Value + 
                ConfigManager.TowerDoorsChance.Value + ConfigManager.TowerShipDoorChance.Value) {
                PluginLogger.Debug("Inverting ship door state");
                Networker.Instance.SetShipDoorStateServerRpc(StartOfRound.Instance.hangarDoorsClosed);
                return message;
            }

            if (ConfigManager.TowerBreakerChance.Value > 0) {
                PluginLogger.Debug("Turning off breaker");
                Networker.Instance.BreakerOffServerRpc();
                return message;
            }
            
            return "Nothing happened";

        }

        public override string GetCardName() {
            return "The Tower";
        }

        public TowerCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}