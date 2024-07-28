using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using QFramework;
using Translator;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Overture.Jigsaw
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;

        public static Cube HoldingCube = null;
        public static Dictionary<Vector2, string> CORRECT = new Dictionary<Vector2, string>() {
            {new Vector2(2, 2), "feu"},
            {new Vector2(2, 4), "geh"},
            {new Vector2(2, 6), "vor"},
            {new Vector2(4, 2), "was"},
            {new Vector2(4, 4), "geh"},
            {new Vector2(4, 6), "rut"}
        };
        public static Dictionary<Vector2, bool> Record = new Dictionary<Vector2, bool>() {
            {new Vector2(2, 2), true},
            {new Vector2(2, 4), false},
            {new Vector2(2, 6), false},
            {new Vector2(4, 2), false},
            {new Vector2(4, 4), false},
            {new Vector2(4, 6), false}
        };
        private int correct => Record.Values.Where(x => x == true).Count();

        private Button backButton;
        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            Instance = this;
        }

        public override void OnEnter()
        {
            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {
                transform.Find("Background/Answer").gameObject.SetActive(true);
                transform.Find("Interactable").gameObject.SetActive(false);
            }
            else
            {
                CurrentCoroutine = StartCoroutine(CheckAnswerCoroutine());
            }

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() => {
                PuzzleManager.Exit();
            });

            UserDictionary.AddRelatedPuzzleAndSave("feu", Id);
            GameProgressData.Unlock(this);
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

        private IEnumerator CheckAnswerCoroutine()
        {
            yield return new WaitUntil(() => {
                return correct == 6;
            });

            PuzzleManager.Solved();
            CurrentCoroutine = null;
        }
    }
}