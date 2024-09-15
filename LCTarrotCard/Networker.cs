using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using HarmonyLib;
using LCTarrotCard.Ressource;
using LCTarrotCard.Util;
using Unity.Netcode;
using UnityEngine;

namespace LCTarrotCard {
    [HarmonyPatch]
    public class Networker : NetworkBehaviour {
        public static Networker Instance { get; private set; }
        
        
        [ServerRpc(RequireOwnership = false)]
        public void KillPlayerServerRpc(int playerId, Vector3 velocity, CauseOfDeath causeOfDeath) {
            KillPlayerClientRpc(playerId, velocity, causeOfDeath);
        }

        [ClientRpc]
        public void KillPlayerClientRpc(int playerId, Vector3 velocity, CauseOfDeath causeOfDeath) {
            StartOfRound.Instance.allPlayerScripts[playerId]?.KillPlayer(velocity, true, causeOfDeath);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetShipDoorStateServerRpc(bool opened) {
            SetShipDoorStateClientRpc(opened);
        }
        
        [ClientRpc]
        public void SetShipDoorStateClientRpc(bool opened) {
            foreach (HangarShipDoor shipDoor in Object.FindObjectsOfType<HangarShipDoor>()) {
                if (opened) {
                    shipDoor.SetDoorOpen();
                    shipDoor.doorPower = 0f;
                }
                else {
                    shipDoor.SetDoorClosed();
                    shipDoor.doorPower = 100;
                }
                shipDoor.shipDoorsAnimator.SetBool(Closed, !opened);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void OpenOrCloseRandomDoorServerRpc(int playerId) {
            OpenOrCloseRandomDoorClientRpc(playerId, new System.Random().Next());
        }
        
        [ClientRpc]
        public void OpenOrCloseRandomDoorClientRpc(int playerId, int rngSeed) {
            System.Random rng = new System.Random(rngSeed);
            foreach (DoorLock door in FindObjectsOfType<DoorLock>()) {
                if (rng.Next(0, 2) == 0) door.OpenOrCloseDoor(StartOfRound.Instance.allPlayerScripts[playerId]);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void BreakerOffServerRpc() {
            BreakerOffClientRpc();
        }

        [ClientRpc]
        public void BreakerOffClientRpc() {
            RoundManager.Instance.SwitchPower(false);
            BreakerBox[] bboxs = FindObjectsOfType<BreakerBox>();
            foreach (BreakerBox box in bboxs) {
                box.SetSwitchesOff();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void AgroCoilheadOrSpawnServerRpc() {
            SpringManAI[] coilheads = FindObjectsOfType<SpringManAI>();
                
            int agroCount = 0;
            foreach (SpringManAI coilhead in coilheads) {
                if (!coilhead.isEnemyDead && coilhead.IsSpawned) {
                    agroCount++;
                    coilhead.SwitchToBehaviourState(1);
                }
                if (agroCount >= 2) return;
            }
                            
            SpawnCoilheadServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SpawnCoilheadServerRpc() {

            EnemyVent[] vents = FindObjectsOfType<EnemyVent>();
            Vector3 spawnPos = vents[Random.Range(0, vents.Length)].floorNode.position;
            GameObject coilhead = Instantiate(Helper.Enemies.SpringMan.enemyPrefab, spawnPos, Quaternion.identity);
            coilhead.GetComponentInChildren<NetworkObject>().Spawn(true);
            RoundManager.Instance.SpawnedEnemies.Add(coilhead.GetComponent<SpringManAI>());
            coilhead.GetComponent<SpringManAI>().SetEnemyOutside();
            
            coilhead.GetComponent<SpringManAI>().SwitchToBehaviourState(1);
        }

        [ServerRpc(RequireOwnership = false)]
        public void PopOrSpawnJesterServerRpc(float popDelay) {
            bool foundJester = false;
            foreach (EnemyAI enemy in RoundManager.Instance.SpawnedEnemies) {
                if (enemy is JesterAI jester && !jester.isEnemyDead) {
                    foundJester = true;
                }
            }

            if (!foundJester) {
                
                EnemyVent[] vents = FindObjectsOfType<EnemyVent>();
                Vector3 spawnPos = vents[Random.Range(0, vents.Length)].floorNode.position;
            
                GameObject jester = Instantiate(Helper.Enemies.Jester.enemyPrefab, spawnPos, Quaternion.identity);
                jester.GetComponentInChildren<NetworkObject>().Spawn(true);
                RoundManager.Instance.SpawnedEnemies.Add(jester.GetComponent<JesterAI>());
                jester.GetComponent<JesterAI>().SetEnemyOutside();
            
                jester.GetComponent<JesterAI>().SwitchToBehaviourState(1);
                
            }

            StartCoroutine(PopDelay(popDelay));

        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnGiantOrDogServerRpc(int amount) {
            GameObject[] outsideNodes = GameObject.FindGameObjectsWithTag("OutsideAINode");
            for (int i = 0; i < amount; i++) {
                Vector3 spawnPos = outsideNodes[Random.Range(0, outsideNodes.Length)].transform.position;
                if (Random.Range(0, 2) == 0) {
                    PluginLogger.Debug("Spawn giant");
                    GameObject giant = Instantiate(Helper.Enemies.ForestGiant.enemyPrefab, spawnPos, Quaternion.identity);
                    giant.GetComponentInChildren<NetworkObject>().Spawn(true);
                    RoundManager.Instance.SpawnedEnemies.Add(giant.GetComponent<ForestGiantAI>());
                    giant.GetComponent<ForestGiantAI>().SetEnemyOutside(true);
                }
                else {
                    PluginLogger.Debug("Spawn dog");
                    GameObject dog = Instantiate(Helper.Enemies.MouthDog.enemyPrefab, spawnPos, Quaternion.identity);
                    dog.GetComponentInChildren<NetworkObject>().Spawn(true);
                    RoundManager.Instance.SpawnedEnemies.Add(dog.GetComponent<MouthDogAI>());
                    dog.GetComponent<MouthDogAI>().SetEnemyOutside(true);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void TeleportRandomEntityServerRpc(Vector3 position, bool inside) {
            List<EnemyAI> allEnemies = RoundManager.Instance.SpawnedEnemies;
            Helper.Shuffle(allEnemies);
            foreach (EnemyAI enemy in allEnemies.Where(enemy => !enemy.isEnemyDead && enemy.isOutside == !inside && enemy.IsSpawned)) {
                PluginLogger.Debug("Tp " + enemy.enemyType.enemyName + " to " + position);
                TeleportEnemy(enemy, position);
                return;
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void GhostBreatheServerRpc(Vector3 position) {
            GhostBreatheClientRpc(position);
        }
        
        [ClientRpc]
        public void GhostBreatheClientRpc(Vector3 position) {
            AudioSource.PlayClipAtPoint(Assets.GhostBreathe, position, 3);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void TeleportEnemyAwayServerRpc() {
            List<EnemyAI> allEnemies = RoundManager.Instance.SpawnedEnemies;
            foreach (EnemyAI enemy in allEnemies) {
                Vector3 newPos = enemy.ChooseFarthestNodeFromPosition(enemy.transform.position).position;
                TeleportEnemy(enemy, newPos);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void TeleportPlayerServerRpc(int playerId, Vector3 position, bool inside = true, bool insideShip = false) {
            TeleportPlayerClientRpc(playerId, position, inside, insideShip);
        }
        
        [ClientRpc]
        public void TeleportPlayerClientRpc(int playerId, Vector3 position, bool inside = true, bool insideShip = false) {
            PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerId];

            if (FindObjectOfType<AudioReverbPresets>()) {
                FindObjectOfType<AudioReverbPresets>().audioPresets[2].ChangeAudioReverbForPlayer(playerControllerB);
            }
            
            playerControllerB.StopSinkingServerRpc();
            playerControllerB.CancelSpecialTriggerAnimations();
            playerControllerB.inSpecialInteractAnimation = false;
            playerControllerB.isClimbingLadder = false;
            
            playerControllerB.isInElevator = insideShip;
            playerControllerB.isInHangarShipRoom = insideShip;
            playerControllerB.isInsideFactory = inside;
            playerControllerB.averageVelocity = 0f;
            playerControllerB.velocityLastFrame = Vector3.zero;
            playerControllerB.TeleportPlayer(position);
            playerControllerB.beamOutParticle.Play();
            if (playerId == (int) GameNetworkManager.Instance.localPlayerController.playerClientId) {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
            }
        }
        
        
        
        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerHealthServerRpc(int playerId, int health) {
            SetPlayerHealthClientRpc(playerId, health);
        }
        
        [ClientRpc]
        public void SetPlayerHealthClientRpc(int playerId, int health) {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerId];
            int previousHealth = player.health;
            player.health = health;

            if (player.IsOwner) {
                HUDManager.Instance.UpdateHealthUI(health, previousHealth > health);
                if (health < 10 && !player.criticallyInjured) {
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
                    player.MakeCriticallyInjured(true);
                }
                else if (player.criticallyInjured && health > 10) {
                    player.MakeCriticallyInjured(false);
                }
                
            }
            
            if (previousHealth > health) return;
            
            if (player.IsOwner) {
                
                StartOfRound.Instance.LocalPlayerDamagedEvent.Invoke();
                player.takingFallDamage = false;
                if (player.inSpecialInteractAnimation && !player.twoHandedAnimation) {
                    player.playerBodyAnimator.SetTrigger(Damage);
                }

                player.specialAnimationWeight = 1f;
                player.PlayQuickSpecialAnimation(0.7f);
            }
            player.playersManager.gameStats.allPlayerStats[player.playerClientId].damageTaken += previousHealth - health;
            player.timeSinceTakingDamage = Time.realtimeSinceStartup;
            
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void DamagePlayerServerRpc(int playerId, int damage, Vector3 velocity, CauseOfDeath causeOfDeath) {
            DamagePlayerClientRpc(playerId, damage, velocity, causeOfDeath);
        }
        
        [ClientRpc]
        public void DamagePlayerClientRpc(int playerId, int damage, Vector3 velocity, CauseOfDeath causeOfDeath) {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerId];
            player.DamagePlayer(damage, true, true, causeOfDeath, force: velocity);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void MultiplyInventoryValueServerRpc(int playerId, float multiplier) {
            MultiplyInventoryValueClientRpc(playerId, multiplier);
        }
        
        [ClientRpc]
        public void MultiplyInventoryValueClientRpc(int playerId, float multiplier) {
            foreach (GrabbableObject item in StartOfRound.Instance.allPlayerScripts[playerId].ItemSlots) {
                item.SetScrapValue((int)(item.scrapValue * multiplier));
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void MultiplyRandomScrapValueServerRpc(float multiplier) {
            MultiplyRandomScrapValueClientRpc(multiplier, new System.Random().Next());
        }
        
        [ClientRpc]
        public void MultiplyRandomScrapValueClientRpc(float multiplier, int randomSeed) {
            System.Random rng = new System.Random(randomSeed);
            GrabbableObject[] items = FindObjectsOfType<GrabbableObject>();
            foreach (GrabbableObject item in items) {
                if (item.enabled && item.itemProperties.isScrap && item.scrapValue > 0 && rng.Next(0, 5) == 0) {
                    item.SetScrapValue((int)(item.scrapValue * multiplier));
                }
            }
            
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RevivePlayerServerRpc(int playerId) {
            RevivePlayerClientRpc(playerId);
        }

        [ClientRpc]
        public void RevivePlayerClientRpc(int playerId) {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerId];
            if (!player.isPlayerDead && !player.isPlayerControlled) return;
            player.ResetPlayerBloodObjects();
            player.isClimbingLadder = false;
            player.clampLooking = false;
            player.inVehicleAnimation = false;
            player.disableMoveInput = false;
            player.ResetZAndXRotation();
            player.thisController.enabled = true;
            player.health = 100;
            player.hasBeenCriticallyInjured = false;
            player.disableLookInput = false;
            player.disableInteract = false;
            if (player.isPlayerDead) {
                player.isPlayerDead = false;
                player.isPlayerControlled = true;
                player.isInElevator = true;
                player.isInHangarShipRoom = true;
                player.isInsideFactory = false;
                player.parentedToElevatorLastFrame = false;
                player.overrideGameOverSpectatePivot = null;
                StartOfRound.Instance.SetPlayerObjectExtrapolate(false);
                player.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].position);
                player.setPositionOfDeadPlayer = false;
                player.DisablePlayerModel(StartOfRound.Instance.allPlayerObjects[playerId], true, true);
                player.helmetLight.enabled = false;
                player.Crouch(false);
                player.criticallyInjured = false;
                if (player.playerBodyAnimator) player.playerBodyAnimator.SetBool(Limp, false);
                player.bleedingHeavily = false;
                player.activatingItem = false;
                player.twoHanded = false;
                player.inShockingMinigame = false;
                player.inSpecialInteractAnimation = false;
                player.freeRotationInInteractAnimation = false;
                player.disableSyncInAnimation = false;
                player.inAnimationWithEnemy = null;
                player.holdingWalkieTalkie = false;
                player.speakingToWalkieTalkie = false;
                player.isSinking = false;
                player.isUnderwater = false;
                player.sinkingValue = 0f;
                player.statusEffectAudio.Stop();
                player.DisableJetpackControlsLocally();
                player.health = 100;
                player.mapRadarDotAnimator.SetBool("dead", false);
                player.externalForceAutoFade = Vector3.zero;
                if (player.IsOwner) {
                    HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", false);
                    player.hasBegunSpectating = false;
                    HUDManager.Instance.RemoveSpectateUI();
                    HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
                    player.hinderedMultiplier = 1f;
                    player.isMovementHindered = 0;
                    player.sourcesCausingSinking = 0;
                    player.reverbPreset = StartOfRound.Instance.shipReverb;
                }
            }

            if (player.IsOwner) {
                SoundManager.Instance.earsRingingTimer = 0f; 
                player.bleedingHeavily = false;
                player.criticallyInjured = false;
                player.playerBodyAnimator.SetBool(Limp, false);
                player.health = 100;
                HUDManager.Instance.UpdateHealthUI(100, false);
                player.spectatedPlayerScript = null;
                HUDManager.Instance.audioListenerLowPass.enabled = false;
                StartOfRound.Instance.SetSpectateCameraToGameOverMode(false, player);
            }
            player.voiceMuffledByEnemy = false;
            SoundManager.Instance.playerVoicePitchTargets[playerId] = 1f;
            SoundManager.Instance.SetPlayerPitch(1f, playerId);
            if (player.currentVoiceChatIngameSettings) {
                if (!player.currentVoiceChatIngameSettings.voiceAudio) {
                    player.currentVoiceChatIngameSettings.InitializeComponents();
                }

                if (!player.currentVoiceChatIngameSettings.voiceAudio) return;
                player.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
            }
            
            RagdollGrabbableObject[] ragdollGrabbableObjects = FindObjectsOfType<RagdollGrabbableObject>();
            foreach (RagdollGrabbableObject ragdollObj in ragdollGrabbableObjects) {
                if (ragdollObj.ragdoll.playerObjectId != playerId) continue;

                switch (ragdollObj.isHeld) {
                    case false when !IsServer:
                        continue;
                    case false when ragdollObj.NetworkObject.IsSpawned:
                        ragdollObj.NetworkObject.Despawn();
                        break;
                    case false:
                        Destroy(ragdollObj.gameObject);
                        break;
                    case true when ragdollObj.playerHeldBy:
                        ragdollObj.playerHeldBy.DropAllHeldItems();
                        break;
                }
            }
            
            DeadBodyInfo[] deadBodies = FindObjectsOfType<DeadBodyInfo>();
            foreach (DeadBodyInfo deadBody in deadBodies) {
                if (deadBody.playerObjectId != playerId) continue;
                Destroy(deadBody.gameObject);
            }

            StartOfRound.Instance.livingPlayers++;
            StartOfRound.Instance.allPlayersDead = false;
            StartOfRound.Instance.UpdatePlayerVoiceEffects();
        }


        public static bool AllowExtraLife;

        [ServerRpc(RequireOwnership = false)]
        public void AllowExtraLifeServerRpc() {
            AllowExtraLifeClientRpc(true);
        }

        [ClientRpc]
        public void AllowExtraLifeClientRpc(bool allow) {
            AllowExtraLife = allow;
        }

        [ServerRpc(RequireOwnership = false)]
        public void ExtraLifeServerRpc(int playerId) {
            if (!AllowExtraLife) return;
            AllowExtraLifeClientRpc(false);
            TeleportPlayerServerRpc(playerId, StartOfRound.Instance.playerSpawnPositions[0].position, false, true);
            ExtraLifeClientRpc(playerId);
        }

        [ClientRpc]
        public void ExtraLifeClientRpc(int playerId) {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerId];
            player.health = 100;
            player.isPlayerDead = false;
            player.MakeCriticallyInjured(false);
            if (player.IsOwner) {
                HUDManager.Instance.UpdateHealthUI(100, false);
            }
            if (StartOfRound.Instance.allPlayerScripts[playerId].IsOwner) {
                HUDManager.Instance.DisplayTip("Extra life", "Thanks to the high priestess card, you are given an extra chance to live");
            }
        }
        

        public static void TeleportEnemy(EnemyAI enemy, Vector3 position, bool exitEnter = false, bool outside = false) {
            if (exitEnter) {
                enemy.SetEnemyOutside(outside);
            }

            if (enemy.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId) {
                enemy.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
            }

            Vector3 newPos = enemy.ChooseClosestNodeToPosition(position).position;
            enemy.serverPosition = newPos;
            enemy.transform.position = enemy.serverPosition;
            enemy.agent.Warp(enemy.serverPosition);
            enemy.SyncPositionToClients();
        }
        
        
        private static IEnumerator PopDelay(float delay) {
            yield return new WaitForSeconds(delay);
            PluginLogger.Debug("Popping jesters");
            foreach (EnemyAI enemy in RoundManager.Instance.SpawnedEnemies) {
                if (enemy is JesterAI jester && !jester.isEnemyDead) {
                    jester.SwitchToBehaviourState(2);
                }
            }
        }
        
        
        
        

        public override void OnNetworkSpawn() {
            
            
            if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer))
                Instance?.gameObject.GetComponent<NetworkObject>().Despawn();
            Instance = this;
            
            base.OnNetworkSpawn();
        }


        
        

        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        private static void SpawnNetHandler() {
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;
            
            
            GameObject networkHandlerHost = Object.Instantiate(_networkPrefab, Vector3.zero, Quaternion.identity);
            networkHandlerHost.GetComponent<NetworkObject>().Spawn();
        }
        
        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        [HarmonyPostfix]
        private static void Init() {
            if (_networkPrefab != null) return;
            
            _networkPrefab = Assets.Bundle.LoadAsset<GameObject>("Assets/Tarrot/Networker.prefab");
            _networkPrefab.AddComponent<Networker>();
            NetworkManager.Singleton.AddNetworkPrefab(_networkPrefab);
        }

        private static GameObject _networkPrefab;
        private static readonly int Closed = Animator.StringToHash("Closed");
        private static readonly int Damage = Animator.StringToHash("Damage");
        private static readonly int Limp = Animator.StringToHash("Limp");
    }
}