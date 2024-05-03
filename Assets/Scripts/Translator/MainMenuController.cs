using System.Collections;
using System.Collections.Generic;
using DataSystem;
using Kuchinashi;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Translator
{
    public class MainMenuController : MonoBehaviour
    {
        private CanvasGroup mCanvasGroup;

        private Button mStartGameBtn;
        private Button mQuitGameBtn;
        private Button mSettingsBtn;
        private Button mCreditBtn;

        private CanvasGroup mCreditPanel;
        private Button mCreditPanelBtn;
        private CanvasGroup mQuitPanel;
        private CanvasGroup mBlank;

        private void Awake()
        {
            mCanvasGroup = GetComponent<CanvasGroup>();

            mStartGameBtn = transform.Find("Menu/Start").GetComponent<Button>();
            mQuitGameBtn = transform.Find("Menu/Quit").GetComponent<Button>();
            mSettingsBtn = transform.Find("Menu/Settings").GetComponent<Button>();
            mCreditBtn = transform.Find("Menu/Credit").GetComponent<Button>();

            mCreditPanel = transform.Find("Credit").GetComponent<CanvasGroup>();
            mCreditPanel.alpha = 0;
            mCreditPanelBtn = mCreditPanel.GetComponent<Button>();
            mQuitPanel = transform.Find("QuitPanel").GetComponent<CanvasGroup>();
            mQuitPanel.alpha = 0;
            mBlank = transform.Find("Blank").GetComponent<CanvasGroup>();
            mBlank.alpha = 0;

            mStartGameBtn.onClick.AddListener(() => {
                AudioMng.PlayBtnPressed(1);
                StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mCanvasGroup, 0f));
            });
            mQuitGameBtn.onClick.AddListener(() => {
                AudioMng.PlayBtnPressed(2);
                StartCoroutine(QuitGameCoroutine());
            });
            mSettingsBtn.onClick.AddListener(() => {
                AudioMng.PlayBtnPressed(0);
                TranslatorSM.StateMachine.ChangeState(States.Settings);
            });
            mCreditBtn.onClick.AddListener(() => {
                AudioMng.PlayBtnPressed(0);
                StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mCreditPanel, 1f, 0.2f));
            });
            mCreditPanelBtn.onClick.AddListener(() => {
                StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mCreditPanel, 0f, 0.2f));
            });

            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => {
                ToggleInteractability(false);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => {
                ToggleInteractability(true);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            ToggleInteractability(TranslatorSM.StateMachine.CurrentStateId == States.Off);
            List<string> ids = new List<string>();
            foreach (var c in GetComponentsInChildren<Character>(includeInactive: true))
            {
                ids.Add(c.data.Id);
            }
            UserDictionary.Unlock(ids);
        }

        private void ToggleInteractability(bool _flag)
        {
            mStartGameBtn.interactable = _flag;
            mStartGameBtn.transform.Find("Characters").gameObject.SetActive(!_flag);

            mQuitGameBtn.interactable = _flag;
            mQuitGameBtn.transform.Find("Characters").gameObject.SetActive(!_flag);

            mSettingsBtn.interactable = _flag;
            mSettingsBtn.transform.Find("Characters").gameObject.SetActive(!_flag);

            mCreditBtn.interactable = _flag;
            mCreditBtn.transform.Find("Characters").gameObject.SetActive(!_flag);
        }

        IEnumerator QuitGameCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            AudioMng.StopAll();
            yield return CanvasGroupHelper.FadeCanvasGroup(mQuitPanel, 1f, 0.01f);

            yield return new WaitForSeconds(1f);

            yield return CanvasGroupHelper.FadeCanvasGroup(mBlank, 1f, 0.01f);

            Application.Quit();
        }
    }
}
