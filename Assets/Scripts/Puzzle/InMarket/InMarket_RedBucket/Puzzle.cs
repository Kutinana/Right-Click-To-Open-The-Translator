using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InMarket.RedBucket
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;
        private Button backButton;

        private void Awake()
        {
            Instance = this;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() => {
                PuzzleManager.Exit();
            });

            List<string> ids = new List<string>();
            foreach (var c in GetComponentsInChildren<Character>())
            {
                ids.Add(c.data.Id);
            }
            UserDictionary.AddRelatedPuzzleAndSave(ids, Id);
        }

        private void OnMouseUp()
        {
            if (InteractableObjectManager.Current != null && InteractableObjectManager.Current.Data.Id == "greenLiquid")
            {
                GetComponent<Collider2D>().enabled = false;
                AudioKit.PlaySound("023RedBucket");
                InteractableObjectManager.DropAndGet("yellowLiquid");
                GameProgressData.IncreaseInventory("yellowLiquid");

                PuzzleManager.Solved(delay: 2f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
