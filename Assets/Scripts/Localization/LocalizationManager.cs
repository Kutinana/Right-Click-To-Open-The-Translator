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

        internal Dictionary<Language, LocalizedPlot> Plot => ReadableData.DeSerialization<Dictionary<Language, LocalizedPlot>>("I18n/Plot");
        public static LocalizedPlot GetPlot() => Instance.Plot.TryGetValue(Instance.CurrentLanguage, out var plot) ? plot : throw new System.Exception("Incomplete Localization");
    }
}
