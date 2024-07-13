using System;
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

namespace Puzzle.InEnergy.Clock
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;

        public Transform Hour;
        public Transform Minute;

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
                Hour.localEulerAngles = new Vector3(0, 0, 45);
                Minute.localEulerAngles = new Vector3(0, 0, 315);

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
            yield return new WaitUntil(() => {
                return ((Hour.localEulerAngles.z % 360) + 360) % 360 > 310f
                    && ((Hour.localEulerAngles.z % 360) + 360) % 360 < 320f
                    && ((Minute.localEulerAngles.z % 360) + 360) % 360 > 40f
                    && ((Minute.localEulerAngles.z % 360) + 360) % 360 < 50f;
            });

            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));

            transform.GetComponentInChildren<HiddenMessage>().ShowHiddenMessage();
            PuzzleManager.Solved(isClosing: false);
            
            CurrentCoroutine = null;
        }
    }
}