using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Hint.Hint2
{
    public class Hint2 : HintBase , ISingleton
    {
        public static Hint2 Instance => SingletonProperty<Hint2>.Instance;
        public void OnSingletonInit() {}

        private Button backButton;

        public override void OnEnter()
        {
            base.OnEnter();

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() => {
                TypeEventSystem.Global.Send<OnHintExitEvent>();
            });
            
            List<string> ids = new List<string>();
            foreach (var c in GetComponentsInChildren<Character>())
            {
                ids.Add(c.data.Id);
            }
            UserDictionary.Unlock(ids);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}