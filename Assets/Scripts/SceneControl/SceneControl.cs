using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using Kuchinashi;
using TMPro;
using UnityEngine.Events;

namespace SceneControl
{
    public class SceneControl : MonoSingleton<SceneControl>
    {
        public static string CurrentScene;
        public static bool IsLoading;
        public static bool CanTransition;

        private TMP_Text mTips;
        // private ParticleSystem mLoadingParticle;
        private CanvasGroup mLoadingPanelCG;
        private CanvasGroup mBackgroundImageCG;
        private CanvasGroup mBlackLayerCG;

        private static Coroutine mCurrentCoroutine;
        private static UnityAction mOnSceneLoaded;

        private AsyncOperation mAsyncOperation;

        void Awake()
        {            
            mTips = transform.Find("Tips").GetComponent<TMP_Text>();
            // mLoadingParticle = transform.Find("Ring").GetComponent<ParticleSystem>();

            mLoadingPanelCG = GetComponent<CanvasGroup>();
            mLoadingPanelCG.alpha = 0;
            mBackgroundImageCG = transform.Find("Background").GetComponent<CanvasGroup>();
            mBackgroundImageCG.alpha = 0;
            mBlackLayerCG = transform.Find("Layer").GetComponent<CanvasGroup>();
            mBlackLayerCG.alpha = 1;

            StartCoroutine(InitializeSceneControl());
        }

        private IEnumerator InitializeSceneControl()
        {
            mAsyncOperation = SceneManager.LoadSceneAsync("StartScene", LoadSceneMode.Additive);
            yield return mAsyncOperation;
            // SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("ControlScene"));
        }

        public static void SetActive(string targetSceneName)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetSceneName));
        }

        public static void SwitchScene(string targetSceneName)
        {
            if (IsLoading || mCurrentCoroutine != null) return;
            IsLoading = true;

            CanTransition = false;
            mCurrentCoroutine = Instance.StartCoroutine(Instance.SwitchSceneEnumerator(targetSceneName));
        }

        public static void SwitchSceneWithEvent(string targetSceneName, Action action)
        {
            if (IsLoading || mCurrentCoroutine != null) return;
            IsLoading = true;

            CanTransition = false;
            mCurrentCoroutine = Instance.StartCoroutine(Instance.SwitchSceneWithEventEnumerator(targetSceneName, action));
        }

        public static void SwitchSceneWithoutConfirm(string targetSceneName)
        {
            if (IsLoading || mCurrentCoroutine != null) return;
            IsLoading = true;

            mCurrentCoroutine = Instance.StartCoroutine(Instance.SwitchSceneWithoutConfirmEnumerator(targetSceneName));
        }

        public static void LoadScene(string targetSceneName)
        {
            if (IsLoading || mCurrentCoroutine != null) return;
            IsLoading = true;
            
            CanTransition = false;
            mCurrentCoroutine = Instance.StartCoroutine(Instance.LoadSceneEnumerator(targetSceneName));
        }

        public static void LoadSceneWithoutConfirm(string targetSceneName)
        {
            if (IsLoading || mCurrentCoroutine != null) return;
            IsLoading = true;

            CanTransition = true;
            mCurrentCoroutine = Instance.StartCoroutine(Instance.LoadSceneEnumerator(targetSceneName));
        }

        public static void UnloadCurrentScene()
        {
            if (IsLoading || mCurrentCoroutine != null) return;
            IsLoading = true;
            
            mCurrentCoroutine = Instance.StartCoroutine(Instance.UnloadCurrentSceneEnumerator());
        }

        IEnumerator UnloadCurrentSceneEnumerator()
        {
            yield return Fade(1);

            yield return SceneManager.UnloadSceneAsync(CurrentScene);

            yield return Fade(0);
            mCurrentCoroutine = null;
        }

        IEnumerator LoadSceneEnumerator(string sceneName)
        {
            yield return Fade(1);

            mAsyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return mAsyncOperation;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            yield return new WaitUntil(() => {return CanTransition;});
            yield return new WaitForSeconds(0.5f);

            yield return Fade(0);
            mCurrentCoroutine = null;
        }

        IEnumerator SwitchSceneEnumerator(string sceneName)
        {
            yield return Fade(1);

            mAsyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            CurrentScene = sceneName;
            
            mAsyncOperation.allowSceneActivation = true;

            yield return mAsyncOperation;
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            TypeEventSystem.Global.Send<OnSceneLoadedEvent>();

            yield return new WaitUntil(() => {return CanTransition;});
            yield return Fade(0);
            
            mCurrentCoroutine = null;
            mAsyncOperation = null;
        }

        IEnumerator SwitchSceneWithEventEnumerator(string sceneName, Action action)
        {
            yield return Fade(1);
            
            mAsyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            CurrentScene = sceneName;
            
            mAsyncOperation.allowSceneActivation = true;

            yield return mAsyncOperation;
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            TypeEventSystem.Global.Send<OnSceneLoadedEvent>();

            action();
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => {return CanTransition;});

            yield return Fade(0);

            mCurrentCoroutine = null;
            mAsyncOperation = null;
        }

        IEnumerator SwitchSceneWithoutConfirmEnumerator(string sceneName)
        {
            yield return Fade(1);

            mAsyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            CurrentScene = sceneName;
            
            mAsyncOperation.allowSceneActivation = true;

            yield return mAsyncOperation;
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            TypeEventSystem.Global.Send<OnSceneLoadedEvent>();

            yield return Fade(0);
            mCurrentCoroutine = null;
            mAsyncOperation = null;
        }
        
        private IEnumerator Fade(float targetAlpha)
        {
            IsLoading = targetAlpha == 1;

            if (targetAlpha == 1)
            {
                yield return CanvasGroupHelper.FadeCanvasGroup(mLoadingPanelCG, 1, 0.02f);
                mBackgroundImageCG.alpha = 1;
                yield return CanvasGroupHelper.FadeCanvasGroup(mBlackLayerCG, 0);

                // mLoadingParticle.gameObject.SetActive(true);
                mTips.CrossFadeAlpha(1, 0.5f, true);

                yield return new WaitForSeconds(0.5f);
            }
            else if (targetAlpha == 0)
            {
                // mLoadingParticle.gameObject.SetActive(false);
                mTips.gameObject.SetActive(false);

                yield return CanvasGroupHelper.FadeCanvasGroup(mBlackLayerCG, 1, 0.02f);
                mBackgroundImageCG.alpha = 0;
                yield return CanvasGroupHelper.FadeCanvasGroup(mLoadingPanelCG, 0);
            }
        }
    }
}
