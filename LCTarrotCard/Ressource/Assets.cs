using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using LCTarrotCard.Items;
using LCTarrotCard.Util;
using Unity.Netcode;
using UnityEngine;

namespace LCTarrotCard.Ressource {
    
    public static class Assets {
        
        internal static AssetBundle Bundle;

        public static Item TarotItem;
        public static GameObject SingleTarotCard;

        public static readonly List<AudioClip> PullCardClips = new List<AudioClip>();
        public static AudioClip FoolCardSound;

        public static AudioClip GhostBreathe;

        public static class Materials {

            public static Material CardDeathMat;
            public static Material CardHangedMat;
            public static Material CardPriestessMat;
            public static Material CardDevilMat;
            public static Material CardFoolMat;
            public static Material CardHermitMat;
            public static Material CardMoonMat;
            public static Material CardSunMat;
            public static Material CardTowerMat;
            public static Material CardWheelMat;

            public static Material BurnAqua;
            public static Material BurnBlue;
            public static Material BurnGreen;
            public static Material BurnPurple;
            public static Material BurnRed;
            public static Material BurnWhite;
            public static Material BurnYellow;

        }

        public static void Load() {
            if (LoadBundle()) {
                LoadAssets();
                InitSpawns();
            }
        }

        private static bool LoadBundle() {
            Stream bundleStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LCTarrotCard.Ressource.tarrotbundle");
            if (bundleStream == null) {
                PluginLogger.Error("Cannot load the asset bundle");
                return false;
            }
            Bundle = AssetBundle.LoadFromStream(bundleStream);
            if (Bundle != null) return true;
            
            PluginLogger.Error("Cannot load the asset bundle");
            return false;
        }

        private static void LoadAssets() {
            TarotItem = LoadAsset<Item>("TarrotCard.asset");

            TarotBehaviour tarotBehaviour = TarotItem.spawnPrefab.AddComponent<TarotBehaviour>();
            tarotBehaviour.grabbable = true;
            tarotBehaviour.grabbableToEnemies = true;
            tarotBehaviour.itemProperties = TarotItem;
            
            SingleTarotCard = LoadAsset<GameObject>("single_card_prefab.prefab");
            PullCardClips.Add(LoadAsset<AudioClip>("Sound/tarot_draw1.ogg"));
            PullCardClips.Add(LoadAsset<AudioClip>("Sound/tarot_draw2.ogg"));
            PullCardClips.Add(LoadAsset<AudioClip>("Sound/tarot_draw3.ogg"));
            FoolCardSound = LoadAsset<AudioClip>("Sound/fool.ogg");
            GhostBreathe = LoadAsset<AudioClip>("Sound/breathe.ogg");
            LoadMaterials();
        }

        private static void LoadMaterials() {
            Materials.CardDeathMat = LoadAsset<Material>("Cards/DeathMat.mat");
            Materials.CardHangedMat = LoadAsset<Material>("Cards/HangedManMat.mat");
            Materials.CardPriestessMat = LoadAsset<Material>("Cards/HighPreistessMat.mat");
            Materials.CardDevilMat = LoadAsset<Material>("Cards/TheDevilMat.mat");
            Materials.CardFoolMat = LoadAsset<Material>("Cards/TheFoolMat.mat");
            Materials.CardHermitMat = LoadAsset<Material>("Cards/TheHermitMat.mat");
            Materials.CardMoonMat = LoadAsset<Material>("Cards/TheMoonMat.mat");
            Materials.CardSunMat = LoadAsset<Material>("Cards/TheSunMat.mat");
            Materials.CardTowerMat = LoadAsset<Material>("Cards/TowerMat.mat");
            Materials.CardWheelMat = LoadAsset<Material>("Cards/WheelMat.mat");

            Materials.BurnAqua = LoadAsset<Material>("Burn/AquaBurn.mat");
            Materials.BurnBlue = LoadAsset<Material>("Burn/BlueBurn.mat");
            Materials.BurnGreen = LoadAsset<Material>("Burn/GreenBurn.mat");
            Materials.BurnPurple = LoadAsset<Material>("Burn/PurpleBurn.mat");
            Materials.BurnRed = LoadAsset<Material>("Burn/RedBurn.mat");
            Materials.BurnWhite = LoadAsset<Material>("Burn/WhiteBurn.mat");
            Materials.BurnYellow = LoadAsset<Material>("Burn/YellowBurn.mat");
        }

        public static SpawnRarity TarotRarity;

        private static void InitSpawns() {
            TarotRarity = new SpawnRarity(17, TarotItem);
            TarotRarity.SetValues(vow: 19, experimentation: 12, march: 21, adamance: 22, rend: 22, dine: 23, titan: 30, artifice: 25, embrion: 20);
        }

        private static T LoadAsset<T>(string assetPath) where T : Object {
            return Bundle.LoadAsset<T>("Assets/Tarrot/" + assetPath);
        }
        
        
    }

    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class NetworkPatcher {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void RegisterNetworkPrefab() {
            NetworkManager manager = GameNetworkManager.Instance.GetComponent<NetworkManager>();
            
            manager.AddNetworkPrefab(Assets.TarotItem.spawnPrefab);
            manager.AddNetworkPrefab(Assets.SingleTarotCard);
        }
    }
    
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatcher {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void RegisterItemsPatch(StartOfRound __instance) {
            SelectableLevel[] levels = __instance.levels;
            foreach (SelectableLevel level in levels) {
                Assets.TarotRarity.ApplySpawnRarity(level);
            }
            
            
            RegisterItem(Assets.TarotItem, __instance);
        }
        
        private static void RegisterItem(Item item, StartOfRound manager) {
            if (!manager.allItemsList.itemsList.Contains(item)) {
                manager.allItemsList.itemsList.Add(item);
            }
        }
    }
}