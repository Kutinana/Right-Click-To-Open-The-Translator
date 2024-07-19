using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using Kuchinashi;
using System;
using Cameras;
using QFramework;

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
        public List<patternType> Patterns = new List<patternType>() { patternType.Circle, patternType.Circle, patternType.Circle };
        public List<Transform> CorrectPosY;
        public const float ERROR = 0.3f;
        public List<float> Scopes = new List<float>() { 0, 0, 0 };
        public List<Sprite> CTS;
        private Coroutine CurrentCoroutine = null;
        public bool enable = true;
        private CanvasGroup tutorialCanvasGroup;
        private bool CORRECT => Patterns[0] == patternType.Circle && Patterns[1] == patternType.Triangle && Patterns[2] == patternType.Square;
        private bool SCOPE_CORRECT => Mathf.Abs(Scopes[0] - Instance.CorrectPosY[0].position.y) <= ERROR
            && Mathf.Abs(Scopes[1] - Instance.CorrectPosY[1].position.y) <= ERROR
            && Mathf.Abs(Scopes[2] - Instance.CorrectPosY[2].position.y) <= ERROR;

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
            int val = ((int) Instance.Patterns[pos] + 1) % 3;
            Instance.Patterns[pos] = (patternType)val;
            switch (Instance.Patterns[pos])
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
            if (Instance.CORRECT)
            {
                S1Correct.Invoke();
            }
        }

        public static void PatternUpdateAll()
        {
            Image image;
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        image = Instance.transform.Find("Interactable/Hidden/Buttons/Left/Image").GetComponent<Image>();
                        break;
                    case 1:
                        image = Instance.transform.Find("Interactable/Hidden/Buttons/Middle/Image").GetComponent<Image>();
                        break;
                    case 2:
                        image = Instance.transform.Find("Interactable/Hidden/Buttons/Right/Image").GetComponent<Image>();
                        break;
                    default:
                        image = null;
                        break;
                }
                switch (Instance.Patterns[i])
                {
                    case patternType.Square:
                        image.sprite = Instance.CTS[2];
                        break;
                    case patternType.Triangle:
                        image.GetComponent<Image>().sprite = Instance.CTS[1];
                        break;
                    case patternType.Circle:
                        image.GetComponent<Image>().sprite = Instance.CTS[0];
                        break;
                    default:
                        break;
                }
            }

            if (Instance.CORRECT)
            {
                S1Correct.Invoke();
            }
        }

        public static void UpdateScopeState(int id, Vector3 pos)
        {
            Instance.Scopes[id] = pos.y;
            if (Mathf.Abs(Instance.Scopes[id] - Instance.CorrectPosY[id].position.y) <= ERROR)
            {
                Show(Instance, id);
            }
            else
            {
                Hide(Instance, id);
            }
            if (Instance.SCOPE_CORRECT && Instance.enable)
            {
                Instance.enable = false;
            }
        }
        public static void UpdateScopeStateWithoutCheck(int id, Vector3 pos)
        {
            Instance.Scopes[id] = pos.y;
            //Show(Instance, id);
        }

        public IEnumerator CheckPuzzleFinish()
        {
            yield return new WaitUntil(() => !enable);

            PuzzleManager.Solved();
            CurrentCoroutine = null;
        }

        public override void OnEnter()
        {
            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {
                transform.Find("Answer").gameObject.SetActive(true);
                transform.Find("Interactable").gameObject.SetActive(false);
                transform.Find("Tutorial").gameObject.SetActive(false);
            }
            else
            {
                if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.NotFound)
                {
                    StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(tutorialCanvasGroup, 1f));
                    GameProgressData.Unlock(this);
                    transform.Find("Interactable").GetComponent<HiddenPicture>().enabled = false;

                    tutorialCanvasGroup.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(tutorialCanvasGroup, 0f));
                        StartCoroutine(WaitTilTutEnd());
                    });
                }
                else
                {
                    transform.Find("Tutorial").gameObject.SetActive(false);
                }
                
                HiddenPicture picture = transform.Find("Interactable").GetComponent<HiddenPicture>();
                picture.setStage(picture.Stage);
                CurrentCoroutine = StartCoroutine(CheckPuzzleFinish());
            }

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() =>
            {
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
        
        private IEnumerator WaitTilTutEnd()
        {
            yield return new WaitUntil(() =>
            {
                return tutorialCanvasGroup.alpha <= 0.01f;
            });

            transform.Find("Interactable").GetComponent<HiddenPicture>().enabled = true;
            yield return null;
        }
    }
}