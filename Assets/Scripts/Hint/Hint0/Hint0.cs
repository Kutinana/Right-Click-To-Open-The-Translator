using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Hint.Hint0
{
    public class Hint0 : HintBase
    {
        public static Hint0 Instance;

        private Button backButton;

        private void Awake()
        {
            Instance = this;
        }

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