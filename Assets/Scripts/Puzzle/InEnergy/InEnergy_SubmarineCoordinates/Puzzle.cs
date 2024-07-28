using DataSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.Submarine
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;
        private CanvasGroup m_characterGroup;

        private void Awake()
        {
            Instance = this;

            m_characterGroup = transform.Find("Interactable/Characters").GetComponent<CanvasGroup>();
            DisableCharacters();

            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => EnableCharacters()).UnRegisterWhenGameObjectDestroyed(gameObject);
            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => DisableCharacters()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private Coroutine CurrentCoroutine = null;
        private Button backButton;

        public List<int> m_coordinates = new List<int> { 0, 0, 0, 0 };
        private readonly List<int> m_answer = new List<int> { 2, 0, 5, 4 };

        public Transform item;
        public List<CharacterData> numbers;

        private void EnableCharacters()
        {
            m_characterGroup.interactable = true;
            m_characterGroup.blocksRaycasts = true;
        }

        private void DisableCharacters()
        {
            m_characterGroup.interactable = false;
            m_characterGroup.blocksRaycasts = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {

            }
            else
            {
                if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.NotFound)
                {

                }

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

        public static bool PuzzleFinish() => Instance.m_coordinates[0] == Instance.m_answer[0]
            && Instance.m_coordinates[1] == Instance.m_answer[1]
            && Instance.m_coordinates[2] == Instance.m_answer[2]
            && Instance.m_coordinates[3] == Instance.m_answer[3];

        private IEnumerator CheckAnswerCoroutine()
        {
            yield return new WaitUntil(() => !enabled);

            PuzzleManager.Solved(isClosing: false);
            CurrentCoroutine = null;
        }
    }
}