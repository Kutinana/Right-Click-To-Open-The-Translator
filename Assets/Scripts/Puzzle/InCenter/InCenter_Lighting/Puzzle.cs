using DataSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InCenter.Lighting
{
    public enum LightState
    {
        On,
        Off
    }

    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;
        public List<CubeLighting> Lights;
        public Dictionary<CubeLighting, List<CubeLighting>> LightChain;
        private const int CAPACITY = 13;

        public List<Sprite> LightSprite;
        public List<int> CurrentStates = new List<int>()
        {
            1, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1
        };

        private void Awake()
        {
            Instance = this;

            LightChain = new Dictionary<CubeLighting, List<CubeLighting>>(CAPACITY);
            Lights = new List<CubeLighting>(CAPACITY);

            Initialize();
            SetLightStateAll();
        }

        private void Initialize()
        {
            for (int i = 0; i < CAPACITY; i++)
            {
                CubeLighting light = transform.Find("Interactable/cubes/" + (i+1)).GetComponent<CubeLighting>();
                Lights.Add(light);
                LightChain.Add(light, new List<CubeLighting>());
            }
            AddLightChain(1, 3); AddLightChain(2, 3); AddLightChain(3, 4);
            AddLightChain(5, 6); AddLightChain(2, 6); AddLightChain(6, 7);
            AddLightChain(3, 7); AddLightChain(7, 8);
            AddLightChain(4, 8); AddLightChain(8, 9);
            AddLightChain(6, 10); AddLightChain(10, 11);
            AddLightChain(7, 11); AddLightChain(11, 12); AddLightChain(11, 13);
            AddLightChain(8, 12);
        }

        private void SetLightStateAll()
        {
            for (int i = 0; i < CAPACITY; i++)
            {
                Lights[i].SetState((LightState)CurrentStates[i]);
            }
        }

        private void AddLightChain(int a, int b)
        {
            LightChain[Lights[a-1]].Add(Lights[b-1]);
            LightChain[Lights[b-1]].Add(Lights[a-1]);
        }

        private Coroutine CurrentCoroutine = null;
        private Button backButton;

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

        public static bool PuzzleFinish()
        {
            foreach (var lightState in Instance.CurrentStates)
            {
                if (lightState == 0) return false;
            }
            return true;
        }

        private IEnumerator CheckAnswerCoroutine()
        {
            yield return new WaitUntil(() => PuzzleFinish());

            PuzzleManager.Solved();
            CurrentCoroutine = null;
        }
    }
}

