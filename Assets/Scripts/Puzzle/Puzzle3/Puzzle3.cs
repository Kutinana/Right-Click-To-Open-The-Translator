using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using QFramework;
using Translator;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Puzzle3
{
    public class Puzzle3 : PuzzleBase , ISingleton
    {
        public static Puzzle3 Instance => SingletonProperty<Puzzle3>.Instance;
        public void OnSingletonInit() {}

        public static bool IsHoldingDoor = false;

        private Button backButton;
        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
        }

        public override void OnEnter()
        {
            CurrentCoroutine = StartCoroutine(CheckAnswerCoroutine());

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() => {
                TypeEventSystem.Global.Send<OnPuzzleExitEvent>();
            });

            UserDictionary.Unlock("feu");
        }

        public override void OnExit()
        {
            base.OnExit();

            if (CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);
                CurrentCoroutine = null;
            }
        }

        public override void OnComplete()
        {
            base.OnComplete();

            Debug.Log("Complete");
        }

        private IEnumerator CheckAnswerCoroutine()
        {
            yield return new WaitUntil(() => {
                return Input.GetKeyUp(KeyCode.Space);
            });

            TypeEventSystem.Global.Send<OnPuzzleSolvedEvent>();
            CurrentCoroutine = null;
        }
    }
}