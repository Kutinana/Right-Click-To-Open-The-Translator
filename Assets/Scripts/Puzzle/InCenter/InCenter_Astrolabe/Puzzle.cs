using DataSystem;
using Kuchinashi;
using Puzzle.Overture.Jigsaw;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InCenter.Astrolable
{
    public enum CubeType
    {
        Light,
        Dark,
        None
    }

    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;
        public bool solved = false;

        public List<Transform> ValidPoints;
        public List<CubeType> cubeTypes = new List<CubeType>() 
        {
            CubeType.None, CubeType.None, CubeType.None, CubeType.None, 
            CubeType.None, CubeType.None, CubeType.None, CubeType.None
        };

        private readonly List<CubeType> CORRECT = new List<CubeType>() 
        {
            CubeType.Light, CubeType.Light, CubeType.Light, CubeType.Dark,
            CubeType.Light, CubeType.Light, CubeType.Dark, CubeType.Dark
        };

        public List<Cube> CubesInBlock = new List<Cube>()
        {
            null, null, null, null, null, null, null, null
        };

        public const float ERROR = 0.5f;

        public static Cube HoldingCube = null;

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
                transform.Find("Interactable/Mask").gameObject.SetActive(false);
                transform.Find("Interactable/fire").gameObject.SetActive(false);

                solved = true;
            }
            else
            {
                if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.NotFound)
                {
                    GameProgressData.Unlock(this);
                }

                transform.Find("Interactable/fire").gameObject.SetActive(false);
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
            yield return new WaitUntil(() => CheckAnswer());

            transform.Find("Interactable/Mask").gameObject.SetActive(false);
            transform.Find("Interactable/fire").gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            GameProgressData.IncreaseInventory("fire");

            PuzzleManager.Solved(isClosing: false);
            CurrentCoroutine = null;
        }

        private bool CheckAnswer()
        {
            for (int i = 0; i < 8; i++)
            {
                if (cubeTypes[i] != CORRECT[i]) return false;
            }
            return true;
        }
    }
}