using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using Kuchinashi;
using System;

namespace Puzzle.Tutorial.P1
{
    public enum patternType
    {
        Circle,
        Triangle,
        Square
    }

    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;

        private Button backButton;

        public static List<patternType> Patterns = new List<patternType>() { patternType.Circle, patternType.Circle, patternType.Circle};
        public static List<float> CorrectPosY = new List<float>() { -175f, -400f, -320f};
        public const float ERROR = 20f;
        public static List<float> Scopes = new List<float>() { 0, 0, 0};
        public List<Sprite> CTS;
        public static List<Transform> m_scope;
        private Coroutine CurrentCoroutine = null;
        public static bool enable = true;
        private CanvasGroup tutorialCanvasGroup;
        private static bool CORRECT => Patterns[0] == patternType.Circle && Patterns[1] == patternType.Triangle && Patterns[2] == patternType.Square;
        private static bool SCOPE_CORRECT => Mathf.Abs(Scopes[0] - CorrectPosY[0]) <= ERROR && Mathf.Abs(Scopes[1] - CorrectPosY[1]) <= ERROR && Mathf.Abs(Scopes[2] - CorrectPosY[2]) <= ERROR;

        public static bool holding_scope = false;

        public static System.Action S1Correct;

        public static event EventHandler<int> Show;
        public static event EventHandler<int> Hide;

        private void Awake()
        {
            Instance = this;

            tutorialCanvasGroup = transform.Find("Tutorial").GetComponent<CanvasGroup>();
        }

        public static void PatternUpdate(int pos, Transform button)
        {
            int val = ((int)Patterns[pos] + 1) % 3;
            Patterns[pos] = (patternType)val;
            switch (Patterns[pos])
            {
                case patternType.Square:
                    button.GetChild(0).GetComponent<Image>().sprite = Instance.CTS[2];
                    break;
                case patternType.Triangle:
                    button.GetChild(0).GetComponent<Image>().sprite = Instance.CTS[1];
                    break;
                case patternType.Circle:
                    button.GetChild(0).GetComponent<Image>().sprite = Instance.CTS[0];
                    break;
                default:
                    break;
            }
            if (CORRECT) S1Correct.Invoke();
        }

        public static void UpdateScopeState(int id, float posY)
        {
            Scopes[id] = posY;
            if (Mathf.Abs(Scopes[id] - CorrectPosY[id]) <= ERROR)
            {
                Show(Instance, id);
            }
            else
            {
                Hide(Instance, id);
            }
            if (SCOPE_CORRECT && enable) 
            {
                enable = false;
            }
        }
        public static void UpdateScopeStateWithoutCheck(int id, float posY)
        {
            Scopes[id] = posY;
        }

        public IEnumerator CheckPuzzleFinish()
        {
            yield return new WaitUntil(() =>
            {
                return !enable;
            });
            Debug.Log("finish");
            yield return new WaitUntil(() =>
            {
                return Input.GetMouseButton(0);
            });

            PuzzleManager.Solved();
            CurrentCoroutine = null;
        }

        public override void OnEnter()
        {
            
            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {
                
            }
            else
            {
                if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.NotFound)
                {
                    StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(tutorialCanvasGroup, 1f));
                    GameProgressData.Unlock(this);

                    tutorialCanvasGroup.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(tutorialCanvasGroup, 0f));
                    });
                }
                CurrentCoroutine = StartCoroutine(CheckPuzzleFinish());
            }

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() => {
                PuzzleManager.Exit();
            });
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
    }
}