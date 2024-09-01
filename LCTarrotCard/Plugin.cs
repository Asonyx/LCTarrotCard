using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace LCTarrotCard {
    
    [BepInPlugin(PluginConstants.PLUGIN_GUID, PluginConstants.PLUGIN_NAME, PluginConstants.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {

        public static Plugin Instance;

        internal Harmony harmony;

        public ManualLogSource logger;
        private void Awake() {
            if (Instance != null) return;
            Instance = this;
            logger = BepInEx.Logging.Logger.CreateLogSource(PluginConstants.PLUGIN_GUID);
            harmony = new Harmony(PluginConstants.PLUGIN_GUID);
            
            harmony.PatchAll();
        }

        public static void Debug(object o) {
            Instance.logger.LogDebug(o);
        }
    }

    public static class PluginConstants {
        public const string PLUGIN_GUID = "LCTarrotCard";
        public const string PLUGIN_NAME = "Phasmophobia Tarrot Card";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}