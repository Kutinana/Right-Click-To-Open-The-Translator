using DataSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InMarket.Church
{
    public enum BlockState
    {
        Up,
        Left,
        Down,
        Right
    }

    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;
        private const int CAPACITY = 12;

        private List<BlockState> BlockStates;
        private List<Block> Blocks;

        private List<BlockState> CORRECT;

        public List<CanvasGroup> characters;

        private void Awake()
        {
            Instance = this;

            BlockStates = new List<BlockState>() { 
                BlockState.Down, BlockState.Left, BlockState.Right, BlockState.Down, 
                BlockState.Right, BlockState.Up, BlockState.Left, BlockState.Right,
                BlockState.Down, BlockState.Left, BlockState.Down, BlockState.Left
            };

            CORRECT = new List<BlockState>(CAPACITY);
            Blocks = new List<Block>(CAPACITY);

            Initialize();

            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => EnableCharacters()).UnRegisterWhenGameObjectDestroyed(gameObject);
            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => DisableCharacters()).UnRegisterWhenGameObjectDestroyed(gameObject);

            DisableCharacters();
        }

        private Coroutine CurrentCoroutine = null;
        private Button backButton;

        private void Initialize()
        {
            for (int i = 0; i < CAPACITY; i++)
            {
                int row = (i / 4) + 1;
                int column = (i % 4) + 1;
                string BlockName = row + "," + column;
                Transform block = transform.Find("Interactable/pictures/" + BlockName);
                Blocks.Add(block.GetComponent<Block>());
                CORRECT.Add(BlockState.Up);
            }
            Refresh();
        }

        private bool CheckState()
        {
            for (int i = 0; i < CAPACITY; i++)
            {
                if (BlockStates[i] != CORRECT[i]) return false;
            }
            return true;
        }

        private void Refresh()
        {
            for (int i = 0; i < CAPACITY; i++)
            {
                Blocks[i].SetState(BlockStates[i]);
            }
        }

        public static void SetState(BlockState blockState, Block block)
        {
            int id = Instance.Blocks.IndexOf(block);
            Instance.BlockStates[id] = blockState;
        }

        private void EnableCharacters()
        {
            foreach (var character in characters)
            {
                character.interactable = true;
                character.blocksRaycasts = true;
            }

        }

        private void DisableCharacters()
        {
            foreach (var character in characters)
            {
                character.interactable = false;
                character.blocksRaycasts = false;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {
                BlockStates = CORRECT;
                Refresh();

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

        private IEnumerator CheckAnswerCoroutine()
        {
            yield return new WaitUntil(() => CheckState());

            PuzzleManager.Solved();
            CurrentCoroutine = null;
        }
    }
}