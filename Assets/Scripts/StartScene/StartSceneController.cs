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
        public List<string> InitialMiddlePlot = new()
        {
            "事实上，饲养小白并不是一件困难的事情。",
            "或者说，比想象的要简单太多。",
            "为了省钱网购了猫砂盆和猫砂，在送货的时候用沙土和纸箱凑活了一下。在看到上面什么痕迹也没有的时候本来很担心，但似乎在屋子里也没有发现痕迹。小白似乎能自己到屋外解决问题，也没有把自己弄脏。最后把网购订单退货了，省下了一大笔钱。",
            "说到这个，小白确实是很干净的猫。别说尘土了，掉毛也很少，少到好像没掉过一样。",
            "吃饭也是。虽然小白好像不怎么喜欢在有人的时候吃饭——说实话一开始是担心过的——但是一会儿不看就吃得干干净净了。没有撒到地上，也没有剩饭，洗起来感觉碗干净得像被狗舔过一样…开玩笑的。",
            "而且小白很安静，小白不怎么喜欢跑动，也几乎没有听到过小白的叫声。",
            "当然，这不一定是好事，或许是身体和声带有问题也说不定。本来想带去医院检查，顺便看看要不要绝育，但是小白状况一直很稳定，能吃能喝能睡，从体型来说也像是没成年。所以决定等到有发情征兆了再说。这样看起来有点不负责任，但是能拖还是拖一下…就像拔智齿，虽然应该尽快处理，但也是能拖就拖一下吧…",
            "总归来说，小白是一只非常聪明又省心的猫。",
            "有时候会觉得，如果起名叫小黑的话，是不是某天就能变成人型了。",
            "但是还是希望小白能叫小白。",
            "小白现在听到这么叫会有回应。这也算是承认了这个名字吧。既然本猫都没有异议，那这就无可置疑了。",
            "除了名字，小白似乎也能理解一些其他的词，说了几次之后好像就记住了。不仅是声音，有时候看小白盯着摊开的书和电脑屏幕看，几乎要怀疑是不是能看懂文字……应该没有聪明到这个地步吧？",
            "当然，如果真的能理解的话肯定是好事。猫如果能理解人的语言和文字，应该就能理解人，然后反过来再让人理解猫在想什么。",
            "不过好像也不一定，猫有肢体语言和声音，人尚且还很难理解猫，就算加上文字也说不好。猫本来就是很难理解的生物，妄图理解猫多少有点不现实。",
            "更何况还是不爱叫的猫。",
            "扯远了。",
            "总之，小白没有给我增加太多负担。屋子里有个活着的生物感觉也不错。",
            "对于把小白带回家这件事，后悔是从未有过的。小白也没有要离开的意思。",
            "虽然猫的寿命不够长，但是至少在小白还在这个世界的时候，日子应该能就这样过下去吧。"
        };
        public List<VideoClip> InitialClips;

        private void Awake()
        {
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

            mPostProcessVolume = transform.Find("Post Processing").GetComponent<PostProcessVolume>();

            InitialSettings();

            TranslatorSM.CanActivate = false;

            if (PlayerPrefs.HasKey("Played") && PlayerPrefs.GetInt("Played") == 1)
            {
                PlayerPrefs.SetInt("FirstTime", 0);
                NormalEnterGame();
            }
            else
            {
                PlayerPrefs.SetInt("FirstTime", 1);
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
                        yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));
                        AudioKit.PlaySound("Cube-Slide");
                    }
                    yield return CanvasGroupHelper.FadeCanvasGroup(new CanvasGroup[] {mInitialCGCanvasGroup, mInitialCGTextACG}, 0f);

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
                        yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));
                        AudioKit.PlaySound("Cube-Slide");
                    }
                    yield return CanvasGroupHelper.FadeCanvasGroup(new CanvasGroup[] {mInitialCGCanvasGroup, mInitialCGTextBCG}, 0f);

                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    if (index == InitialPlot.Count)
                    {
                        yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 0f);
                        yield return new WaitForSeconds(1f);

                        foreach (var str in InitialMiddlePlot)
                        {
                            mInitialCGTextA.text = str;
                            yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGTextACG, 1f);
                            yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));
                            AudioKit.PlaySound("Cube-Slide");
                        }
                        yield return CanvasGroupHelper.FadeCanvasGroup(mInitialCGTextACG, 0f);

                        yield return new WaitForSeconds(2f);
                    }

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
