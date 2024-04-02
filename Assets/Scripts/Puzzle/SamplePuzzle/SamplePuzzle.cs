using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Puzzle.SamplePuzzle
{
    public enum BottleType
    {
        Ball,
        Cube,
        Taper
    }
    
    public class SamplePuzzle : PuzzleBase , ISingleton
    {
        public static SamplePuzzle Instance => SingletonProperty<SamplePuzzle>.Instance;
        public void OnSingletonInit() {}
        public static Dictionary<int, BottleType> PositionDictionary = new Dictionary<int, BottleType>() {
            {0, BottleType.Cube},
            {1, BottleType.Taper},
            {2, BottleType.Ball}
        };

        public static readonly Dictionary<int, BottleType> CORRECT = new Dictionary<int, BottleType>() {
            {0, BottleType.Ball},
            {1, BottleType.Cube},
            {2, BottleType.Taper}
        };

        public Bottle Cube;
        public Bottle Taper;
        public Bottle Ball;

        public static Bottle HoldingBottle = null;

        private static Coroutine CurrentCoroutine = null;

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
                    Instance.Cube.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case BottleType.Taper:
                    Instance.Taper.transform.localPosition = new Vector3(-300, 0, 0);
                    break;
                case BottleType.Ball:
                    Instance.Ball.transform.localPosition = new Vector3(-600, 0, 0);
                    break;
            }

            switch (PositionDictionary[1])
            {
                case BottleType.Cube:
                    Instance.Cube.transform.localPosition = new Vector3(300, 0, 0);
                    break;
                case BottleType.Taper:
                    Instance.Taper.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case BottleType.Ball:
                    Instance.Ball.transform.localPosition = new Vector3(-300, 0, 0);
                    break;
            }

            switch (PositionDictionary[2])
            {
                case BottleType.Cube:
                    Instance.Cube.transform.localPosition = new Vector3(600, 0, 0);
                    break;
                case BottleType.Taper:
                    Instance.Taper.transform.localPosition = new Vector3(300, 0, 0);
                    break;
                case BottleType.Ball:
                    Instance.Ball.transform.localPosition = new Vector3(0, 0, 0);
                    break;
            }
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            CurrentCoroutine = StartCoroutine(CheckAnswerCoroutine());
        }

        protected override void OnExit()
        {
            base.OnExit();

            if (CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);
                CurrentCoroutine = null;
            }
        }

        protected override void OnComplete()
        {
            base.OnComplete();

            Debug.Log("Complete");
        }

        private IEnumerator CheckAnswerCoroutine()
        {
            yield return new WaitUntil(() => PositionDictionary.Equals(CORRECT));

            OnComplete();
            CurrentCoroutine = null;
        }
    }
}