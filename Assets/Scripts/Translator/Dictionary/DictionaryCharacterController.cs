using System.Collections;
using System.Collections.Generic;
using Dictionary;
using QFramework;
using UnityEngine;

namespace UI
{
    public class DictionaryCharacterController : Character
    {
        protected override void Awake()
        {
            base.Awake();

            _buttonExtension.OnLeftClick += () =>
            {
                TypeEventSystem.Global.Send(new OnCharacterDataUpdateEvent(data));
            };

            TypeEventSystem.Global.Register<OnCharacterRefreshEvent>(e => Refresh()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
    }
}