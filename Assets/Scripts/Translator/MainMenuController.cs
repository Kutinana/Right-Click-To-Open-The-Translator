using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Translator
{
    public class MainMenuController : MonoBehaviour
    {
        private Button mStartGameBtn;
        private Button mQuitGameBtn;
        private Button mSettingsBtn;
        private Button mCreditBtn;

        private void Awake()
        {
            mStartGameBtn = transform.Find("Menu/Start").GetComponent<Button>();
            mQuitGameBtn = transform.Find("Menu/Quit").GetComponent<Button>();
            mSettingsBtn = transform.Find("Menu/Settings").GetComponent<Button>();
            mCreditBtn = transform.Find("Menu/Credit").GetComponent<Button>();

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
            mQuitGameBtn.interactable = _flag;
            mSettingsBtn.interactable = _flag;
            mCreditBtn.interactable = _flag;
        }
    }
}
