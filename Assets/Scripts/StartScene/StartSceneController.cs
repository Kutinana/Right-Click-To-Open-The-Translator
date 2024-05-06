using System.Collections;
using System.Collections.Generic;
using Cameras;
using Kuchinashi;
using QFramework;
using Translator;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Video;
using DataSystem;
using Settings;
using UI;
using TMPro;
using System;

namespace StartScene
{
    public class StartSceneController : MonoSingleton<StartSceneController>
    {
        public static string Version => "0.1.1";

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

        private PostProcessVolume mPostProcessVolume;

        public List<Sprite> InitialCGs;
        public List<List<string>> InitialPlot = new()
        {
            new() {"又是下雨天...好烦...", "湿漉漉的", "总算快到家了"},
            new() {"？", "怎么回事"},
            new() {"错觉吗...？", "果然是这几天太累了"},
            new() {"…呃嗯…？"},
            new() {"有点在意......"},
            new() {"原来是猫啊...", "好可怜", "刚刚那个，应该是哪里的灯吧。"},
            new() {"哎，居然还活着吗", "不过这种天气...也活不过今晚吧", "我...唉......"},
            new() {"这个眼神...", "只是跟我来这套也没用啊......", "“很抱歉。”"},
            new() {"（盯）", "“就算你这么看着我也——”"},
            new() {"带回家了。", "算了，一个晚上而已。", "“只有今晚哦。”"},
            new() {"…怎么又来"},
            new() {"“就算你这么看着我也——”"},
            new() {"最终还是......留下了。", "算了，多双筷子。", "甚至也不需要筷子。加个碗就好了。", "今后就多多指教了。", "“喂。你怎么又上窗台。”"},
            new() {"黑毛，黑眼睛。", "喜欢扒着窗户看外面。也不知道有什么好看的。", "和我一点都不像。", "我养了你，你总该有哪里像我吧。"},
            new() {"“小白。吃饭了。”"}
        };
        public List<VideoClip> InitialClips;

        private void Awake()
        {
            mFirstSplashCanvasGroup = transform.Find("FirstSplash").GetComponent<CanvasGroup>();
            mFirstSplashCanvasGroup.alpha = 0;
            mSecondSplashCanvasGroup = transform.Find("SecondSplash").GetComponent<CanvasGroup>();
            mSecondSplashCanvasGroup.alpha = 0;

            mInitialCGCanvasGroup = transform.Find("FirstTimeCG").GetComponent<CanvasGroup>();
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

            mPostProcessVolume = transform.Find("Post Processing").GetComponent<PostProcessVolume>();

            InitialSettings();

            TranslatorSM.CanActivate = false;

            if (PlayerPrefs.HasKey("Played") && PlayerPrefs.GetInt("Played") == 1)
            {
                NormalEnterGame();
            }
            else
            {
                FirstTimeEnterGame();
            }
        }

        private void FirstTimeEnterGame()
        {
            UserDictionary.WriteInAndSave("la", "来");
            UserDictionary.WriteInAndSave("schl", "按键");
            UserDictionary.WriteInAndSave("rec", "右");
            UserDictionary.WriteInAndSave("offen", "启动");
            UserDictionary.WriteInAndSave("masc", "机器");
            UserDictionary.WriteInAndSave("ubrs", "翻译");
            UserDictionary.WriteInAndSave("geb", "游戏");

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
                .Delay(1f)
                .Start(this);

            PlayerPrefs.SetInt("Played", 1);
        }

        private IEnumerator InitialCG()
        {

            yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 0f);
            AudioMng.Instance.PlayAmbient();
            AudioMng.Instance.D_PlayBGM("BGM0");
            var index = 0;
            foreach (var cg in InitialCGs)
            {
                if (index < 5)
                {
                    mInitialCG.sprite = cg;
                    yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 1f);
                    yield return new WaitForSeconds(0.5f);

                    foreach (var str in InitialPlot[index])
                    {
                        mInitialCGTextA.text = str;
                        yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGTextACG, 1f);
                        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
                        AudioKit.PlaySound("Cube-Slide");
                    }
                    yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 0f);
                    mInitialCGTextACG.alpha = 0;

                    yield return new WaitForSeconds(0.5f);
                }
                else if (index < InitialPlot.Count)
                {
                    if (index == 9){
                        AudioMng.StopAmbient();
                        AudioMng.Instance.ChangeBGM("BGM1");
                    }
                        if (index == 12) yield return new WaitForSeconds(2f);


                    mInitialCG.sprite = cg;
                    yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 1f);
                    yield return new WaitForSeconds(0.5f);

                    foreach (var str in InitialPlot[index])
                    {
                        mInitialCGTextB.text = str;
                        yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGTextBCG, 1f);
                        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
                        AudioKit.PlaySound("Cube-Slide");
                    }
                    yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 0f);
                    mInitialCGTextBCG.alpha = 0;

                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    if (index == InitialPlot.Count) yield return new WaitForSeconds(2f);

                    mInitialCGFront.sprite = cg;
                    yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGFrontCanvasGroup, 1f);

                    mInitialCGCanvasGroup.alpha = 1;
                    mInitialCG.sprite = cg;
                    mInitialCGFrontCanvasGroup.alpha = 0f;

                    yield return new WaitForSeconds(1f);
                }

                index++;
                if(index==15) AudioMng.StopAll();
            }
        }

        private IEnumerator InitialPerformance()
        {
            mInitialVideoPlayer.clip = InitialClips[0];
            mInitialVideoPlayer.isLooping = false;
            mInitialVideoPlayer.Play();

            yield return new WaitUntil(() => mInitialVideoPlayer.isPlaying == false || Input.GetKeyUp(KeyCode.Escape));
            mInitialVideoPlayer.clip = InitialClips[1];
            mInitialVideoPlayer.isLooping = true;

            mInitialVideoPlayer.Play();

            yield return new WaitUntil(() => Input.GetMouseButtonUp(1));

            mInitialVideoPlayer.Pause();
            mInitialVideoPlayer.clip = InitialClips[2];
            mInitialVideoPlayer.isLooping = false;
            mInitialVideoPlayer.Play();

            while (!Mathf.Approximately(mPostProcessVolume.weight, 1f))
            {
                mPostProcessVolume.weight = Mathf.MoveTowards(mPostProcessVolume.weight, 1f, Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
            mPostProcessVolume.weight = 1f;

            yield return new WaitUntil(() => mInitialVideoPlayer.isPlaying == false);
            mInitialVideoPlayer.clip = InitialClips[3];
            mInitialVideoPlayer.isLooping = false;
            mPostProcessVolume.weight = 0;

            mInitialVideoPlayer.Play();

            yield return new WaitUntil(() => mInitialVideoPlayer.isPlaying == false);
        }

        private void NormalEnterGame()
        {
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
        }

        private void InitialSettings()
        {
            Application.targetFrameRate = 240;

            // Version Validating
            if (PlayerPrefs.HasKey("Version"))
            {
                var localVersion = PlayerPrefs.GetString("Version").Split(".");
                if (Int32.Parse(localVersion[0]) < Int32.Parse(Version.Split(".")[0])
                    || Int32.Parse(localVersion[0]) == Int32.Parse(Version.Split(".")[0]) && Int32.Parse(localVersion[1]) < Int32.Parse(Version.Split(".")[1])
                    || Int32.Parse(localVersion[0]) == Int32.Parse(Version.Split(".")[0]) && Int32.Parse(localVersion[1]) == Int32.Parse(Version.Split(".")[1]) && Int32.Parse(localVersion[2]) < Int32.Parse(Version.Split(".")[2]))
                {
                    GameProgressData.Clean();
                }
            }

            if (!PlayerPrefs.HasKey("Version") || PlayerPrefs.GetString("Version") != Version)
            {
                PlayerPrefs.DeleteAll();

                PlayerPrefs.SetString("Version", Version);
            }

            AudioKit.Settings.MusicVolume.SetValueWithoutEvent(
                PlayerPrefs.HasKey("BackgroundVolume") ? PlayerPrefs.GetFloat("BackgroundVolume") : 0.8f);
            AudioKit.Settings.SoundVolume.SetValueWithoutEvent(
                PlayerPrefs.HasKey("EffectVolume") ? PlayerPrefs.GetFloat("EffectVolume") : 0.8f);

            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            if (PlayerPrefs.HasKey("Window Mode"))
            {
                if (PlayerPrefs.GetInt("Window Mode") == 1)
                {
                    var width = Screen.resolutions[^1].width;
                    Screen.SetResolution(width, width / 16 * 9, true);
                }
                else if (PlayerPrefs.HasKey("Resolution"))
                {
                    Screen.SetResolution(SettingsSM.AvailableResolutions[PlayerPrefs.GetInt("Resolution")].Item1,
                        SettingsSM.AvailableResolutions[PlayerPrefs.GetInt("Resolution")].Item2, false);
                }
            }
            else
            {
                var width = Screen.resolutions[^1].width;
                Screen.SetResolution(width, width / 16 * 9, true);
            }
        }
    }
}
