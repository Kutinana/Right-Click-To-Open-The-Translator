using DataSystem;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

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

        public List<Sprite> Lights;
        private Image Light;

        private void Awake()
        {
            Instance = this;

            blocks = new List<Item>(5);
            Towers = new List<Tower>(3);

            Light = transform.Find("Interactable/Lights").GetComponent<Image>();

            Initialize();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                BlockDown();
                int num = 0;
                foreach (var state in BlockStates)
                {
                    if (state == blockState.Middle) num++;
                }

                switch (num)
                {
                    case 0:
                        Light.sprite = Lights[0];
                        break;
                    case 1:
                        Light.sprite = Lights[1];
                        break;
                    case 2:
                        Light.sprite = Lights[2];
                        break;
                    case 3:
                        Light.sprite = Lights[3];
                        break;
                    case 4:
                        Light.sprite = Lights[4];
                        break;
                    case 5:
                        Light.sprite = Lights[5];
                        break;
                    default:
                        break;
                }
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