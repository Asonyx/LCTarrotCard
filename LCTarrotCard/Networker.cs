using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using HarmonyLib;
using LCTarrotCard.Patches;
using LCTarrotCard.Ressource;
using LCTarrotCard.Util;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

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
            foreach (HangarShipDoor shipDoor in FindObjectsOfType<HangarShipDoor>()) {
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
            Transform spawnTransform = Helper.GetRandomSpawnLocation();
            if (spawnTransform == null) {
                PluginLogger.Error("No spawn location found");
                return;
            }
            Vector3 spawnPos = spawnTransform.position;
            
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
                    if (jester.currentBehaviourStateIndex == 2) return;
                    foundJester = true;
                }
            }

            if (!foundJester) {
                Transform spawnTransform = Helper.GetRandomSpawnLocation();
                if (spawnTransform == null) {
                    PluginLogger.Error("No spawn location found");
                    return;
                }
                Vector3 spawnPos = spawnTransform.position;
            
                GameObject jester = Instantiate(Helper.Enemies.Jester.enemyPrefab, spawnPos, Quaternion.identity);
                jester.GetComponentInChildren<NetworkObject>().Spawn(true);
                RoundManager.Instance.SpawnedEnemies.Add(jester.GetComponent<JesterAI>());
                jester.GetComponent<JesterAI>().SetEnemyOutside();
                jester.GetComponent<JesterAI>().SwitchToBehaviourState(1);
            }

            StartCoroutine(PopDelay(popDelay));

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

        [ServerRpc(RequireOwnership = false)]
        public void SpawnGiantOrDogServerRpc(int amount) {
            for (int i = 0; i < amount; i++) {
                Transform spawnTransform = Helper.GetRandomSpawnLocation(false);
                if (spawnTransform == null) continue;
                
                Vector3 spawnPos = spawnTransform.position;
                
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
                PluginLogger.Debug("Enemy is outside ? " + enemy.isOutside + " player inside ? " + inside);
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
            AudioSource.PlayClipAtPoint(Assets.GhostBreathe, position, 4);
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

            
            
            playerControllerB.StopSinkingServerRpc();
            if (playerControllerB.currentTriggerInAnimationWith) {
                playerControllerB.currentTriggerInAnimationWith.CancelAnimationExternally();
            }
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
            if (FindObjectOfType<AudioReverbPresets>()) {
                if (playerControllerB.isInsideFactory) {
                    FindObjectOfType<AudioReverbPresets>().audioPresets[2].ChangeAudioReverbForPlayer(playerControllerB);
                }
                else FindObjectOfType<AudioReverbPresets>().audioPresets[3].ChangeAudioReverbForPlayer(playerControllerB);
            }
        }
        
        
        
        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerHealthServerRpc(int playerId, int health) {
            SetPlayerHealthClientRpc(playerId, health);
        }
        
        [ClientRpc]
        public void SetPlayerHealthClientRpc(int playerId, int health) {
            if (health <= 0) health = 1;
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerId];
            int previousHealth = player.health;
            player.health = health;

            if (player.IsOwner) {
                HUDManager.Instance.UpdateHealthUI(health, previousHealth > health);
                if (health <= 20 && !player.criticallyInjured) {
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
                    player.MakeCriticallyInjured(true);
                }
                else if (player.criticallyInjured && health > 20) {
                    player.MakeCriticallyInjured(false);
                }
                
            }
            
            if (previousHealth > health) return;
            
            if (player.IsOwner) {
                StartCoroutine(SetPlayerUI(player, health <= 20));
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
                StartCoroutine(SetPlayerUI(player));
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
            PluginLogger.Debug("extra life " + StartOfRound.Instance.allPlayerScripts[playerId].playerUsername);
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
                StartCoroutine(SetPlayerUI(player));
                HUDManager.Instance.UpdateHealthUI(100, false);
            }
            if (StartOfRound.Instance.allPlayerScripts[playerId].IsOwner) {
                HUDManager.Instance.DisplayTip("Extra life", "Thanks to the high priestess card, you are given an extra chance to live");
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void LittleGirlChaseServerRpc(int playerId) {
            DressGirlAI dressGirl = null;
            foreach (EnemyAI enemy in RoundManager.Instance.SpawnedEnemies) {
                if (!(enemy is DressGirlAI littleGirl) || littleGirl.isEnemyDead || !littleGirl.IsSpawned) continue;
                dressGirl = littleGirl;
                break;
            }

            if (dressGirl == null) {
                Transform spawnTransform = Helper.GetRandomSpawnLocation();
                if (spawnTransform == null) {
                    PluginLogger.Error("No spawn location found");
                    return;
                }
                Vector3 spawnPos = spawnTransform.position;
            
                GameObject girl = Instantiate(Helper.Enemies.DressGirl.enemyPrefab, spawnPos, Quaternion.identity);
                girl.GetComponentInChildren<NetworkObject>().Spawn(true);
                RoundManager.Instance.SpawnedEnemies.Add(girl.GetComponent<DressGirlAI>());
                girl.GetComponent<DressGirlAI>().SetEnemyOutside();
                dressGirl = girl.GetComponent<DressGirlAI>();
            }
            dressGirl.hauntingPlayer = StartOfRound.Instance.allPlayerScripts[playerId];
            
            StartCoroutine(DelaySetGirlProperties(playerId, dressGirl));
        }
        
        private IEnumerator DelaySetGirlProperties(int playerId, DressGirlAI dressGirl) {
            yield return new WaitUntil(() => dressGirl.agent != null);
            LittleGirlChaseClientRpc(playerId, dressGirl.NetworkObjectId);
        }

        [ClientRpc]
        public void LittleGirlChaseClientRpc(int playerId, ulong networkId) {
            DressGirlAI dressGirl = RoundManager.Instance.SpawnedEnemies.FirstOrDefault(enemy => enemy.NetworkObjectId == networkId) as DressGirlAI;
            if (dressGirl == null) return;
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerId];

            if ((int)GameNetworkManager.Instance.localPlayerController.playerClientId == playerId) {
                Vector3 hauntPos = player.transform.position + player.transform.forward * 5;
                TeleportEnemy(dressGirl, hauntPos);
                
                dressGirl.EnableEnemyMesh(true, true);
                
                dressGirl.SwitchToBehaviourStateOnLocalClient(1);
                dressGirl.staringInHaunt = false;
                dressGirl.disappearingFromStare = false;
                dressGirl.agent.speed = 5.25f;
                dressGirl.creatureAnimator.SetBool("Walk", true);
                dressGirl.timer = 0f;
                dressGirl.SetMovingTowardsTargetPlayer(player);
                dressGirl.moveTowardsDestination = true;
                dressGirl.creatureVoice.volume = 1f;
                dressGirl.creatureVoice.clip = dressGirl.breathingSFX;
                dressGirl.creatureVoice.Play();
                dressGirl.SFXVolumeLerpTo = 1f;
                dressGirl.creatureSFX.volume = 1f;
                if (!DressGirlAIPatch.ChaseTimes.ContainsKey(dressGirl.NetworkObjectId))
                    DressGirlAIPatch.ChaseTimes.Add(dressGirl.NetworkObjectId, Time.time);
            }
            dressGirl.hauntingPlayer = player;
            if (!DressGirlAIPatch.ChasingGirls.ContainsKey(dressGirl.NetworkObjectId))
                DressGirlAIPatch.ChasingGirls.Add(dressGirl.NetworkObjectId, playerId);
        }

        private static IEnumerator SetPlayerUI(PlayerControllerB player, bool critical = false) {
            yield return new WaitForFixedUpdate();
            HUDManager.Instance.UpdateHealthUI(player.health, false);
            player.MakeCriticallyInjured(critical);
        }

        [ServerRpc(RequireOwnership = false)]
        public void AgroBrackenOrSpawnServerRpc(int playerId) {
            if (!StartOfRound.Instance.allPlayerScripts[playerId].isPlayerControlled) return;
            
            FlowermanAI bracken = null;
            foreach (EnemyAI enemy in RoundManager.Instance.SpawnedEnemies) {
                if (!(enemy is FlowermanAI flowerman) || flowerman.isEnemyDead || !flowerman.IsSpawned) continue;
                bracken = flowerman;
                break;
            }
            
            if (bracken == null) {
                Transform spawnTransform = Helper.GetRandomSpawnLocation();
                if (spawnTransform == null) {
                    PluginLogger.Error("No spawn location found");
                    return;
                }
                Vector3 spawnPos = spawnTransform.position;
            
                GameObject flowerman = Instantiate(Helper.Enemies.FlowerMan.enemyPrefab, spawnPos, Quaternion.identity);
                flowerman.GetComponentInChildren<NetworkObject>().Spawn(true);
                RoundManager.Instance.SpawnedEnemies.Add(flowerman.GetComponent<FlowermanAI>());
                flowerman.GetComponent<FlowermanAI>().SetEnemyOutside();
                bracken = flowerman.GetComponent<FlowermanAI>();
                bracken.SyncPositionToClients();
            }
            
            SyncBrackenPropertiesClientRpc(playerId, bracken.NetworkObjectId);
        }
        
        [ClientRpc]
        public void SyncBrackenPropertiesClientRpc(int playerId, ulong networkId) {
            FlowermanAI bracken = RoundManager.Instance.SpawnedEnemies.FirstOrDefault(enemy => enemy.NetworkObjectId == networkId) as FlowermanAI;
            if (bracken == null) return;
            if (!FlowermanAIPatch.AngryBrackens.ContainsKey(bracken.NetworkObjectId)) 
                FlowermanAIPatch.AngryBrackens.Add(bracken.NetworkObjectId, new KeyValuePair<int, float>(playerId, Time.time));
            StartCoroutine(SetBrackenProperties(bracken, playerId));
        }
        
        private IEnumerator SetBrackenProperties(FlowermanAI bracken, int playerId) {
            yield return new WaitUntil(() => bracken.agent != null);
            bracken.SwitchToBehaviourState(2);
            bracken.targetPlayer = StartOfRound.Instance.allPlayerScripts[playerId];
            bracken.angerMeter = 2.5f;
        }

        [ServerRpc(RequireOwnership = false)]
        public void DogChaseOrSpawnServerRpc(int playerId) {
            if (!StartOfRound.Instance.allPlayerScripts[playerId].isPlayerControlled) return;
            MouthDogAI dog = null;
            foreach (EnemyAI enemy in RoundManager.Instance.SpawnedEnemies) {
                if (!(enemy is MouthDogAI mouthDog) || mouthDog.isEnemyDead || !mouthDog.IsSpawned) continue;
                dog = mouthDog;
                break;
            }

            if (dog == null) {
                Transform spawnTransform = Helper.GetRandomSpawnLocation(false);
                if (spawnTransform == null) {
                    PluginLogger.Error("No spawn location found");
                    return;
                }
                Vector3 spawnPos = spawnTransform.position;
            
                GameObject mouthDog = Instantiate(Helper.Enemies.MouthDog.enemyPrefab, spawnPos, Quaternion.identity);
                mouthDog.GetComponentInChildren<NetworkObject>().Spawn(true);
                RoundManager.Instance.SpawnedEnemies.Add(mouthDog.GetComponent<MouthDogAI>());
                mouthDog.GetComponent<MouthDogAI>().SetEnemyOutside();
                dog = mouthDog.GetComponent<MouthDogAI>();
                dog.SyncPositionToClients();
            }
            SyncDogPropertiesClientRpc(playerId, dog.NetworkObjectId);
        }
        
        [ClientRpc]
        public void SyncDogPropertiesClientRpc(int playerId, ulong networkId) {
            MouthDogAI dog = RoundManager.Instance.SpawnedEnemies.FirstOrDefault(enemy => enemy.NetworkObjectId == networkId) as MouthDogAI;
            if (dog == null) return;
            if (!MouthDogAIPatch.ChasingDogs.ContainsKey(dog.NetworkObjectId)) MouthDogAIPatch.ChasingDogs.Add(dog.NetworkObjectId, playerId);
            StartCoroutine(SetDogProperties(dog, StartOfRound.Instance.allPlayerScripts[playerId]));
        }
        
        private IEnumerator SetDogProperties(MouthDogAI dog, PlayerControllerB player) {
            yield return new WaitUntil(() => dog.agent != null);
            dog.ReactToOtherDogHowl(player.transform.position);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetDogPathServerRpc(ulong dogId, int targetedPlayer) {
            SetDogPathClientRpc(dogId, targetedPlayer);
        }
        
        [ClientRpc]
        public void SetDogPathClientRpc(ulong dogId, int targetedPlayer) {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[targetedPlayer];
            if (!player.isPlayerControlled) return;
            MouthDogAI dog = RoundManager.Instance.SpawnedEnemies.FirstOrDefault(enemy => enemy.NetworkObjectId == dogId) as MouthDogAI;
            if (dog == null) return;
            dog.ReactToOtherDogHowl(player.transform.position);
            dog.suspicionLevel = 12;
        }

        [ServerRpc(RequireOwnership = false)]
        public void ShipLeaveEarlyServerRpc() {
            ShipLeaveEarlyClientRpc();
        }
        
        [ClientRpc]
        public void ShipLeaveEarlyClientRpc() {
            TimeOfDay.Instance.votesForShipToLeaveEarly = StartOfRound.Instance.connectedPlayersAmount + 1 - StartOfRound.Instance.livingPlayers;
            TimeOfDay.Instance.SetShipLeaveEarlyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void GiantChasePlayerOrSpawnServerRpc(int playerId) {
            PlayerControllerB chasedPlayer = StartOfRound.Instance.allPlayerScripts[playerId];
            if (!chasedPlayer.isPlayerControlled) return;
            ForestGiantAI giant = null;
            foreach (EnemyAI enemy in RoundManager.Instance.SpawnedEnemies) {
                if (!(enemy is ForestGiantAI forestGiant) || forestGiant.isEnemyDead || !forestGiant.IsSpawned) continue;
                giant = forestGiant;
                break;
            }

            if (giant == null) {
                Transform spawnTransform = Helper.GetRandomSpawnLocation(false);
                if (spawnTransform == null) {
                    PluginLogger.Error("No spawn location found");
                    return;
                }
                Vector3 spawnPos = spawnTransform.position;
                
                GameObject forestGiant = Instantiate(Helper.Enemies.ForestGiant.enemyPrefab, spawnPos, Quaternion.identity);
                forestGiant.GetComponentInChildren<NetworkObject>().Spawn(true);
                RoundManager.Instance.SpawnedEnemies.Add(forestGiant.GetComponent<ForestGiantAI>());
                forestGiant.GetComponent<ForestGiantAI>().SetEnemyOutside();
                giant = forestGiant.GetComponent<ForestGiantAI>();
                giant.SyncPositionToClients();
            }
            GiantSetPropertiesClientRpc(playerId, giant.NetworkObjectId);
        }
        
        [ClientRpc]
        public void GiantSetPropertiesClientRpc(int playerId, ulong networkId) {
            ForestGiantAI giant = RoundManager.Instance.SpawnedEnemies.FirstOrDefault(enemy => enemy.NetworkObjectId == networkId) as ForestGiantAI;
            if (giant == null) return;
            PluginLogger.Debug("Adding giant to chasing list");
            if (!ForestGiantAIPatch.ChasingGiants.ContainsKey(giant.NetworkObjectId)) ForestGiantAIPatch.ChasingGiants.Add(giant.NetworkObjectId, playerId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void BirdChaseOrSpawnServerRpc(int playerId) {
            PlayerControllerB chasedPlayer = StartOfRound.Instance.allPlayerScripts[playerId];
            if (!chasedPlayer.isPlayerControlled) return;
            RadMechAI bird = null;
            foreach (EnemyAI enemy in RoundManager.Instance.SpawnedEnemies) {
                if (!(enemy is RadMechAI radMech) || radMech.isEnemyDead || !radMech.IsSpawned) continue;
                bird = radMech;
                break;
            }

            if (bird == null) {
                Transform spawnTransform = Helper.GetRandomSpawnLocation(false);
                if (spawnTransform == null) {
                    PluginLogger.Error("No spawn location found");
                    return;
                }
                Vector3 spawnPos = spawnTransform.position;
                
                GameObject radMech = Instantiate(Helper.Enemies.RadMech.enemyPrefab, spawnPos, Quaternion.identity);
                radMech.GetComponentInChildren<NetworkObject>().Spawn(true);
                RoundManager.Instance.SpawnedEnemies.Add(radMech.GetComponent<RadMechAI>());
                radMech.GetComponent<RadMechAI>().SetEnemyOutside();
                bird = radMech.GetComponent<RadMechAI>();
                bird.SyncPositionToClients();
            }
            BirdSetPropertiesClientRpc(playerId, bird.NetworkObjectId);
        }
        
        [ClientRpc]
        public void BirdSetPropertiesClientRpc(int playerId, ulong networkId) {
            RadMechAI bird = RoundManager.Instance.SpawnedEnemies.FirstOrDefault(enemy => enemy.NetworkObjectId == networkId) as RadMechAI;
            if (bird == null) return;
            PluginLogger.Debug("Adding bird to chasing list");
            if (!RadMechAIPatch.ChasingBirds.ContainsKey(bird.NetworkObjectId)) RadMechAIPatch.ChasingBirds.Add(bird.NetworkObjectId, playerId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void TeleportOrSpawnWornServerRpc(int playerId) {
            PlayerControllerB chasedPlayer = StartOfRound.Instance.allPlayerScripts[playerId];
            if (!chasedPlayer.isPlayerControlled) return;
            SandWormAI worm = null;
            foreach (EnemyAI enemy in RoundManager.Instance.SpawnedEnemies) {
                if (!(enemy is SandWormAI sandWorm) || sandWorm.isEnemyDead || !sandWorm.IsSpawned) continue;
                worm = sandWorm;
                break;
            }

            if (worm == null) {
                Transform spawnTransform = Helper.GetRandomSpawnLocation(false);
                if (spawnTransform == null) {
                    PluginLogger.Error("No spawn location found");
                    return;
                }
                Vector3 spawnPos = spawnTransform.position;
                
                GameObject sandWorm = Instantiate(Helper.Enemies.SandWorm.enemyPrefab, spawnPos, Quaternion.identity);
                sandWorm.GetComponentInChildren<NetworkObject>().Spawn(true);
                RoundManager.Instance.SpawnedEnemies.Add(sandWorm.GetComponent<SandWormAI>());
                sandWorm.GetComponent<SandWormAI>().SetEnemyOutside();
                worm = sandWorm.GetComponent<SandWormAI>();
                worm.SyncPositionToClients();
            }

            StartCoroutine(TeleportEnemyWhenReady(worm, chasedPlayer.transform.position, outside: true));
        }

        public static IEnumerator TeleportEnemyWhenReady(EnemyAI enemy, Vector3 position, bool exitEnter = false,
            bool outside = false) {
            yield return new WaitUntil(() => enemy.agent != null);
            TeleportEnemy(enemy, position, exitEnter, outside);
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