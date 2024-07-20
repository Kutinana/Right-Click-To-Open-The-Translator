using DataSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;

namespace Puzzle.InEnergy.Cable
{
    public enum EnergyType
    {
        Red,
        Green,
        Yellow
    }

    public enum CableState
    {
        Up,
        Left,
        Down,
        Right
    }

    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;
        // private static string Path = "";
        private const int CAPACITY = 25;

        private List<CableState> CableStates;
        private List<Cable> Cables;
        private CanvasGroup m_characterGroup;

        private readonly Dictionary<int, List<CableState>> RedAnswer = new Dictionary<int, List<CableState>>()
        {
            {0, new List<CableState>(){CableState.Down} }, {1, new List<CableState>(){CableState.Down, CableState.Up} }, {2, new List<CableState>(){CableState.Up} },
            {5, new List<CableState>(){CableState.Left} }, {7, new List<CableState>(){CableState.Right, CableState.Left} }, {12, new List<CableState>(){CableState.Up, CableState.Down} },
            {17, new List<CableState>(){CableState.Left} }, {18, new List<CableState>(){CableState.Left} }, {23, new List<CableState>(){CableState.Left} },
            {24, new List<CableState>(){CableState.Right, CableState.Left} }
        };

        private readonly List<int> RedBlocks = new List<int>() { 0, 1, 2, 5, 7, 12, 17, 18, 23, 24 };

        private readonly Dictionary<int, List<CableState>> YellowAnswer = new Dictionary<int, List<CableState>>()
        {
            {1, new List<CableState>(){CableState.Down} }, {2, new List<CableState>(){CableState.Up, CableState.Down} }, {3, new List<CableState>(){CableState.Left} },
            {4, new List<CableState>(){CableState.Right} }, {6, new List<CableState>(){CableState.Right, CableState.Left} }, {7, new List<CableState>(){CableState.Left} },
            {8, new List<CableState>(){CableState.Up} }, {9, new List<CableState>(){CableState.Right, CableState.Left} }, {11, new List<CableState>(){CableState.Down, CableState.Up} },
            {12, new List<CableState>(){CableState.Up} }, {13, new List<CableState>(){CableState.Down, CableState.Up} }, {14, new List<CableState>(){CableState.Up} },
            {15, new List<CableState>(){CableState.Right} }, {16, new List<CableState>(){CableState.Down, CableState.Up} },
            {20, new List<CableState>(){CableState.Right} }, {21, new List<CableState>(){CableState.Up} }
        };

        private readonly List<int> YellowBlocks = new List<int>() { 1, 2, 3, 4, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 20, 21 };

        private readonly Dictionary<int, List<CableState>> GreenAnswer = new Dictionary<int, List<CableState>>()
        {
            {14, new List<CableState>(){CableState.Up} }, {15, new List<CableState>(){CableState.Right} }, {16, new List<CableState>(){CableState.Up} },
            {19, new List<CableState>(){CableState.Right, CableState.Left} }, {21, new List<CableState>(){CableState.Up} }, {22, new List<CableState>(){CableState.Right, CableState.Left} },
            {23, new List<CableState>(){CableState.Left} }, {24, new List<CableState>(){CableState.Right} }
        };

        private readonly List<int> GreenBlocks = new List<int>() { 14, 15, 16, 19, 21, 22, 23, 24 };

        private void Awake()
        {
            Instance = this;

            CableStates = new List<CableState>(CAPACITY);
            Cables = new List<Cable>(CAPACITY);
            m_characterGroup = transform.Find("Interactable/Characters").GetComponent<CanvasGroup>();

            if (GameProgressData.GetInventory().TryGetValue("greenLiquid", out var value) && value >= 1)
                transform.Find("Interactable/Cables/5,3").gameObject.SetActive(true);

            Initialize();

            DisableCharacters();

            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => EnableCharacters()).UnRegisterWhenGameObjectDestroyed(gameObject);
            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => DisableCharacters()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        private Coroutine CurrentCoroutine = null;
        private Button backButton;

        private void Initialize()
        {
            if (TempCableData.CableStates.Count != 0)
            {
                ReadData();
            }
            for (int i = 0; i < CAPACITY; i++)
            {
                int row = (i / 5) + 1;
                int column = (i % 5) + 1;
                string CableName = row + "," + column;
                Transform cable = transform.Find("Interactable/Cables/" + CableName);
                Cables.Add(cable.GetComponent<Cable>());
                if (TempCableData.CableStates.Count == 0) CableStates.Add(CableState.Up);
            }

            Refresh();
        }

        private void EnableCharacters()
        {
            m_characterGroup.interactable = true;
            m_characterGroup.blocksRaycasts = true;
        }

        private void DisableCharacters()
        {
            m_characterGroup.interactable = false;
            m_characterGroup.blocksRaycasts = false;
        }

        public static void CheckState()
        {
            Instance.SaveData();
            if ((GameProgressData.GetPuzzleProgress("SubPuzzleRed") == PuzzleProgress.NotFound || GameProgressData.GetPuzzleProgress("SubPuzzleRed") == PuzzleProgress.UnSolved)
                && Instance.CheckAnswer(0))
            {
                Instance.transform.Find("Interactable/SubPuzzles/SubPuzzleRed").GetComponent<SubPuzzleRed>().SubPuzzleSolved();
            }
            if ((GameProgressData.GetPuzzleProgress("SubPuzzleYellow") == PuzzleProgress.NotFound || GameProgressData.GetPuzzleProgress("SubPuzzleYellow") == PuzzleProgress.UnSolved)
                && Instance.CheckAnswer(1))
            {
                Instance.transform.Find("Interactable/SubPuzzles/SubPuzzleYellow").GetComponent<SubPuzzleYellow>().SubPuzzleSolved();
            }
            if ((GameProgressData.GetPuzzleProgress("SubPuzzleGreen") == PuzzleProgress.NotFound || GameProgressData.GetPuzzleProgress("SubPuzzleGreen") == PuzzleProgress.UnSolved)
                && Instance.CheckAnswer(2))
            {
                Instance.transform.Find("Interactable/SubPuzzles/SubPuzzleGreen").GetComponent<SubPuzzleGreen>().SubPuzzleSolved();
            }

            if (GameProgressData.GetPuzzleProgress("SubPuzzleRed") == PuzzleProgress.Solved 
                && GameProgressData.GetPuzzleProgress("SubPuzzleYellow") == PuzzleProgress.Solved
                && GameProgressData.GetPuzzleProgress("SubPuzzleGreen") == PuzzleProgress.Solved)
            {
                Instance.enabled = false;
            }
        }

        private bool CheckAnswer(int color)
        {
            Dictionary<int, List<CableState>> valuePairs = new Dictionary<int, List<CableState>>();
            List<int> KeyList = new List<int>();
            switch (color)
            {
                case 0:
                    valuePairs = RedAnswer;
                    KeyList = RedBlocks;
                    break;
                case 1:
                    valuePairs = YellowAnswer;
                    KeyList = YellowBlocks;
                    break;
                case 2:
                    valuePairs = GreenAnswer;
                    KeyList = GreenBlocks;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < KeyList.Count; i++)
            {
                int id = KeyList[i];
                bool flag = false;
                List<CableState> CorrectStates = valuePairs[id];
                for (int j = 0; j < CorrectStates.Count; j++)
                {
                    if (CableStates[id] == CorrectStates[j])
                    {
                        //Debug.Log(id + " " + true);
                        flag = true;
                    }
                }
                if (!flag) return false;
            }
            Debug.Log("subpuzzle solved");
            return true;
        }

        private void SaveData()
        {
            TempCableData.CableStates = this.CableStates;
        }

        private void ReadData()
        {
            this.CableStates = TempCableData.CableStates;
        }

        private void Refresh()
        {
            for (int i = 0; i < CAPACITY; i++)
            {
                Cables[i].SetState(CableStates[i]);
            }
        }

        public static void SetState(CableState cableState, Cable cable)
        {
            int id = Instance.Cables.IndexOf(cable);
            Instance.CableStates[id] = cableState;
            CheckState();
        }

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
            SaveData();

            if (CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);
                CurrentCoroutine = null;
            }
        }
        private IEnumerator CheckAnswerCoroutine()
        {
            yield return new WaitUntil(() => !enabled);

            PuzzleManager.Solved(isClosing: false);
            CurrentCoroutine = null;
        }
    }

    public static class TempCableData
    {
        public static List<CableState> CableStates = new List<CableState>();
    }
}