using System.Collections.Generic;
using GameNetcodeStuff;
using LCTarrotCard.Config;
using LCTarrotCard.Ressource;
using LCTarrotCard.Util;
using UnityEngine;

namespace LCTarrotCard.Cards {
    public class DeathCard : Card {

        public override Material GetCardMaterial() {
            return Assets.Materials.CardDeathMat;
        }

        public override Material GetCardBurn() {
            return Assets.Materials.BurnPurple;
        }

        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int rng;
            List<int> outcomes = new List<int>();
            if (playerWhoDrew.isInsideFactory) {
                if (ConfigManager.DeathEnableCoilhead.Value) outcomes.Add(0);
                if (ConfigManager.DeathEnableJester.Value) outcomes.Add(1);
                if (ConfigManager.DeathEnableGhostGirl.Value) outcomes.Add(2);
                if (ConfigManager.DeathEnableBracken.Value) outcomes.Add(3);
                
                if (outcomes.Count == 0) {
                    PluginLogger.Debug("No outcomes enabled, returning");
                    return "Nothing happened";
                }
                
                Helper.Shuffle(outcomes);
                
                rng = outcomes[0];
                
                switch (rng) {
                    case 0:
                        PluginLogger.Debug("Agroing coilhead or spawning one");
                        Networker.Instance.AgroCoilheadOrSpawnServerRpc();
                        break;
                    case 1:
                        PluginLogger.Debug("Pop or spawning jester");
                        Networker.Instance.PopOrSpawnJesterServerRpc(Random.Range(1.5f, 10.5f));
                        break;
                    case 2:
                        PluginLogger.Debug("Make ghost girl chase player or spawn");
                        Networker.Instance.LittleGirlChaseServerRpc((int)playerWhoDrew.playerClientId);
                        break;
                    case 3:
                        PluginLogger.Debug("Make bracken chase player or spawn");
                        Networker.Instance.AgroBrackenOrSpawnServerRpc((int)playerWhoDrew.playerClientId);
                        break;
                }
                return "They know where you are";
            }
            
            if (ConfigManager.DeathEnableDog.Value) outcomes.Add(0);
            if (ConfigManager.DeathEnableGiant.Value) outcomes.Add(1);
            if (ConfigManager.DeathEnableOldBird.Value) outcomes.Add(2);
            if (ConfigManager.DeathEnableWorm.Value) outcomes.Add(3);
            
            if (outcomes.Count == 0) {
                PluginLogger.Debug("No outcomes enabled, returning");
                return "Nothing happened";
            }
            
            Helper.Shuffle(outcomes);
            
            rng = outcomes[0];
            
            switch (rng) {
                case 0:
                    PluginLogger.Debug("Dog chase player or spawn");
                    Networker.Instance.DogChaseOrSpawnServerRpc((int)playerWhoDrew.playerClientId);
                    break;
                case 1:
                    PluginLogger.Debug("Giant chase player or spawn");
                    Networker.Instance.GiantChasePlayerOrSpawnServerRpc((int) playerWhoDrew.playerClientId);
                    break;
                case 2:
                    PluginLogger.Debug("Old bird chase player or spawn");
                    Networker.Instance.BirdChaseOrSpawnServerRpc((int)playerWhoDrew.playerClientId);
                    break;
                case 3:
                    PluginLogger.Debug("Worm chase player or spawn");
                    Networker.Instance.TeleportOrSpawnWornServerRpc((int) playerWhoDrew.playerClientId);
                    break;
            }
            
            return "It's coming for you";
        }

        public override string GetCardName() {
            return "Death";
        }

        public DeathCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}