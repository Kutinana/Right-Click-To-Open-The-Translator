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

namespace StartScene
{
    public class StartSceneController : MonoSingleton<StartSceneController>
    {
        private CanvasGroup mSplashCanvasGroup;

        private CanvasGroup mInitialCGCanvasGroup;
        private Image mInitialCG;
        private CanvasGroup mInitialVideoCanvasGroup;
        private VideoPlayer mInitialVideoPlayer;

        private PostProcessVolume mPostProcessVolume;

        public List<VideoClip> InitialClips;

        private void Awake()
        {
            mSplashCanvasGroup = transform.Find("Splash").GetComponent<CanvasGroup>();
            mSplashCanvasGroup.alpha = 0;

            mInitialCGCanvasGroup = transform.Find("FirstTimeCG").GetComponent<CanvasGroup>();
            mInitialCGCanvasGroup.alpha = 0;
            mInitialCG = transform.Find("FirstTimeCG/Image").GetComponent<Image>();

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
            ActionKit.Sequence()
                .Delay(1f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mSplashCanvasGroup, 1f)))
                .Delay(2f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mSplashCanvasGroup, 0f)))
                .Delay(1f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 1f)))
                .Condition(() => Input.GetMouseButtonUp(0))
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mInitialCGCanvasGroup, 0f)))
                .Delay(1f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mInitialVideoCanvasGroup, 1f)))
                .Coroutine(InitialPerformance)
                .Callback(() => TranslatorCanvasManager.StartMainMenu())
                .Delay(1f)
                .Callback(() => SceneControl.SceneControl.SwitchSceneWithoutConfirm("TestScene"))
                .Callback(() => TranslatorSM.CanActivate = true)
                .Start(this);

            PlayerPrefs.SetInt("Played", 1);
        }

        private IEnumerator InitialPerformance()
        {
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
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mSplashCanvasGroup, 1f)))
                .Delay(2f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mSplashCanvasGroup, 0f)))
                .Delay(1f)
                .Callback(() => TranslatorCanvasManager.StartMainMenu())
                .Delay(1f)
                .Callback(() => SceneControl.SceneControl.SwitchSceneWithoutConfirm("TestScene"))
                .Callback(() => TranslatorSM.CanActivate = true)
                .Start(this);
        }

        private void InitialSettings()
        {
            Application.targetFrameRate = 240;

            AudioKit.Settings.MusicVolume.SetValueWithoutEvent(
                PlayerPrefs.HasKey("BackgroundVolume") ? PlayerPrefs.GetFloat("BackgroundVolume") : 0.8f);
            AudioKit.Settings.SoundVolume.SetValueWithoutEvent(
                PlayerPrefs.HasKey("EffectVolume") ? PlayerPrefs.GetFloat("EffectVolume") : 0.8f);
        }
    }
}
