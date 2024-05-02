using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Puzzle1
{
    public enum BottleType
    {
        Ball,
        Cube,
        Taper
    }
    
    public class Puzzle1 : PuzzleBase , ISingleton
    {
        public static Puzzle1 Instance => SingletonProperty<Puzzle1>.Instance;
        public void OnSingletonInit() {}
        public static Dictionary<int, BottleType> PositionDictionary = new Dictionary<int, BottleType>() {
            {0, BottleType.Taper},
            {1, BottleType.Cube},
            {2, BottleType.Ball}
        };

        public static readonly Dictionary<int, BottleType> CORRECT = new Dictionary<int, BottleType>() {
            {0, BottleType.Ball},
            {1, BottleType.Taper},
            {2, BottleType.Cube}
        };

        public Bottle Cube;
        public Bottle Taper;
        public Bottle Ball;

        public static Bottle HoldingBottle = null;

        private Button backButton;
        private Coroutine CurrentCoroutine = null;

        public static void Swap(Bottle bottleA, Bottle bottleB)
        {
            (PositionDictionary[bottleA.CurrentPosition], PositionDictionary[bottleB.CurrentPosition])
                = (PositionDictionary[bottleB.CurrentPosition], PositionDictionary[bottleA.CurrentPosition]);

            (bottleA.CurrentPosition, bottleB.CurrentPosition)
                = (bottleB.CurrentPosition, bottleA.CurrentPosition);
        }

        public static void ReArrangePosition()
        {
            switch (PositionDictionary[0])
            {
                case BottleType.Cube:
                    Instance.Cube.transform.localPosition = new Vector3(-300, 0, 0);
                    Instance.Cube.CurrentPosition = 0;
                    break;
                case BottleType.Taper:
                    Instance.Taper.transform.localPosition = new Vector3(0, 0, 0);
                    Instance.Taper.CurrentPosition = 0;
                    break;
                case BottleType.Ball:
                    Instance.Ball.transform.localPosition = new Vector3(-600, 0, 0);
                    Instance.Ball.CurrentPosition = 0;
                    break;
            }

            switch (PositionDictionary[1])
            {
                case BottleType.Cube:
                    Instance.Cube.transform.localPosition = new Vector3(0, 0, 0);
                    Instance.Cube.CurrentPosition = 1;
                    break;
                case BottleType.Taper:
                    Instance.Taper.transform.localPosition = new Vector3(300, 0, 0);
                    Instance.Taper.CurrentPosition = 1;
                    break;
                case BottleType.Ball:
                    Instance.Ball.transform.localPosition = new Vector3(-300, 0, 0);
                    Instance.Ball.CurrentPosition = 1;
                    break;
            }

            switch (PositionDictionary[2])
            {
                case BottleType.Cube:
                    Instance.Cube.transform.localPosition = new Vector3(300, 0, 0);
                    Instance.Cube.CurrentPosition = 2;
                    break;
                case BottleType.Taper:
                    Instance.Taper.transform.localPosition = new Vector3(600, 0, 0);
                    Instance.Taper.CurrentPosition = 2;
                    break;
                case BottleType.Ball:
                    Instance.Ball.transform.localPosition = new Vector3(0, 0, 0);
                    Instance.Ball.CurrentPosition = 2;
                    break;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {
                PositionDictionary = CORRECT;
                ReArrangePosition();

                foreach (var col in transform.Find("Interactable").GetComponentsInChildren<Bottle>())
                {
                    col.enabled = false;
                }
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
            yield return new WaitUntil(() => {
                return PositionDictionary[0] == CORRECT[0] && PositionDictionary[1] == CORRECT[1] && PositionDictionary[2] == CORRECT[2];
            });

            PuzzleManager.Solved();
            CurrentCoroutine = null;
        }
    }
}