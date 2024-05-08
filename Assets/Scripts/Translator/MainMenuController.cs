using System.Collections;
using System.Collections.Generic;
using DataSystem;
using Kuchinashi;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            mBlank = transform.Find("Blank").GetComponent<CanvasGroup>();
            mBlank.alpha = 0;

            mStartGameBtn.onClick.AddListener(() => {
                AudioMng.PlayBtnPressed(1);
                StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mCanvasGroup, 0f));

                switch (GameProgressData.GetLastScene())
                {
                    case "Zero":
                        SceneControl.SceneControl.SwitchSceneWithoutConfirm("Zero", delay: 1f);
                        break;
                    case "First":
                        SceneControl.SceneControl.SwitchSceneWithoutConfirm("First", delay: 1f);
                        break;
                    default:
                        SceneControl.SceneControl.SwitchSceneWithoutConfirm("Zero", delay: 1f);
                        break;
                }
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
            var start = mStartGameBtn.transform.Find("Characters");
            start.gameObject.SetActive(!_flag);

            mQuitGameBtn.interactable = _flag;
            var quit = mQuitGameBtn.transform.Find("Characters");
            quit.gameObject.SetActive(!_flag);

            mSettingsBtn.interactable = _flag;
            var settings = mSettingsBtn.transform.Find("Characters");
            settings.gameObject.SetActive(!_flag);

            mCreditBtn.interactable = _flag;
            var credit = mCreditBtn.transform.Find("Characters");
            credit.gameObject.SetActive(!_flag);

            mStartGameBtn.transform.parent.GetComponentsInChildren<Character>().ForEach(x => x.Refresh());
        }

        IEnumerator QuitGameCoroutine()
        {
            yield return new WaitForSeconds(0.5f);

            yield return CanvasGroupHelper.FadeCanvasGroup(mBlank, 1f, 0.02f);
            AudioMng.StopAll();
            
            QuitPanelController.StartQuitting();
        }
    }
}
