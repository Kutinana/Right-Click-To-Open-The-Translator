using DataSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using Translator;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Puzzle.InCenter.HanoiTower
{
    public enum blockState
    {
        Left,
        Middle,
        Right
    }

    public enum Size
    {
        Mini,
        Air,
        Standard,
        Pro,
        ProMax
    }

    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;

        public static List<float> Layer = new List<float>() { -185, -87, 14, 113, 214};
        public static List<blockState> BlockStates = new List<blockState>() { 
            blockState.Left, blockState.Left, blockState.Left, blockState.Left, blockState.Left
        };
        public static List<Tower> Towers;
        public static List<Item> blocks;
        public const float HoverPosition = 300f;
        public static Item CurrentItem = null;
        public static Tower CurrentTower = null;
        public static Tower OriginTower = null;

        private void Awake()
        {
            Instance = this;

            blocks = new List<Item>(5);
            Towers = new List<Tower>(3);

            Initialize();
            Debug.Log(Towers.Count);
            Debug.Log(blocks.Count);

            
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                BlockDown();
            }
        }

        private void Initialize()
        {
            for (int i = 0; i < 3; i++)
            {
                Towers.Add(transform.Find("Interactable/Towers/Tower" + (i + 1)).GetComponent<Tower>());
            }
            for (int i = 0; i < 5; i++)
            {
                Item temp = transform.Find("Interactable/Blocks/towerblock" + (i + 1)).GetComponent<Item>();
                blocks.Add(temp);
                Towers[0].ItemStack.Push(temp);
            }
        }

        private void BlockDown()
        {
            if (Puzzle.CurrentItem == null) return;

            if (Puzzle.OriginTower != Puzzle.CurrentTower)
            {
                if (!CurrentTower.SettleItem())
                {
                    Puzzle.CurrentItem.transform.localPosition = Puzzle.CurrentItem.InitialPosition;
                }
                else
                {
                    UpdateBlockState();
                    OriginTower.ItemStack.Pop();
                }
                clear();
            }
            if (Puzzle.OriginTower == Puzzle.CurrentTower && CurrentTower != null && CurrentItem != null)
            {
                Puzzle.CurrentItem.transform.localPosition = Puzzle.CurrentItem.InitialPosition;
                clear();
            }
            Debug.Log(BlockStates[0] + " " + BlockStates[1] + " " + BlockStates[2] + " " + BlockStates[3] + " " + BlockStates[4]);
        }

        private void clear()
        {
            Puzzle.CurrentItem.InitialPosition = Puzzle.CurrentItem.transform.localPosition;
            Puzzle.CurrentItem = null;
            Puzzle.CurrentTower = null;
            Puzzle.OriginTower = null;
        }

        private void UpdateBlockState()
        {
            int index = blocks.IndexOf(CurrentItem);
            BlockStates[index] = (blockState)Towers.IndexOf(CurrentTower);
        }

        private Coroutine CurrentCoroutine = null;
        private UnityEngine.UI.Button backButton;

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

            backButton = transform.Find("Menu/Back").GetComponent<UnityEngine.UI.Button>();
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
            foreach (var item in BlockStates)
            {
                if (item != blockState.Middle) return false;
            }
            return true;
        }

        private IEnumerator CheckAnswerCoroutine()
        {
            yield return new WaitUntil(() => PuzzleFinish());

            transform.Find("Interactable/Mask").gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            PuzzleManager.Solved();
            CurrentCoroutine = null;
        }
    }
}