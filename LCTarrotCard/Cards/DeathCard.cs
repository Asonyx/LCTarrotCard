using GameNetcodeStuff;
using LCTarrotCard.Ressource;
using UnityEngine;

namespace LCTarrotCard.Cards {
    public class DeathCard : Card {

        public override Material GetCardMaterial() {
            return Assets.Materials.CardDeathMat;
        }

        public override Material GetCardBurn() {
            return Assets.Materials.BurnPurple;
        }

        public override void ExecuteEffect(PlayerControllerB playerWhoDrew) {
            int rng = Random.Range(0, 4);

            switch (rng) {
                case 0:
                    PluginLogger.Debug("Agroing coilhead or spawning one");
                    Networker.Instance.AgroCoilheadOrSpawnServerRpc();
                    return;
                case 1:
                    PluginLogger.Debug("Pop or spawning jester");
                    Networker.Instance.PopOrSpawnJesterServerRpc(2.0f);
                    return;
                default:
                    PluginLogger.Debug("Spawning giant or dog");
                    Networker.Instance.SpawnGiantOrDogServerRpc(rng - 1);
                    return;
            }
        }

        public DeathCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}