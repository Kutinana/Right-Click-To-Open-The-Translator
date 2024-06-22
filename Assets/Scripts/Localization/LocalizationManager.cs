using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UnityEngine;

namespace Localization
{
    public enum Language
    {
        zh_CN,
        en_US,
        ja_JP
    }

    public class LocalizationManager : ISingleton
    {
        private LocalizationManager() { }
        public Language CurrentLanguage { get; set; } = Language.zh_CN;

        public static LocalizationManager Instance => SingletonProperty<LocalizationManager>.Instance;
        public void OnSingletonInit()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    CurrentLanguage = Language.en_US;
                    break;
                case SystemLanguage.Chinese:
                    CurrentLanguage = Language.zh_CN;
                    break;
                default:
                    CurrentLanguage = Language.zh_CN;
                    break;
            }
        }

        internal Dictionary<Language, Dictionary<string, string>> CommonStrings => ReadableData.DeSerialization<Dictionary<Language, Dictionary<string, string>>>("I18n/Common");
        public static Dictionary<string, string> GetCommonStrings() => Instance.CommonStrings.TryGetValue(Instance.CurrentLanguage, out var common) ? common : throw new System.Exception("Incomplete Localization");
        public static string GetCommonString(string key) => GetCommonStrings().TryGetValue(key, out var common) ? common : throw new System.Exception("Incomplete Localization");

        internal Dictionary<Language, LocalizedPlot> Plot => ReadableData.DeSerialization<Dictionary<Language, LocalizedPlot>>("I18n/Plot");
        public static LocalizedPlot GetPlot() => Instance.Plot.TryGetValue(Instance.CurrentLanguage, out var plot) ? plot : throw new System.Exception("Incomplete Localization");

        
        internal Dictionary<Language, Dictionary<string, List<string>>> Narrations => ReadableData.DeSerialization<Dictionary<Language, Dictionary<string, List<string>>>>("I18n/Narration");
        public static Dictionary<string, List<string>> GetNarrations() => Instance.Narrations.TryGetValue(Instance.CurrentLanguage, out var common) ? common : throw new System.Exception("Incomplete Localization");
        public static List<string> GetNarration(string key) => GetNarrations().TryGetValue(key, out var common) ? common : throw new System.Exception("Incomplete Localization");
    }
}
