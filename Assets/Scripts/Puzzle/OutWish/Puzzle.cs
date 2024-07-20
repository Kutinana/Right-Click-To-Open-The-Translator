using DataSystem;
using Kuchinashi;
using Puzzle.Tutorial.P2;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.OutWish
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;

        private Button Yes;
        private Button No;

        private Button backButton;
        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            Instance = this;

            Yes = transform.Find("Interactable/Yes").GetComponent<Button>();
            No = transform.Find("Interactable/No").GetComponent<Button>();

            Yes.onClick.AddListener(() => YesEvent());
            No.onClick.AddListener(() => PuzzleManager.Exit());
        }

        private void YesEvent()
        {
            if (GameProgressData.GetInventory().TryGetValue("bomb", out var value) && value >= 1) this.enabled = false;
            else StartCoroutine(Fail());
        }

        private IEnumerator Fail()
        {
            transform.Find("Interactable/Fail").gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            transform.Find("Interactable/Fail").gameObject.SetActive(false);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {

            }
            else
            {
                CurrentCoroutine = StartCoroutine(CheckAnswerCoroutine());
            }

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
            yield return new WaitUntil(() => !enabled);

            PuzzleManager.Solved();
            CurrentCoroutine = null;
        }
    }
}