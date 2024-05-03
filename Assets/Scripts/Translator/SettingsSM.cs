using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UnityEngine;
using UI;
using Translator;
using UnityEngine.UI;
using TMPro;

namespace Settings
{
    public enum States
    {
        Home,
        PuzzleList,
        Puzzle
    }

    public struct OnVolumeSettingsChanged {}

    public class SettingsSM : MonoSingleton<SettingsSM>
    {
        public Coroutine CurrentCoroutine = null;

        private Slider mBackgroundVolumeSlider;
        private TMP_Text mBackgroundVolumeText;
        private Slider mEffectVolumeSlider;
        private TMP_Text mEffectVolumeText;

        private void Awake()
        {
            mBackgroundVolumeSlider = transform.Find("Content/Scroll View/Viewport/Content/BackgroundVolume/Slider").GetComponent<Slider>();
            mBackgroundVolumeSlider.onValueChanged.AddListener(value => {
                PlayerPrefs.SetFloat("Background Volume", value / 100f);
                TypeEventSystem.Global.Send<OnVolumeSettingsChanged>();
            });
            mBackgroundVolumeText = transform.Find("Content/Scroll View/Viewport/Content/BackgroundVolume/Value").GetComponent<TMP_Text>();

            mEffectVolumeSlider = transform.Find("Content/Scroll View/Viewport/Content/EffectVolume/Slider").GetComponent<Slider>();
            mEffectVolumeSlider.onValueChanged.AddListener(value => {
                PlayerPrefs.SetFloat("Effect Volume", value / 100f);
                TypeEventSystem.Global.Send<OnVolumeSettingsChanged>();
            });
            mEffectVolumeText = transform.Find("Content/Scroll View/Viewport/Content/EffectVolume/Value").GetComponent<TMP_Text>();

            TypeEventSystem.Global.Register<OnVolumeSettingsChanged>(e => Refresh()).UnRegisterWhenGameObjectDestroyed(gameObject);

            Initialize();
        }

        private void Initialize()
        {
            mBackgroundVolumeSlider.SetValueWithoutNotify(PlayerPrefs.HasKey("Background Volume") ? PlayerPrefs.GetFloat("Background Volume") * 100 : 80);
            mEffectVolumeSlider.SetValueWithoutNotify(PlayerPrefs.HasKey("Effect Volume") ? PlayerPrefs.GetFloat("Effect Volume") * 100 : 80);

            Refresh();
        }

        private void Refresh()
        {
            mBackgroundVolumeText.SetText(mBackgroundVolumeSlider.value.ToString());
            mEffectVolumeText.SetText(mEffectVolumeSlider.value.ToString());
        }
    }
}