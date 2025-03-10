using System.Collections;
using System.Collections.Generic;
using Cameras;
using Kuchinashi;
using QFramework;
using Translator;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DataSystem;
using Settings;
using UI;
using TMPro;
using System;
using Kuchinashi.DataSystem;
using Localization;

namespace StartScene
{
    public class StartSceneController : MonoSingleton<StartSceneController>
    {
        public static string Version => "0.3.2";

        private CanvasGroup mFirstSplashCanvasGroup;
        private CanvasGroup mSecondSplashCanvasGroup;

        private CanvasGroup mInitialCGCanvasGroup;
        private Image mInitialCG;
        private CanvasGroup mInitialCGFrontCanvasGroup;
        private Image mInitialCGFront;

        private CanvasGroup mInitialCGTextACG;
        private TMP_Text mInitialCGTextA;
        private CanvasGroup mInitialCGTextBCG;
        private TMP_Text mInitialCGTextB;
        private CanvasGroup mInitialVideoCanvasGroup;

        private VideoPlayer mInitialVideoPlayer;
        private VideoPlayer mSecondTimeVideoPlayer;

        public List<Sprite> InitialCGs;
        private List<VideoClip> InitialClips = new();

        private void Awake()
        {
            ResKit.Init();

            mFirstSplashCanvasGroup = transform.Find("FirstSplash").GetComponent<CanvasGroup>();
            mFirstSplashCanvasGroup.alpha = 0;
            mSecondSplashCanvasGroup = transform.Find("SecondSplash").GetComponent<CanvasGroup>();
            mSecondSplashCanvasGroup.alpha = 0;

            mInitialCGCanvasGroup = transform.Find("FirstTimeCG/Image").GetComponent<CanvasGroup>();
            mInitialCGCanvasGroup.alpha = 0;
            mInitialCGFrontCanvasGroup = transform.Find("FirstTimeCG/ImageFront").GetComponent<CanvasGroup>();
            mInitialCGFront = transform.Find("FirstTimeCG/ImageFront").GetComponent<Image>();
            mInitialCG = transform.Find("FirstTimeCG/Image").GetComponent<Image>();

            mInitialCGTextACG = transform.Find("FirstTimeCG/TextA").GetComponent<CanvasGroup>();
            mInitialCGTextA = transform.Find("FirstTimeCG/TextA/Text").GetComponent<TMP_Text>();
            mInitialCGTextBCG = transform.Find("FirstTimeCG/TextB").GetComponent<CanvasGroup>();
            mInitialCGTextB = transform.Find("FirstTimeCG/TextB/Text").GetComponent<TMP_Text>();

            mInitialVideoCanvasGroup = transform.Find("FirstTimeVideo").GetComponent<CanvasGroup>();
            mInitialVideoCanvasGroup.alpha = 0;

            mInitialVideoPlayer = transform.Find("FirstTimeVideo/RawImage").GetComponent<VideoPlayer>();
            mSecondTimeVideoPlayer = transform.Find("SecondSplash/RawImage").GetComponent<VideoPlayer>();

            InitialSettings();

            TranslatorSM.CanActivate = false;

            if (GameProgressData.Instance.Dictionary.Count != 0)
            {
                NormalEnterGame();
            }
            else
            {
                Debug.Log("Init");
                FirstTimeEnterGame();
            }
        }

        private void FirstTimeEnterGame()
        {

# if UNITY_EDITOR
            TranslatorCanvasManager.StartMainMenu();
            TranslatorSM.CanActivate = true;
# else

            var resLoader = ResLoader.Allocate();
            switch (LocalizationManager.Instance.CurrentLanguage)
            {
                case Localization.Language.en_US:
                UserDictionary.WriteInAndSave(new Dictionary<string, string>() {
                        {"la", "To"}, {"schl", "Click"}, {"rec", "Right"}, {"offen", "Start"}, {"masc", "Machine"}, {"ubrs", "Translation"}, {"geb", "Game"}
                    });

                    InitialClips = new() {
                        resLoader.LoadSync<VideoClip>("videos", "opening_part1_en"),
                        resLoader.LoadSync<VideoClip>("videos", "opening_part2"),
                        resLoader.LoadSync<VideoClip>("videos", "opening_part3"),
                        resLoader.LoadSync<VideoClip>("videos", "coding effect_long_en")
                    };
                    break;
                case Localization.Language.zh_CN:
                    UserDictionary.WriteInAndSave(new Dictionary<string, string>() {
                        {"la", "来"}, {"schl", "按键"}, {"rec", "右"}, {"offen", "启动"}, {"masc", "机器"}, {"ubrs", "翻译"}, {"geb", "游戏"}
                    });

                    InitialClips = new() {
                        resLoader.LoadSync<VideoClip>("videos", "opening_part1_cn"),
                        resLoader.LoadSync<VideoClip>("videos", "opening_part2"),
                        resLoader.LoadSync<VideoClip>("videos", "opening_part3"),
                        resLoader.LoadSync<VideoClip>("videos", "coding effect_long_cn")
                    };
                    break;
            }
            List<string> ids = new List<string>() {"quit", "settings", "credit"};
            UserDictionary.Unlock(ids);

            TypeEventSystem.Global.Send<OnCharacterRefreshEvent>();

            ActionKit.Sequence()
                .Delay(1f)
                .Callback(() => mFirstSplashCanvasGroup.gameObject.SetActive(true))
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mFirstSplashCanvasGroup, 1f)))
                .Delay(3f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mFirstSplashCanvasGroup, 0f)))
                .Delay(1f)
                .Coroutine(InitialCG)
                .Delay(1f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mInitialVideoCanvasGroup, 1f)))
                .Coroutine(InitialPerformance)
                .Callback(() => TranslatorCanvasManager.StartMainMenu())
                // .Callback(() => SceneControl.SceneControl.SwitchSceneWithoutConfirm("TestScene"))
                .Callback(() => TranslatorSM.CanActivate = true)
                .Callback(() => TranslatorSM.StateMachine.ChangeState(States.Translation))
                .Coroutine(TranslatorSM.Instance.StartTutorialCoroutine)
                .Delay(1f)
                .Start(this);

# endif

            PlayerPrefs.SetInt("Played", 1);
        }

        private IEnumerator InitialCG()
        {

# if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Escape)) yield break;
#endif

            yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 0f);
            AudioMng.Instance.PlayAmbient();
            AudioMng.Instance.D_PlayBGM("BGM0");

            var plot = LocalizationManager.GetPlot();

            var index = 0;
            foreach (var cg in InitialCGs)
            {
                if (index < 5)
                {
                    mInitialCG.sprite = cg;
                    yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 1f);
                    yield return new WaitForSeconds(0.5f);

                    foreach (var str in plot.InitialCGPlot[index])
                    {
                        mInitialCGTextA.text = str;
                        yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGTextACG, 1f);
                        yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));
                        AudioKit.PlaySound("Cube-Slide");
                    }
                    yield return CanvasGroupHelper.FadeCanvasGroup(new CanvasGroup[] {mInitialCGCanvasGroup, mInitialCGTextACG}, 0f);

                    yield return new WaitForSeconds(0.5f);
                }
                else if (index < plot.InitialCGPlot.Count)
                {
                    if (index == 9)
                    {
                        AudioMng.StopAmbient();
                        AudioMng.Instance.ChangeBGM("BGM1");
                    }

                    if (index == 12) yield return new WaitForSeconds(2f);

                    if (index == 15)
                    {
                        yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 0f);
                        yield return new WaitForSeconds(1f);

                        foreach (var str in plot.InitialMiddlePlot)
                        {
                            mInitialCGTextA.text = str;
                            yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGTextACG, 1f);
                            yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));
                            AudioKit.PlaySound("Cube-Slide");
                        }
                        yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGTextACG, 0f);
                        yield return new WaitForSeconds(2f);
                    }

                    mInitialCG.sprite = cg;
                    yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 1f);
                    yield return new WaitForSeconds(0.5f);

                    foreach (var str in plot.InitialCGPlot[index])
                    {
                        mInitialCGTextB.text = str;
                        yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGTextBCG, 1f);
                        yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));
                        AudioKit.PlaySound("Cube-Slide");
                    }
                    yield return CanvasGroupHelper.FadeCanvasGroup(new CanvasGroup[] {mInitialCGCanvasGroup, mInitialCGTextBCG}, 0f);

                    yield return new WaitForSeconds(0.5f);
                }

                index++;
            }
            
            AudioMng.StopAll();
        }

        private IEnumerator InitialPerformance()
        {
            mInitialVideoPlayer.clip = InitialClips[0];
            mInitialVideoPlayer.isLooping = false;
            mInitialVideoPlayer.Play();

# if UNITY_EDITOR
            yield return new WaitUntil(() => mInitialVideoPlayer.isPlaying == false || Input.GetKeyUp(KeyCode.Escape));
# else
            yield return new WaitUntil(() => mInitialVideoPlayer.isPlaying == false);
# endif

            mInitialVideoPlayer.clip = InitialClips[1];
            mInitialVideoPlayer.isLooping = true;

            mInitialVideoPlayer.Play();

            yield return new WaitUntil(() => Input.GetMouseButtonUp(1));

            mInitialVideoPlayer.Pause();
            mInitialVideoPlayer.clip = InitialClips[2];
            mInitialVideoPlayer.isLooping = false;
            mInitialVideoPlayer.Play();

# if UNITY_EDITOR
            yield return new WaitUntil(() => mInitialVideoPlayer.isPlaying == false || Input.GetKeyUp(KeyCode.Escape));
# else
            yield return new WaitUntil(() => mInitialVideoPlayer.isPlaying == false);
# endif

            mInitialVideoPlayer.clip = InitialClips[3];
            mInitialVideoPlayer.isLooping = false;

            mInitialVideoPlayer.Play();

# if UNITY_EDITOR
            yield return new WaitUntil(() => mInitialVideoPlayer.isPlaying == false || Input.GetKeyUp(KeyCode.Escape));
# else
            yield return new WaitUntil(() => mInitialVideoPlayer.isPlaying == false);
# endif

        }

        private void NormalEnterGame()
        {

# if UNITY_EDITOR
            TranslatorCanvasManager.StartMainMenu();
            TranslatorSM.CanActivate = true;
# else
            var resLoader = ResLoader.Allocate();
            mSecondTimeVideoPlayer.clip = LocalizationManager.Instance.CurrentLanguage switch
            {
                Localization.Language.zh_CN => resLoader.LoadSync<VideoClip>("videos", "coding effect_short_cn"),
                _ => resLoader.LoadSync<VideoClip>("videos", "coding effect_short_en"),
            };
            
            ActionKit.Sequence()
                .Delay(1f)
                .Callback(() => mSecondSplashCanvasGroup.gameObject.SetActive(true))
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mSecondSplashCanvasGroup, 1f)))
                .Delay(4f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mSecondSplashCanvasGroup, 0f)))
                .Delay(1f)
                .Callback(() => TranslatorCanvasManager.StartMainMenu())
                // .Callback(() => SceneControl.SceneControl.SwitchSceneWithoutConfirm("TestScene"))
                .Delay(1f)
                .Callback(() => TranslatorSM.CanActivate = true)
                .Start(this);
# endif

        }

        private void InitialSettings()
        {
            Application.targetFrameRate = 240;
            PlayerPrefs.DeleteAll();

            // Version Validating
            if (UserConfig.TryRead<string>("Version", out var local))
            {
                var localVersion = local.Split(".");
                if (int.Parse(localVersion[0]) < int.Parse(Version.Split(".")[0])
                    || (int.Parse(localVersion[0]) == int.Parse(Version.Split(".")[0]) && int.Parse(localVersion[1]) < int.Parse(Version.Split(".")[1])))
                {
                    GameProgressData.Clean();
                }
            }
            UserConfig.Write<string>("Version", Version);

            AudioKit.Settings.MusicVolume.SetValueWithoutEvent(UserConfig.ReadWithDefaultValue("Background Volume", 0.8f));
            AudioKit.Settings.SoundVolume.SetValueWithoutEvent(UserConfig.ReadWithDefaultValue("Effect Volume", 0.8f));

            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            var width = Screen.resolutions[^1].width;
            Screen.SetResolution(width, width / 16 * 9, true);
            if (UserConfig.TryRead<int>("Window Mode", out var mode))
            {
                if (mode == 1) return; // Full Screen
                else if (mode == 0 && UserConfig.TryRead<int>("Resolution", out var res))
                {
                    Screen.SetResolution(SettingsSM.AvailableResolutions[res].Item1,
                        SettingsSM.AvailableResolutions[res].Item2, false);
                }
                else Screen.SetResolution(1920, 1080, false);
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
        }
    }
}
