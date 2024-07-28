using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InCenter.Elevator
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;
        private void Awake()
        {
            Instance = this;
        }

        private Button backButton;

        public override void OnEnter()
        {
            base.OnEnter();

            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {
                var third = transform.Find("Interactable/3F");
                third.GetComponent<ElevatorButton>().Interactable = true;
                for (var i = 0; i < third.childCount; i++)
                {
                    third.GetChild(i).gameObject.SetActive(true);
                }
            }
            else
            {
                var third = transform.Find("Interactable/3F");
                third.GetComponent<ElevatorButton>().Interactable = false;
                for (var i = 0; i < third.childCount; i++)
                {
                    third.GetChild(i).gameObject.SetActive(false);
                }
            }

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() =>
            {
                PuzzleManager.Exit();
            });

            List<string> ids = new List<string>();
            foreach (var c in GetComponentsInChildren<Character>())
            {
                ids.Add(c.data.Id);
            }
            UserDictionary.AddRelatedPuzzleAndSave(ids, Id);
        }

        public override void OnExit()
        {
            AudioKit.PlaySound("023Ting", volumeScale: .8f);
            base.OnExit();
        }

        public struct MoveTo { public int floor; }

        public void MoveToFloor(int F)
        {
            TypeEventSystem.Global.Send(new MoveTo { floor = F });
        }
    }
}
