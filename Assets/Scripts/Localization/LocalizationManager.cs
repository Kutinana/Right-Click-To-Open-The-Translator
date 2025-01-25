using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using Kuchinashi;
using QFramework;
using UI.Narration;
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
            // CurrentLanguage = Application.systemLanguage switch
            // {
            //     SystemLanguage.English => Language.en_US,
            //     SystemLanguage.Chinese => Language.zh_CN,
            //     _ => Language.zh_CN,
            // };
            CurrentLanguage = Language.zh_CN;
        }

        internal Dictionary<Language, Dictionary<string, string>> CommonStrings => ReadableData.DeSerialization<Dictionary<Language, Dictionary<string, string>>>("I18n/Common");
        public static Dictionary<string, string> GetCommonStrings() => Instance.CommonStrings.TryGetValue(Instance.CurrentLanguage, out var common) ? common : throw new System.Exception("Incomplete Localization");
        public static string GetCommonString(string key) => GetCommonStrings().TryGetValue(key, out var common) ? common : throw new System.Exception("Incomplete Localization");

        internal Dictionary<Language, LocalizedPlot> Plot => ReadableData.DeSerialization<Dictionary<Language, LocalizedPlot>>("I18n/Plot");
        public static LocalizedPlot GetPlot() => Instance.Plot.TryGetValue(Instance.CurrentLanguage, out var plot) ? plot : throw new System.Exception("Incomplete Localization");

        
        internal Dictionary<Language, Dictionary<string, List<NarrationSentence>>> Narrations => ReadableData.DeSerialization<Dictionary<Language, Dictionary<string, List<NarrationSentence>>>>("I18n/Narration");
        public static Dictionary<string, List<NarrationSentence>> GetNarrations() => Instance.Narrations.TryGetValue(Instance.CurrentLanguage, out var common) ? common : throw new System.Exception("Incomplete Localization");
        public static List<NarrationSentence> GetNarration(string key) => GetNarrations().TryGetValue(key, out var common) ? common : throw new System.Exception("Incomplete Localization");
    }

    public struct NarrationSentence
    {
        public NarrationType type;
        public string narrator;
        public string content;
    }

    public class LocalizationHelper
    {
        public static string Get(Dictionary<Language, string> _key)
        {
            var lang = LocalizationManager.Instance.CurrentLanguage;
            if (_key != null && _key.ContainsKey(lang))
            {
                return _key[lang].Replace("\\n", "\n");
            }
            return _key.First().Value.Replace("\\n", "\n");
        }

        public static string Get(Dictionary<Language, string> _key, Language lang)
        {
            if (_key != null && _key.ContainsKey(lang))
            {
                return _key[lang].Replace("\\n", "\n");
            }
            return _key.First().Value.Replace("\\n", "\n");
        }

        public static string Get(SerializableDictionary<Language, string> _key)
        {
            var lang = LocalizationManager.Instance.CurrentLanguage;
            if (_key != null && _key.ContainsKey(lang))
            {
                return _key[lang].Replace("\\n", "\n");
            }
            return _key.First().Value.Replace("\\n", "\n");
        }

        public static string Get(SerializableDictionary<Language, string> _key, Language lang)
        {
            if (_key != null && _key.ContainsKey(lang))
            {
                return _key[lang].Replace("\\n", "\n");
            }
            return _key.First().Value.Replace("\\n", "\n");
        }

        public static string Get(string commonKey, params string [] param)
        {
            var origin = LocalizationManager.GetCommonString(commonKey);
            try
            {
                for (var i = 0; i < param.Length; i++)
                {
                    origin = origin.Replace($"{{{i}}}", param[i]);
                }
                return origin.Replace("\\n", "\n");
            }
            catch
            {
                return origin.Replace("\\n", "\n");
            }
        }
    }
}
