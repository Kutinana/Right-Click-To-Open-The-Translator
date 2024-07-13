using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using Kuchinashi;
using QFramework;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.WeighBeaker
{
    public enum InteractTarget
    {
        None,
        Tank,
        Balance,
        Tap,
        Bottle_4,
        Bottle_6,
        Bottle_9
    }

    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;

        public Bottle HoldingBottle = null;
        public Bottle OnBalanceBottle = null;
        public InteractTarget Target = InteractTarget.None;
        public Tap Tap;
        public Balance Balance;

        public SerializableDictionary<InteractTarget, Bottle> Bottles;
        public List<int> States => new List<int>() {
            Bottles[InteractTarget.Bottle_4].State, Bottles[InteractTarget.Bottle_6].State, Bottles[InteractTarget.Bottle_9].State
        };

        private Button backButton;
        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            Instance = this;
        }

        public override void OnEnter()
        {
            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {
                Balance.ChangeState(1);

                var bottle = Bottles[InteractTarget.Bottle_9];
                OnBalanceBottle = bottle;
                bottle.ChangeState(7);
                bottle.transform.localPosition = new Vector3(bottle.OnBalance.x, bottle.OnBalance.y - 65);

                foreach (var c in GetComponentsInChildren<Collider2D>(includeInactive: true))
                {
                    c.enabled = false;
                }

                transform.Find("Menu/Back").GetComponent<Button>().onClick.AddListener(() => {
                    PuzzleManager.Exit();
                });
                return;
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
            
            GameProgressData.Unlock(this);
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
            yield return new WaitUntil(() => OnBalanceBottle?.State == 7);

            PuzzleManager.Solved();
            CurrentCoroutine = null;
        }
    }
}