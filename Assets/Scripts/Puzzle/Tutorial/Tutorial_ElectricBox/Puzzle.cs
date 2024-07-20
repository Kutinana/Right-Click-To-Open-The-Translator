
using DataSystem;
using Kuchinashi;
using Puzzle.Overture.Bottle;
using Puzzle.Tutorial.P1;
using System.Collections;
using System.Collections.Generic;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

namespace Puzzle.Tutorial.P2
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;
        public bool solved = false;

        public static List<float> ValidPositions = new List<float>() { 0, 1.1f, 2.2f, 3.3f, 4.4f };

        public List<int> CurrentPosition = new List<int>() { 0, 0, 0 };
        public static readonly List<int> CORRECT = new List<int>() { 0, 4, 3 };

        public Slide HoldingSilde = null;
        public List<Slide> m_slides;
        public List<Image> Lights;
        public Sprite LightOn;
        public Sprite LightOff;

        private CanvasGroup tutorialCanvasGroup;

        private Button backButton;
        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            Instance = this;
            ReArrangePosition(false);
            
            tutorialCanvasGroup = transform.Find("Tutorial").GetComponent<CanvasGroup>();
        }

        public static void ReArrangePosition(bool var)
        {
            for (int i = 0; i < 3; i++)
            {
                Instance.m_slides[i].closestPos = Instance.CurrentPosition[i];
                Instance.m_slides[i].ArrangePosition(var);
            }
            
            if (Instance.CurrentPosition[0] == CORRECT[0] && Instance.CurrentPosition[1] == CORRECT[1] && Instance.CurrentPosition[2] == CORRECT[2])
                Instance.Lights[0].sprite = Instance.LightOn;
            else Instance.Lights[0].sprite = Instance.LightOff;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {
                CurrentPosition = CORRECT;
                ReArrangePosition(false);

                solved = true;
            }
            else
            {
                if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.NotFound)
                {
                    GameProgressData.Unlock(this);
                    StartCoroutine(ShowTutorialCoroutine());
                }
                else
                {
                    transform.Find("Tutorial").gameObject.SetActive(false);
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

        private IEnumerator ShowTutorialCoroutine()
        {
            foreach (var slide in m_slides)
            {
                slide.enabled = false;
            }
            yield return CanvasGroupHelper.FadeCanvasGroup(tutorialCanvasGroup, 1f);

            yield return new WaitForSeconds(1f);
            tutorialCanvasGroup.GetComponent<Button>().onClick.AddListener(() =>
            {
                TranslatorSM.StateMachine.ChangeState(Translator.States.Dictionary);
                StartCoroutine(HideTutorialCoroutine());
            });
        }

        private IEnumerator CheckAnswerCoroutine()
        {
            yield return new WaitUntil(() => {
                return CurrentPosition[0] == CORRECT[0] && CurrentPosition[1] == CORRECT[1] && CurrentPosition[2] == CORRECT[2];
            });

            PuzzleManager.Solved();
            CurrentCoroutine = null;
        }

        private IEnumerator HideTutorialCoroutine()
        {
            yield return CanvasGroupHelper.FadeCanvasGroup(tutorialCanvasGroup, 0f);

            foreach (var slide in m_slides)
            {
                slide.enabled = true;
            }
        }
    }
}