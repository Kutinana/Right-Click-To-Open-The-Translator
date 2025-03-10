using DataSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InCenter.Painting
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;
        public bool solved = false;

        public List<Transform> ValidPoints;

        public List<Tag> CORRECT;

        public static List<Tag> TagsInBlock = new List<Tag>()
        {
            null, null, null, null, null, null
        };

        public const float ERROR = 0.5f;

        public Tag HoldingTag = null;

        public List<CanvasGroup> Characters;

        private Button backButton;
        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            Instance = this;

            DisableCharacters();

            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => EnableCharacters()).UnRegisterWhenGameObjectDestroyed(gameObject);
            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => DisableCharacters()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        private void EnableCharacters()
        {
            foreach (var item in Characters)
            {
                item.interactable = true;
                item.blocksRaycasts = true;
            }
        }

        private void DisableCharacters()
        {
            foreach (var item in Characters)
            {
                item.interactable = false;
                item.blocksRaycasts = false;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {
                solved = true;
            }
            else
            {
                if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.NotFound) {}

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
            UserDictionary.AddRelatedPuzzleAndSave(ids, Id);
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
            yield return new WaitUntil(() => CheckAnswer());

            //transform.Find("Interactable/Mask").gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            GameProgressData.IncreaseInventory("greenLiquid");
            PuzzleManager.Solved();
            CurrentCoroutine = null;
        }

        private bool CheckAnswer()
        {
            for (int i = 0; i < 6; i++)
            {
                if (TagsInBlock[i] != CORRECT[i])
                    return false;
            }
            return true;
        }
    }
}