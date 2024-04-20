using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Hint.Hint3
{
    public class Hint3 : HintBase , ISingleton
    {
        public static Hint3 Instance => SingletonProperty<Hint3>.Instance;
        public void OnSingletonInit() {}

        private Button backButton;

        public override void OnEnter()
        {
            base.OnEnter();

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() => {
                HintManager.Exit();
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