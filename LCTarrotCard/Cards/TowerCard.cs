using GameNetcodeStuff;
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

        public override void ExecuteEffect(PlayerControllerB playerWhoDrew) {
            
            int rng = Random.Range(0, 60);
            
            if (rng <= 1) {
                PluginLogger.Debug("Ship is leaving");
                StartOfRound.Instance.ShipLeaveAutomatically();
            }
            else if (rng <= 22) {
                PluginLogger.Debug("Opening or closing random security doors");
                TerminalAccessibleObject[] objs = Object.FindObjectsOfType<TerminalAccessibleObject>();
                foreach (TerminalAccessibleObject terminalObj in objs) {
                    if (terminalObj.isBigDoor && Random.Range(0, 2) == 0) {
                        terminalObj.SetDoorToggleLocalClient();
                    }
                }
            }
            else if (rng <= 40) {
                PluginLogger.Debug("Opening or closing rng doors");
                Networker.Instance.OpenOrCloseRandomDoorServerRpc((int)playerWhoDrew.playerClientId);
            }
            else if (rng <= 55) {
                PluginLogger.Debug("Inverting ship door state");
                Networker.Instance.SetShipDoorStateServerRpc(StartOfRound.Instance.hangarDoorsClosed);
            }
            else {
                PluginLogger.Debug("Turning off breaker");
                Networker.Instance.BreakerOffServerRpc();
            }

        }

        public TowerCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}