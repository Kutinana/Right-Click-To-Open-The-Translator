using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using QFramework;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.WeighBeaker
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;

        public Bottle HoldingBottle = null;

        private Button backButton;
        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            Instance = this;
        }

        public override void OnEnter()
        {
            // if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            // {
            //     transform.Find("Background/Answer").gameObject.SetActive(true);
            //     transform.Find("Interactable").gameObject.SetActive(false);
            // }
            // else
            // {
            //     CurrentCoroutine = StartCoroutine(CheckAnswerCoroutine());
            // }

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() => {
                PuzzleManager.Exit();
            });

            List<string> ids = new List<string>();
            foreach (var c in GetComponentsInChildren<Character>())
            {
                ids.Add(c.data.Id);
            }
            UserDictionary.Unlock(ids);
            
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

        // private IEnumerator CheckAnswerCoroutine()
        // {
        //     yield return new WaitUntil(() => {
        //         return correct == 6;
        //     });

        //     PuzzleManager.Solved();
        //     CurrentCoroutine = null;
        // }
    }
}