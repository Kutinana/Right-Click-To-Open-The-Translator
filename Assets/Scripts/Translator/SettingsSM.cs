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

        private Button mClearPlayerPrefs;
        private Button mBackToMainMenu;

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

            mWindowModeDropdown = transform.Find("Content/Scroll View/Viewport/Content/WindowMode/Dropdown").GetComponent<TMP_Dropdown>();
            mResolutionDropdown = transform.Find("Content/Scroll View/Viewport/Content/Resolution/Dropdown").GetComponent<TMP_Dropdown>();

            mClearPlayerPrefs = transform.Find("Content/Scroll View/Viewport/Content/ClearPlayerPrefs").GetComponent<Button>();
            mClearPlayerPrefs.onClick.AddListener(() => {
                PlayerPrefs.DeleteAll();
            });
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
            mBackgroundVolumeSlider.SetValueWithoutNotify(PlayerPrefs.HasKey("Background Volume") ? PlayerPrefs.GetFloat("Background Volume") * 100 : 80);
            mEffectVolumeSlider.SetValueWithoutNotify(PlayerPrefs.HasKey("Effect Volume") ? PlayerPrefs.GetFloat("Effect Volume") * 100 : 80);

            mResolutionDropdown.ClearOptions();
            foreach (var resolution in AvailableResolutions)
            {
                mResolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{resolution.Item1}x{resolution.Item2}"));
            }
            mResolutionDropdown.SetValueWithoutNotify(PlayerPrefs.HasKey("Resolution") ? PlayerPrefs.GetInt("Resolution") : 0);
            mResolutionDropdown.RefreshShownValue();

            mResolutionDropdown.onValueChanged.AddListener(value => {
                Screen.SetResolution(AvailableResolutions[value].Item1, AvailableResolutions[value].Item2, false);
                PlayerPrefs.SetInt("Resolution", value);

                Refresh();
            });

            mWindowModeDropdown.SetValueWithoutNotify(PlayerPrefs.HasKey("Window Mode") ? PlayerPrefs.GetInt("Window Mode") : 1);
            mWindowModeDropdown.onValueChanged.AddListener(value => {
                if (value == 1)
                {
                    var width = Screen.resolutions[^1].width;
                    Screen.SetResolution(width, width / 16 * 9, true);
                }
                else
                {
                    if (PlayerPrefs.HasKey("Resolution"))
                        Screen.SetResolution(AvailableResolutions[PlayerPrefs.GetInt("Resolution")].Item1, AvailableResolutions[PlayerPrefs.GetInt("Resolution")].Item2, false);
                    else Screen.SetResolution(1920, 1080, false);
                }
                PlayerPrefs.SetInt("Window Mode", value);

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