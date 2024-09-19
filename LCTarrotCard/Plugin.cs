using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using LCTarrotCard.Cards;
using LCTarrotCard.Ressource;
using LCTarrotCard.Util;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LCTarrotCard {
    
    [BepInPlugin(PluginConstants.PLUGIN_GUID, PluginConstants.PLUGIN_NAME, PluginConstants.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {

        public static Plugin Instance;

        private Harmony harmony;

        public ManualLogSource logger;
        private void Awake() {
            if (Instance != null) return;
            Instance = this;
            logger = BepInEx.Logging.Logger.CreateLogSource(PluginConstants.PLUGIN_GUID);
            harmony = new Harmony(PluginConstants.PLUGIN_GUID);
            
            Assets.Load();

            AllCards.Init();
            
            harmony.PatchAll();
            NetcodePatcher();
            
        }

        internal static KeyboardShortcut DebugBtn = new KeyboardShortcut(KeyCode.BackQuote);
        
        
        private static void NetcodePatcher()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (MethodInfo method in methods)
                {
                    object[] attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class Test {
        
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void TestPatch(PlayerControllerB __instance) {
            if (PluginConstants.DEBUG_MODE && __instance.IsOwner && Plugin.DebugBtn.IsDown()) {
                PluginLogger.Debug("Spawning item");
                GameObject obj = UnityEngine.Object.Instantiate(Assets.TarotItem.spawnPrefab, __instance.transform.position, __instance.transform.rotation);
                
                GrabbableObject component = obj.GetComponent<GrabbableObject>();
                component.transform.rotation = Quaternion.Euler(component.itemProperties.restingRotation);
                component.fallTime = 0f;
                component.scrapValue = 1;
                NetworkObject no = obj.GetComponent<NetworkObject>();
                no.Spawn();
                component.FallToGround(true);
            }

           
        }

    }

    public static class PluginLogger {
        public static bool ShowDebug = true;
        
        public static void Debug(object o) {
            if (ShowDebug) Plugin.Instance.logger.LogDebug("[DEBUG] " + o);
        }

        public static void Info(object o) {
            Plugin.Instance.logger.LogInfo(o);
        }

        public static void Error(object o) {
            Plugin.Instance.logger.LogError("[ERROR] " + o);
        }

        public static void Warning(object o) {
            Plugin.Instance.logger.LogWarning("[WARNING] " + o);
        }
    }

    public static class PluginConstants {
        public const string PLUGIN_GUID = "LCTarotCard";
        public const string PLUGIN_NAME = "Phasmophobia Tarot Card";
        public const string PLUGIN_VERSION = "1.0.3";
        internal const bool DEBUG_MODE = false;
    }
}