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
        Square,
        Triangle,
        Circle
    }

    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;

        private Button backButton;

        public static List<patternType> Patterns = new List<patternType>() { patternType.Square, patternType.Square, patternType.Square};
        public static List<float> CorrectPosY = new List<float>() { -82f, -300f, -220f};
        public const float ERROR = 20f;
        public static List<float> Scopes = new List<float>() { 0, 0, 0};
        public static List<Transform> m_scope;
        private Coroutine CurrentCoroutine = null;
        public static bool enable = true;

        private static bool CORRECT => Patterns[0] == patternType.Circle && Patterns[1] == patternType.Triangle && Patterns[2] == patternType.Square;
        private static bool SCOPE_CORRECT => Mathf.Abs(Scopes[0] - CorrectPosY[0]) <= ERROR && Mathf.Abs(Scopes[1] - CorrectPosY[1]) <= ERROR && Mathf.Abs(Scopes[2] - CorrectPosY[2]) <= ERROR;

        public static bool holding_scope = false;

        public static System.Action S1Correct;

        public static event EventHandler<int> Show;
        public static event EventHandler<int> Hide;

        private void Awake()
        {
            Instance = this;
        }

        public static void PatternUpdate(int pos, Transform button)
        {
            int val = ((int)Patterns[pos] + 1) % 3;
            Patterns[pos] = (patternType)val;
            switch (Patterns[pos])
            {
                case patternType.Square:
                    button.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Square";
                    break;
                case patternType.Triangle:
                    button.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Triangle";
                    break;
                case patternType.Circle:
                    button.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Circle";
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