using GameNetcodeStuff;
using LCTarrotCard.Ressource;
using UnityEngine;

namespace LCTarrotCard.Cards {
    public class HighPriestessCard : Card {
        
        public override Material GetCardMaterial() {
            return Assets.Materials.CardPriestessMat;
        }

        public override Material GetCardBurn() {
            return Assets.Materials.BurnYellow;
        }

        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int foundDeadPlayer = -1;
            for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++) {
                PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[i];
                PluginLogger.Debug("Checking player " + player.playerUsername + " isPlayerControlled: " + player.isPlayerControlled + " isPlayerDead: " + player.isPlayerDead);
                if (!player.isPlayerDead) continue;
                foundDeadPlayer = i;
                break;
            }


            if (foundDeadPlayer != -1) {
                PluginLogger.Debug("Reviving player " +
                                   StartOfRound.Instance.allPlayerScripts[foundDeadPlayer].playerUsername);
                Networker.Instance.RevivePlayerServerRpc(foundDeadPlayer);
                return StartOfRound.Instance.allPlayerScripts[foundDeadPlayer].playerUsername + " is back from the dead";
            }

            PluginLogger.Debug("No dead player found, allowing extra life");
            
            Networker.Instance.AllowExtraLifeServerRpc();
                
            return "Your team has been granted an extra life";
        }

        public override string GetCardName() {
            return "The High Priestess";
        }

        public HighPriestessCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}