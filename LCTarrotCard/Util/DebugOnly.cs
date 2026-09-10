using System;
using LCTarrotCard.Config;

namespace LCTarrotCard.Util {
    
    /// <summary>
    /// I use this class just in case I forget to remove some testing stuff before shipping the mod
    /// </summary>
    public static class DebugOnly {
        
        public static T OnlyIfTesting<T>(T testValue, T normalValue) {
            return ConfigManager.DebugModeSetting.Value ? testValue : normalValue;
        }

        public static T OnlyIfTesting<T>(Func<T> testFunc) {
            return ConfigManager.DebugModeSetting.Value ? testFunc() : default;
        }

    }
}