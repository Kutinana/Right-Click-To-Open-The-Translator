using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UnityEngine;
using UI;
using Translator;
using UnityEngine.UI;
using TMPro;
using Cameras;

namespace Settings
{
    public struct OnVolumeSettingsChanged {}

    public class SettingsSM : MonoSingleton<SettingsSM>
    {
        public Coroutine CurrentCoroutine = null;
        public static readonly List<(int, int)> AvailableResolutions = new()
        {
            (1280, 720),
            (1920, 1080),
            (2560, 1440)
        };

        #region Volume Settings

        private Slider mBackgroundVolumeSlider;
        private TMP_Text mBackgroundVolumeText;
        private Slider mEffectVolumeSlider;
        private TMP_Text mEffectVolumeText;

        #endregion

        #region Graphic Settings

        private TMP_Dropdown mWindowModeDropdown;
        private TMP_Dropdown mResolutionDropdown;

        #endregion

        private Button mBackToMainMenu;

        private void Awake()
        {
            mBackgroundVolumeSlider = transform.Find("Content/Scroll View/Viewport/Content/BackgroundVolume/Slider").GetComponent<Slider>();
            mBackgroundVolumeSlider.onValueChanged.AddListener(value => {
                UserConfig.Write<float>("Background Volume", value / 100f);
                TypeEventSystem.Global.Send<OnVolumeSettingsChanged>();
            });
            mBackgroundVolumeText = transform.Find("Content/Scroll View/Viewport/Content/BackgroundVolume/Value").GetComponent<TMP_Text>();

            mEffectVolumeSlider = transform.Find("Content/Scroll View/Viewport/Content/EffectVolume/Slider").GetComponent<Slider>();
            mEffectVolumeSlider.onValueChanged.AddListener(value => {
                UserConfig.Write<float>("Effect Volume", value / 100f);
                TypeEventSystem.Global.Send<OnVolumeSettingsChanged>();
            });
            mEffectVolumeText = transform.Find("Content/Scroll View/Viewport/Content/EffectVolume/Value").GetComponent<TMP_Text>();

            mWindowModeDropdown = transform.Find("Content/Scroll View/Viewport/Content/WindowMode/Dropdown").GetComponent<TMP_Dropdown>();
            mResolutionDropdown = transform.Find("Content/Scroll View/Viewport/Content/Resolution/Dropdown").GetComponent<TMP_Dropdown>();

            mBackToMainMenu = transform.Find("Content/Scroll View/Viewport/Content/BackToMainMenu").GetComponent<Button>();
            mBackToMainMenu.onClick.AddListener(() => {
                SceneControl.SceneControl.SwitchSceneWithoutConfirm("EmptyScene");
                TranslatorCanvasManager.StartMainMenu();
                TranslatorSM.StateMachine.ChangeState(States.Off);
            });

            TypeEventSystem.Global.Register<OnVolumeSettingsChanged>(e => Refresh()).UnRegisterWhenGameObjectDestroyed(gameObject);

            Initialize();
        }

        private void Initialize()
        {
            mBackgroundVolumeSlider.SetValueWithoutNotify(Mathf.RoundToInt(UserConfig.ReadWithDefaultValue<float>("Background Volume", 0.8f) * 100));
            mEffectVolumeSlider.SetValueWithoutNotify(Mathf.RoundToInt(UserConfig.ReadWithDefaultValue<float>("Effect Volume", 0.8f) * 100));

            mResolutionDropdown.ClearOptions();
            foreach (var resolution in AvailableResolutions)
            {
                mResolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{resolution.Item1}x{resolution.Item2}"));
            }
            mResolutionDropdown.SetValueWithoutNotify(UserConfig.ReadWithDefaultValue<int>("Resolution", 0));
            mResolutionDropdown.RefreshShownValue();

            mResolutionDropdown.onValueChanged.AddListener(value => {
                Screen.SetResolution(AvailableResolutions[value].Item1, AvailableResolutions[value].Item2, false);
                UserConfig.Write<int>("Resolution", value);

                Refresh();
            });

            mWindowModeDropdown.SetValueWithoutNotify(UserConfig.ReadWithDefaultValue<int>("Window Mode", 1));
            mWindowModeDropdown.onValueChanged.AddListener(value => {
                if (value == 1)
                {
                    var width = Screen.resolutions[^1].width;
                    Screen.SetResolution(width, width / 16 * 9, true);
                }
                else
                {
                    if (UserConfig.TryRead<int>("Resolution", out var res))
                        Screen.SetResolution(AvailableResolutions[res].Item1, AvailableResolutions[res].Item2, false);
                    else Screen.SetResolution(1920, 1080, false);
                }
                UserConfig.Write<int>("Window Mode", value);

                Refresh();
            });

            Refresh();
        }

        private void Refresh()
        {
            mBackgroundVolumeText.SetText(mBackgroundVolumeSlider.value.ToString());
            mEffectVolumeText.SetText(mEffectVolumeSlider.value.ToString());

            mResolutionDropdown.interactable = mWindowModeDropdown.value == 0;
        }
    }
}