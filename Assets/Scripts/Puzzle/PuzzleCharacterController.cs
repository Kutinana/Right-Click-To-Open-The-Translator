using System.Collections;
using System.Collections.Generic;
using TMPro;
using DataSystem;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Translator;
using UnityEngine.EventSystems;
using Kuchinashi;

namespace UI
{
    /// <summary>
    /// Character Controller used on puzzle and hints.
    /// </summary>
    public class PuzzleCharacterController : Character
    {
        Material material;
        protected override void Awake()
        {
            base.Awake();
            material = transform.Find("Image").GetComponent<Image>().material;
            // Register event listeners
            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => {
                stateMachine.ChangeState(States.Interactable);
                material.SetFloat("_EnableOutline",1);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => {
                stateMachine.ChangeState(States.NonInteractable);
                material.SetFloat("_EnableOutline",0);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnCharacterRecordedEvent>(e => {
                if (e.id == data.Id) Refresh();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnCharacterRefreshEvent>(e => Refresh()).UnRegisterWhenGameObjectDestroyed(gameObject);

            Initialize();
        }

        public override void Initialize(CharacterData data = null, bool isInteractable = false, bool isBlack = false)
        {
            base.Initialize(data, isInteractable, isBlack);

            _buttonExtension.OnLeftClick += () => {
                CharacterRecordPanelManager.Instance.Init(this);
            };
        }
    }
}